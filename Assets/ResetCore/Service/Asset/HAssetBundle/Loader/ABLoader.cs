using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ResetCore.Util;

namespace ResetCore.HAsset
{
    public enum ABLoadState
    {
        EStateNone = 0,
        EStateLoading = 1,
        EStateLoadBundle = 2,
        EStateError = 3,
        EStateComplete = 4
    }

    public class ABLoader
    {
        #region 私有域

        private ABLoadState _state = ABLoadState.EStateNone;
        private uint _abName;
        private ABData _abData;
        private ABInfo _abInfo;
        private AssetBundle _bundle;

        private List<ABInfo> _depInfoList;
        private List<ABLoader> _depLoaderList;
        private int _loadingDepCount = 0;

        #endregion

        #region 公开属性

        /// <summary>
        /// 加载状态
        /// </summary>
        public ABLoadState State { get { return _state; } }
        /// <summary>
        /// BUndle名
        /// </summary>
        public uint ABName { get { return _abName; } }
        /// <summary>
        /// Bundle数据
        /// </summary>
        public ABData ABData { get { return _abData; } }
        /// <summary>
        /// Bundle状态
        /// </summary>
        public ABInfo ABInfo { get { return _abInfo; } }
        /// <summary>
        /// 加载的AssetBundle
        /// </summary>
        public AssetBundle Bundle { get { return _bundle; } }
        /// <summary>
        /// 是否已经加载完成
        /// </summary>
        public bool IsComplete { get { return _state == ABLoadState.EStateError || _state == ABLoadState.EStateComplete; } }

        /// <summary>
        /// 加载完成
        /// </summary>
        public Action<ABInfo> Callback { get; set; }

        #endregion


        public ABLoader(uint abName, ABData abData)
        {
            _abName = abName;
            _abData = abData;
        }

        #region 公共函数

        /// <summary>
        /// 分析完依赖后将已加载好依赖的资源推送到AssetBundleManager，申请加载
        /// </summary>
        /// <param name="immediately"></param>
        public void Load(bool immediately, params Action<ABInfo>[] callbacks)
        {
            for (int i = 0; i < callbacks.Length; i++)
            {
                Callback += callbacks[i];
            }
            
            switch (_state)
            {
                case ABLoadState.EStateNone:
                    _state = ABLoadState.EStateLoading;
                    this.LoadDepends(immediately);
                    break;
                case ABLoadState.EStateError:
                    this.OnError();
                    break;
                case ABLoadState.EStateComplete:
                    this.OnComplete();
                    break;
            }
        }
        /// <summary>
        /// 其它都准备好了，加载AssetBundle
        /// 注意：这个方法只能被 ABManager 调用
        /// 由 Manager 统一分配加载时机，防止加载过卡
        /// </summary>
        public void LoadBundle(bool immediately, Action<ABInfo> callback = null)
        {
            if (callback != null)
            {
                Callback += callback;
            }

            string cachePath = Path.Combine(ABPathResolver.BundleCacheDir, string.Format("{0}.ab", _abName));
            string srcPath = ABPathResolver.GetBundleSourceFile(string.Format("{0}.ab", _abName), false);

            string loadPath = File.Exists(cachePath) ? cachePath : srcPath;

            if (immediately)
            {
                LoadFromFileSync(loadPath);
            }
            else
            {
                _state = ABLoadState.EStateLoadBundle;
                ABManager.Instance.StartCoroutine(LoadFromFileAsync(loadPath));
            }
        }

        #endregion 公共函数

        #region 私有函数

        /// <summary>
        /// 同步加载Bundle
        /// </summary>
        /// <param name="path"></param>
        private void LoadFromFileSync(string path)
        {
            _bundle = AssetBundle.LoadFromFile(path);
            if (_bundle)
                OnComplete();
            else
                OnError();
        }

