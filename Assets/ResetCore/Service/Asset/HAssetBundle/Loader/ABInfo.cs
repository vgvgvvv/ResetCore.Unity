using System;
using System.Collections.Generic;
using ResetCore.Debugger;
using UnityEngine;

namespace ResetCore.HAsset
{
    public enum ABExportType
    {
        /// <summary>
        /// 普通素材，被根素材依赖的
        /// </summary>
        Asset = 1,
        /// <summary>
        /// 根
        /// </summary>
        Root = 1 << 1,
        /// <summary>
        /// 需要单独打包，说明这个素材是被两个或以上的素材依赖的
        /// </summary>
        Standalone = 1 << 2,
        /// <summary>
        /// 既是根又是被别人依赖的素材
        /// </summary>
        RootAsset = Asset | Root
    }

    /// <summary>
    /// Bundle数据
    /// </summary>
    public class ABData
    {
        public uint fullName;
        public string hash;
        public string debugName;
        public ABExportType compositeType;
        public uint[] dependencies;
        public bool isAnalyzed;
        public ABData[] dependList;
    }

    /// <summary>
    /// Bundle状态
    /// </summary>
    public class ABInfo
    {
        private const float MIN_LIFE_TIME = 2f; // 如果没有其它东西引用的情况下，此AB最小生存时间（单位秒）
        private const float MIN_SCENE_TIME = 20f; // 场景AB最小生存时间（单位秒）

        public Action<ABInfo> OnUnloaded;

        private AssetBundle _bundle;
        private float _readyTime; // 准备完毕时的时间

        private uint _abName;
        private ABData _data;
        private bool _isReady; // 标记当前是否准备完毕
        private int _refCount;
        private bool _isScene;

        private HashSet<ABInfo> _deps = new HashSet<ABInfo>();
        private UnityEngine.Object _mainObject;
        private Sprite[] _sprites;

        public uint ABName { get { return _abName; } }
        public ABData ABData { get { return _data; } }
        public int RefCount { get { return _refCount; } }
        public bool IsReady { get { return _isReady; } set { _isReady = value; } }
        public bool IsScene { get { return _isScene; } }

        public ABInfo(uint abName, ABData data, AssetBundle bundle)
        {
            _abName = abName;
            _data = data;
            _bundle = bundle;
        }

        

        /// <summary>
        /// 这个资源是否不用了
        /// </summary>
        public bool IsUnused
        {
            get
            {
                float lifeTime = _isScene ? MIN_SCENE_TIME : MIN_LIFE_TIME;
                return _isReady && Time.time - _readyTime > lifeTime && RefCount <= 0;
            }
        }

        /// <summary>
        /// 是否可以被销毁
        /// </summary>
        public bool CanDestroy
        {
            get
            {
                return _mainObject is GameObject || _mainObject is Material || _mainObject is ScriptableObject;
            }
        }

        /// <summary>
        /// 主要资源
        /// </summary>
        public UnityEngine.Object MainObject
        {
            get
            {
                tryLoadMainObjAndSprites();
                return _mainObject;
            }
        }

        /// <summary>
        /// 获取图集中的元素
        /// </summary>
        public Sprite[] Sprites
        {
            get
            {
                tryLoadMainObjAndSprites();
                return _sprites;
            }
        }

        /// <summary>
        /// 重置生命周期
        /// </summary>
        public void ResetLifeTime()
        {
            if (_isReady)
            {
                _readyTime = Time.time;
            }
        }

        /// <summary>
        /// 引用计数增一
        /// </summary>
        public void Retain()
        {
            _refCount++;
        }

        /// <summary>
        /// 引用计数减一
        /// </summary>
        public void Release()
        {
            _refCount--;
        }

        /// <summary>
        /// 释放AssetBundle
        /// </summary>
        public virtual void Dispose()
        {
            if (_bundle)
            {
                UnloadBundle();
                _bundle = null;
            }
            var e = _deps.GetEnumerator();
            while (e.MoveNext())
            {
                ABInfo dep = e.Current;
                dep.Release();
            }
            _deps.Clear();
            if (OnUnloaded != null)
            {
                OnUnloaded(this);
            }

            if (_mainObject && !CanDestroy)
            {
                Resources.UnloadAsset(_mainObject);
            }
            _mainObject = null;
            _sprites = null;
        }

        public void UnloadBundle()
        {
            if (_bundle != null)
            {
                if (ABManager.ENABLE_LOG)
                {
                    ReDebug.Log(ReLogType.System, "ABObject", "Unload : " + _data.compositeType + " >> " + _abName + "(" + _data.debugName + ")");
                }
                _bundle.Unload(false);
            }
            _bundle = null;
        }

        /// <summary>
        /// 添加依赖（内部调用）
        /// </summary>
        internal void AddDependency(ABInfo target)
        {
            if (target != null && _deps.Add(target))
            {
                target.Retain();
            }
        }

        private void tryLoadMainObjAndSprites()
        {
            if(_bundle)
            {
                if (!_isScene && _mainObject == null)
                {
                    string[] assets = _bundle.GetAllAssetNames();
                    if (assets.Length > 0)
                    {
                        _mainObject = _bundle.LoadAsset(assets[0]);
                    }
                    else
                    {
                        _isScene = true;
                    }
                }
                if (!_isScene && _sprites == null)
                {
                    _sprites = _bundle.LoadAllAssets<Sprite>();
                }

                // 优化，如果是导出类型Root且MainObject不是GameObject和Material的话可以提前释放AssetBundle，节省内存
                if (_data.compositeType == ABExportType.Root && !CanDestroy && !_isScene)
                {
                    ABManager.Instance.AddUnloadBundleQueue(this);
                }
            }
        }
    }
}
