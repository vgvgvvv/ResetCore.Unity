using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ResetCore.Debugger;
using ResetCore.Util;
using UnityEngine;

namespace ResetCore.SAsset
{
    public enum ABLoadState
    {
        Ready,
        LoadingDep,
        LoadSelf,
        Error, 
        Complete
    }

    public class ABLoader : IStorable
    {
        /// <summary>
        /// BUndle名
        /// </summary>
        public uint abName { get; private set; }
        /// <summary>
        /// Bundle数据
        /// </summary>
        public ABData abData { get; private set; }

        /// <summary>
        /// Bundle物体
        /// </summary>
        public ABObject abObject { get; private set; }

        /// <summary>
        /// Loader目前状态
        /// </summary>
        public ABLoadState state { get; private set; }

        /// <summary>
        /// AssetBundle
        /// </summary>
        public AssetBundle bundle { get; private set; }

        /// <summary>
        /// 是否已经完成
        /// </summary>
        public bool isComplete { get { return state == ABLoadState.Complete || state == ABLoadState.Error; } }

        /// <summary>
        /// 加载上下文
        /// </summary>
        public ABContext context { get; private set; }

        /// <summary>
        /// 加载任务
        /// </summary>
        public CoroutineTaskManager.CoroutineTask loadTask { get; private set; }


        private List<ABObject> depObjectList;
        private List<ABLoader> depLoaderList;
        private int loadingDepCount = 0;
        

        /// <summary>
        /// 加载完成
        /// </summary>
        public Action<ABObject> Callback { get; private set; }

        /// <summary>
        /// 获取Loader
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static ABLoader GetLoader(uint hash, ABContext context)
        {
            //如果已经存在Laoder则返回loader
            if (context.loadingLoaderDict.ContainsKey(hash))
            {
                return context.loadingLoaderDict[hash];
            }

            ABData data = context.dataHelper.GetABData(hash);
            if (data == null)
            {
                ReDebug.LogError(ReLogType.System, "ABManager",
                    string.Format("ABData is null, abName={0}", hash));
                return null;
            }

            //创建Loader并且加入表中
            ABLoader loader = ReusableObjectPool<ABLoader>.Get();
            loader.abName = data.fullName;
            loader.abData = data;
            context.loadingLoaderDict[hash] = loader;
            loader.context = context;

            return loader;
        }

        public ABLoader()
        {
        }

        #region 公开函数
        
        /// <summary>
        /// 加载内容
        /// </summary>
        /// <param name="immediately"></param>
        /// <param name="callbacks"></param>
        public void Load(bool immediately, params Action<ABObject>[] callbacks)
        {
            for (int i = 0; i < callbacks.Length; i++)
            {
                Callback += callbacks[i];
            }

            if (state != ABLoadState.Ready)
                return;

            //加入到队列逐个进行加载
            LoadDepends(immediately, RequestLoadBundle);
        }

        #endregion 公开函数


        #region 私有函数

        private void LoadDepends(bool immediately, Action<bool> loadSelf)
        {
            if (depLoaderList == null)
            {
                depLoaderList = ListPool<ABLoader>.Get();
                depObjectList = ListPool<ABObject>.Get();
                for (int i = 0; i < abData.dependencies.Length; i++)
                {
                    uint hash = abData.dependencies[i];
                    ABObject abObject = context.FindABObjectCache(hash);
                    if (abObject != null)
                    {
                        abObject.ResetLifeTime();
                        depObjectList.Add(abObject);
                    }
                    else
                    {
                        ABLoader loader = GetLoader(hash, context);
                        depLoaderList.Add(loader);
                    }
                }
            }

            ABLoader depLoader;
            int loadDepCount = loadingDepCount = depLoaderList.Count;

            if (loadDepCount == 0)
            {
                loadSelf(immediately);
            }
            else
            {
                for (int i = 0; i < loadDepCount; i++)
                {
                    depLoader = depLoaderList[i];
                    if (depLoader.isComplete)
                        continue;

                    depLoader.Load(immediately, (abObject) =>
                    {
                        loadingDepCount--;
                        if (loadingDepCount == 0 && state != ABLoadState.LoadingDep)
                        {
                            loadSelf(immediately);
                        }
                    });
                }
            }
        }