        /// <summary>
        /// 异步加载Bundle
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private IEnumerator LoadFromFileAsync(string path)
        {
            AssetBundleCreateRequest abReq = AssetBundle.LoadFromFileAsync(path);
            while (!abReq.isDone)
            {
                yield return null;
            }
            _bundle = abReq.assetBundle;

            if (_bundle)
                OnComplete();
            else
                OnError();
        }

        /// <summary>
        /// 加载依赖
        /// </summary>
        /// <param name="immediately"></param>
        private void LoadDepends(bool immediately)
        {
            //设置依赖加载器列表
            if (_depLoaderList == null)
            {
                _depLoaderList = ListPool<ABLoader>.Get();
                _depInfoList = ListPool<ABInfo>.Get();

                for (int i = 0; i < _abData.dependencies.Length; i++)
                {
                    uint abName = _abData.dependencies[i];
                    ABInfo abInfo = ABManager.Instance.GetLoadedABInfo(abName);
                    if (abInfo != null)
                    {
                        abInfo.ResetLifeTime();
                        _depInfoList.Add(abInfo);
                    }
                    else
                    {
                        ABLoader loader = ABManager.Instance.CreateLoader(abName);
                        _depLoaderList.Add(loader);
                    }
                }
            }

            //进行加载并且
            ABLoader depLoader;
            int loadDepCount = _loadingDepCount = _depLoaderList.Count;

            if (_loadingDepCount == 0)
            {
                ABManager.Instance.RequestLoadBundle(this, immediately);
            }

            for (int i = 0; i < loadDepCount; i++)
            {
                depLoader = _depLoaderList[i];
                if (!depLoader.IsComplete)
                {
                    depLoader.Callback += (abInfo) =>
                    {
                        _loadingDepCount--;
                        if (_loadingDepCount == 0 && State != ABLoadState.EStateLoading)
                        {
                            ABManager.Instance.RequestLoadBundle(this, immediately);
                        }
                    };
                    depLoader.Load(immediately);
                }
            }
        }

        /// <summary>
        /// 加载完成
        /// </summary>
        private void OnComplete()
        {
            if (_abInfo == null)
            {
                _state = ABLoadState.EStateComplete;

                //设置abInfo
                _abInfo = CreateBundleInfo();

                ListPool<ABInfo>.Return(_depInfoList);
                _depInfoList = null;
                ListPool<ABLoader>.Return(_depLoaderList);
                _depLoaderList = null;

                _bundle = null;
            }

            if (Callback != null)
            {
                Callback(_abInfo);
                Callback = null;
            }

        }

        /// <summary>
        /// 将BundleInfo置为null并且将状态设为None
        /// </summary>
        /// <param name="abInfo"></param>
        public void OnBundleUnload(ABInfo abInfo)
        {
            _abInfo = null;
            _state = ABLoadState.EStateNone;
        }

        /// <summary>
        /// 加载错误，释放依赖资源
        /// </summary>
        private void OnError()
        {
            _state = ABLoadState.EStateError;
            _abInfo = null;

            ListPool<ABInfo>.Return(_depInfoList);
            _depInfoList = null;

            ListPool<ABLoader>.Return(_depLoaderList);
            _depLoaderList = null;

            if (Callback != null)
            {
                Callback(_abInfo);
                Callback = null;
            }
        }


        /// <summary>
        /// 创建ABInfo
        /// </summary>
        /// <param name="loader"></param>
        /// <param name="assetBundle"></param>
        /// <returns></returns>
        private ABInfo CreateBundleInfo()
        {
            _abInfo = new ABInfo(ABName, ABData, Bundle);
            _abInfo.IsReady = true;
            _abInfo.ResetLifeTime();
            _abInfo.OnUnloaded = OnBundleUnload;
            for (int i = 0; i < _depInfoList.Count; i++)
            {
                _abInfo.AddDependency(_depInfoList[i]);
            }
            for (int i = 0; i < _depLoaderList.Count; i++)
            {
                _abInfo.AddDependency(_depLoaderList[i]._abInfo);
            }
            return _abInfo;
        }

        #endregion 共有函数

    }
}
