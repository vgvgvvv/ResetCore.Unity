//***************************

//文件名称(File Name)： ABManager.cs 

//功能描述(Description)： AssetBundle资源管理

//数据表(Tables)： nothing

//作者(Author)： zzr

//Create Date: 2017.08.10

//修改记录(Revision History)： nothing

//***************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ResetCore.Debugger;
using ResetCore.Util;

namespace ResetCore.HAsset
{
    public class ABManager : MonoBehaviour
    {
        /// <summary>
        /// 版本
        /// </summary>
        public static Version version = new Version(0, 1, 0);

        #region 常量
        /// <summary>
        /// 管理器名称
        /// </summary>
        public const string NAME = "AssetBundleManager";

        /// <summary>
        /// 是否允许Log
        /// </summary>
        public const bool ENABLE_LOG = true;

        private const int LOADING_LIMIT = 4; // 同时最大的加载数
        private const int UNLOAD_LIMIT = 20; // 一次最大卸载数量

        #endregion 常量

        #region 单例

        private static ABManager _Instance;
        public static ABManager Instance { get { return _Instance; } }

        #endregion 单例

        #region 私有域

        /// <summary>
        /// 目前申请加载的Loader队列
        /// </summary>
        private readonly Queue<ABLoader> _loadRequestQueue = new Queue<ABLoader>();
        
        /// <summary>
        /// 已经加载完成的AssetBundle
        /// </summary>
        private readonly Dictionary<uint, ABInfo> _loadedABDict = new Dictionary<uint, ABInfo>();

        /// <summary>
        /// 正在加载的Loader列表
        /// </summary>
        private readonly Dictionary<uint, ABLoader> _loaderCacheDict = new Dictionary<uint, ABLoader>();
        
        /// <summary>
        /// 卸载队列
        /// 在不加载的时候卸载, 优化：如果是根，则可以 unload(false) 以节省内存
        /// </summary>
        private readonly Queue<ABInfo> _requestUnloadBundleQueue = new Queue<ABInfo>();
        
        /// <summary>
        /// 用于获取所有Assetbundle信息
        /// </summary>
        private ABDataHelper dataHelper;

        private ReCoroutineTaskManager.CoroutineTask unloadTask;

        #endregion 私有域

        #region 共有属性

        /// <summary>
        /// 已经加载完的Bundle数量
        /// </summary>
        public int LoadedBundleCount { get { return _loadedABDict.Count; } }

        /// <summary>
        /// 目前正在加载的缓存
        /// </summary>
        public bool IsCurrentLoading { get { return _loaderCacheDict.Count > 0; } }

        #endregion


        void Awake()
        {
            _Instance = this;
            unloadTask = ReCoroutineTaskManager.Instance.LoopTodoByTime(() =>
            {
                this.UnloadUnusedBundle();
            }, 5, -1, this);
        }

        void OnDestroy()
        {
            RemoveAll();
            unloadTask.Stop();
        }

        #region public functions


        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            //读取AB数据
            dataHelper = new ABDataHelper();

            string localPath = ABPathResolver.GetBundleSourceFile(ABPathResolver.DEPEND_FILE_NAME, false);
            string cachePath = string.Format("{0}/{1}", ABPathResolver.BundleCacheDir, ABPathResolver.DEPEND_FILE_NAME);

            Debug.Log("######## " + localPath);
            if (File.Exists(localPath))
            {
                dataHelper.readFromPath(localPath);
            }
            else if (File.Exists(cachePath))
            {
                dataHelper.readFromPath(cachePath);
            }
        }

        /// <summary>
        /// 移除所有AssetBundle及加载内容
        /// </summary>
        public void RemoveAll()
        {
            this.StopAllCoroutines();

            _loadRequestQueue.Clear();
            foreach (ABInfo abi in _loadedABDict.Values)
            {
                abi.Dispose();
            }
            _loadedABDict.Clear();
            _loaderCacheDict.Clear();
            _requestUnloadBundleQueue.Clear();
        }

        /// <summary>
        /// 异步加载AssetBundle
        /// </summary>
        public void Load(uint hash, string location, string suffix, Action<ABInfo> handler = null)
        {
            //防止报空
            if (dataHelper == null)
            {
                if (handler != null)
                {
                    handler(null);
                }
                return;
            }

            //找缓存找到则直接返回
            ABInfo abInfo = GetLoadedABInfo(hash);
            if (abInfo != null)
            {
                if (handler != null)
                    handler(abInfo);
                return;
            }

            //创建Loader
            ABLoader loader = CreateLoader(hash);

            #region 异常情况处理
            if (loader == null)
            {
                if (ENABLE_LOG)
                    ReDebug.LogWarning(ReLogType.System, "ABManager", string.Format("Cannot create ABLoader, location={0}{1}, name={2}", location, suffix, hash));

                if (handler != null)
                    handler(null);

                return;
            }

            //如果已经加载完成
            if (loader.IsComplete)
            {
                ReDebug.LogWarning(ReLogType.System, "ABManager", String.Format("Cannot be here, name={0}", hash));
                if (handler != null)
                    handler(loader.ABInfo);

                return;
            }
            #endregion 异常情况处理

            //进入加载状态
            if (loader.State == ABLoadState.EStateNone)
            {
                loader.Load(false, handler, (res) =>
                {
                    _loadedABDict[res.ABName] = res;
                    _loaderCacheDict.Remove(loader.ABName);
                    loadBundlesInQueue();
                });
            }   
        }