        /// <summary>
        /// 请求加载Bundle
        /// </summary>
        /// <param name="immediately"></param>
        private void RequestLoadBundle(bool immediately)
        {
            if (immediately)
            {
                LoadBundle(true);
            }
            else
            {
                context.waitingLoaderQueue.Enqueue(this);
                LoadWaitingLoader();
            }
        }

        /// <summary>
        /// 加载Bundle
        /// </summary>
        /// <param name="immediately"></param>
        private void LoadBundle(bool immediately)
        {
            string cachePath = Path.Combine(ABPathResolver.BundleCacheDir, string.Format("{0}.ab", abName));
            string srcPath = ABPathResolver.GetBundleSourceFile(string.Format("{0}.ab", abName), false);

            string loadPath = File.Exists(cachePath) ? cachePath : srcPath;

            if (immediately)
            {
                LoadFromFileSync(loadPath);
                OnTaskFinish(true);
            }
            else
            {
                state = ABLoadState.LoadSelf;
                loadTask = CoroutineTaskManager.Instance.AddTask(LoadFromFileAsync(loadPath), (succ) =>
                {
                    OnTaskFinish(succ);
                    context.loadingCount--;
                    LoadWaitingLoader();
                });
            }
        }

        private void LoadFromFileSync(string path)
        {
            bundle = AssetBundle.LoadFromFile(path);
        }

        private IEnumerator LoadFromFileAsync(string path)
        {
            AssetBundleCreateRequest abReq = AssetBundle.LoadFromFileAsync(path);
            while (!abReq.isDone)
            {
                yield return null;
            }
            bundle = abReq.assetBundle;
        }

        /// <summary>
        /// 加载等待队列中的Loader
        /// </summary>
        public void LoadWaitingLoader()
        {
            while (context.waitingLoaderQueue.Count > 0 && context.loadingCount < context.LOADING_LIMIT)
            {
                ABLoader loader = context.waitingLoaderQueue.Dequeue();
                loader.LoadBundle(false);
                context.loadingCount++;
            }
        }


        /// <summary>
        /// 创建ABObject
        /// </summary>
        /// <returns></returns>
        private ABObject CreateABObject()
        {
            ABObject abObject = new ABObject(abName, abData, bundle);
            abObject.isReady = true;
            abObject.ResetLifeTime();

            for (int i = 0; i < depObjectList.Count; i++)
            {
                abObject.AddDependency(depObjectList[i]);
            }

            for (int i = 0; i < depLoaderList.Count; i++)
            {
                abObject.AddDependency(depLoaderList[i].abObject);
            }

            return abObject;
        }

        #endregion


        #region 回调函数

        /// <summary>
        /// 当加载任务结束，无论成功或者失败
        /// </summary>
        /// <param name="succ"></param>
        private void OnTaskFinish(bool succ)
        {
            if (bundle)
            {
                state = ABLoadState.Complete;
                OnComplete();
            }
            else
            {
                state = ABLoadState.Error;
                OnError();
            }

            if(Callback != null)
                Callback(abObject);

            ListPool<ABObject>.Return(depObjectList);
            depObjectList = null;

            ListPool<ABLoader>.Return(depLoaderList);
            depLoaderList = null;

            bundle = null;

            context.loadingLoaderDict.Remove(abName);
            //回收Loader
            ReusableObjectPool<ABLoader>.Return(this);
        }

        /// <summary>
        /// 当加载成功时
        /// </summary>
        private void OnComplete()
        {
            if (abObject == null)
            {
                abObject = CreateABObject();
                context.loadedABDict[abObject.abName] = abObject;
            }
        }

        /// <summary>
        /// 当加载失败时
        /// </summary>
        private void OnError()
        {
            ReDebug.LogError(ReLogType.System, "ABLoader", string.Format("load fail abName = {0}", abData.debugName));
        }

        #endregion 回调函数


        #region IRestorable
        public bool isDirty { get; set; }
        public void Reset()
        {
            state = ABLoadState.Ready;
        }

        public void BeforeStoreToPool()
        {
            ListPool<ABObject>.Return(depObjectList);
            depObjectList = null;
            ListPool<ABLoader>.Return(depLoaderList);
            depLoaderList = null;

            abData = null;
            abObject = null;
            bundle = null;
            Callback = null;
            context = null;
            if (loadTask != null)
            {
                loadTask.Stop();
                loadTask = null;
            }
            state = ABLoadState.Ready;
        }
        #endregion
    }

}