        /// <summary>
        /// 同步加载AssetBundle
        /// </summary>
        public ABInfo LoadImmediately(uint hash, string location, string suffix)
        {
            //防止报空
            if (dataHelper == null)
            {
                ReDebug.LogWarning(ReLogType.System, "ABManager", "dataHelper is null");
                return null;
            }

            //获取AB信息如果存在则直接返回
            ABInfo abInfo = GetLoadedABInfo(hash);
            if (abInfo != null)
            {
                return abInfo;
            }

            //缓存中没有就进行加载
            ABLoader loader = CreateLoader(hash);

            #region 异常情况处理
            if (loader == null)
            {
                if (ENABLE_LOG)
                {
                    ReDebug.LogWarning(ReLogType.System, "ABManager",
                        string.Format("Cannot create ABLoader, location={0}{1}, name={2}", location, suffix, hash));
                }
                return null;
            }

            //如果完成就直接返回
            if (loader.IsComplete)
            {
                ReDebug.LogWarning(ReLogType.System, "ABManager", string.Format("Cannot be here, name={0}", hash));
                return loader.ABInfo;
            }
            #endregion 异常情况处理

            //进行加载
            if (loader.State == ABLoadState.EStateNone)
            {
                loader.Load(true, (res) =>
                {
                    _loadedABDict[res.ABName] = res;
                    _loaderCacheDict.Remove(loader.ABName);
                    loadBundlesInQueue();
                });
            }

            return loader.ABInfo;
        }


        /// <summary>
        /// 添加要卸载的AssetBundle
        /// </summary>
        public void AddUnloadBundleQueue(ABInfo info)
        {
            _requestUnloadBundleQueue.Enqueue(info);
            if (_requestUnloadBundleQueue.Count > 4)
            {
                UnloadUnusedBundle();
            }
        }

        /// <summary>
        /// 卸载不用的AssetBundle
        /// </summary>
        public void UnloadUnusedBundle(bool force = false)
        {
            if ((!IsCurrentLoading) || force)
            {
                int unloadCount = 0;
                while (_requestUnloadBundleQueue.Count > 0 && unloadCount < UNLOAD_LIMIT)
                {
                    ABInfo abInfo = _requestUnloadBundleQueue.Dequeue();
                    if (abInfo != null)
                    {
                        abInfo.UnloadBundle();
                        unloadCount++;
                    }
                }

                if (unloadCount < UNLOAD_LIMIT)
                {
                    List<uint> keys = ListPool<uint>.Get();
                    keys.AddRange(_loadedABDict.Keys);
                    for (int i = 0; i < keys.Count && unloadCount < UNLOAD_LIMIT; i++)
                    {
                        ABInfo abInfo = _loadedABDict[keys[i]];
                        if (abInfo.IsUnused)
                        {
                            abInfo.Dispose();
                            _loadedABDict.Remove(abInfo.ABName);
                            unloadCount++;
                        }
                    }

                    ListPool<uint>.Return(keys);
                }
            }
        }
        #endregion

        #region internal functions
        /// <summary>
        /// 创建加载器
        /// </summary>
        internal ABLoader CreateLoader(uint abFileName)
        {
            //如果已经存在Laoder则返回loader
            if (_loaderCacheDict.ContainsKey(abFileName))
            {
                return _loaderCacheDict[abFileName];
            }

            //获取相应的AB数据
            ABData data = dataHelper.GetABData(abFileName);
            if (data == null)
            {
                ReDebug.LogWarning(ReLogType.System, "ABManager",
                    string.Format("ABData is null, abName={0}", abFileName));
                return null;
            }

            //创建Loader并且加入表中
            ABLoader loader = new ABLoader(data.fullName, data);
            _loaderCacheDict[abFileName] = loader;

            return loader;
        }

        /// <summary>
        /// 请求加载Bundle，这里统一分配加载时机，防止加载太卡
        /// </summary>
        internal void RequestLoadBundle(ABLoader loader, bool immediately)
        {
            if (immediately)
            {
                loader.LoadBundle(true);
            }
            else
            {
                _loadRequestQueue.Enqueue(loader);
                loadBundlesInQueue();
            }
        }

        /// <summary>
        /// 获取已加载完成的AssetBundleInfo
        /// </summary>
        internal ABInfo GetLoadedABInfo(uint abName)
        {
            ABInfo abInfo = null;
            _loadedABDict.TryGetValue(abName, out abInfo);
            return abInfo;
        }

        private void loadBundlesInQueue()
        {
            int loadCount = 0;
            while (_loadRequestQueue.Count > 0 && loadCount < LOADING_LIMIT)
            {
                ABLoader loader = _loadRequestQueue.Dequeue();
                loader.LoadBundle(false);
                loadCount++;
            }
        }


        #endregion


    }
}
