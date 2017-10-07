using System;
using System.Collections;
using System.Collections.Generic;
using ResetCore.Debugger;
using UnityEngine;

namespace ResetCore.SAsset
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

    public class ABObject : RefObject
    {
        private const float MIN_LIFE_TIME = 2f; // 如果没有其它东西引用的情况下，此AB最小生存时间（单位秒）
        private const float MIN_SCENE_TIME = 20f; // 场景AB最小生存时间（单位秒）

        private AssetBundle _bundle;
        //准备完毕时的系统时间
        private float _readyTime;

        private HashSet<ABObject> _deps = new HashSet<ABObject>();
        private UnityEngine.Object _mainObject;
        private Sprite[] _sprites;

        public uint abName { get; private set; }

        public ABData abData { get; private set; }

        public bool isReady { get; set; }

        public bool isScene { get; set; }

        public UnityEngine.Object mainObject
        {
            get
            {
                tryLoadMainObjAndSprites();
                return _mainObject;
            }
        }

        public Sprite[] sprites
        {
            get
            {
                tryLoadMainObjAndSprites();
                return _sprites;
            }
        }

        public Action<ABObject> OnUnloaded { get; private set; }

        public ABObject(uint abName, ABData data, AssetBundle bundle)
        {
            this.abName = abName;
            this.abData = data;
            this._bundle = bundle;
        }

        /// <summary>
        /// 是否为失效资源
        /// </summary>
        public bool isUnused
        {
            get
            {
                float lifeTime = isScene ? MIN_SCENE_TIME : MIN_LIFE_TIME;
                return isReady && Time.time - _readyTime > lifeTime && refCount <= 0;
            }
        }

        /// <summary>
        /// 是否可被销毁
        /// </summary>
        public bool canDestroy
        {
            get
            {
                return mainObject is GameObject || mainObject is Material || mainObject is ScriptableObject;
            }
        }

        /// <summary>
        /// 重置生命时间
        /// </summary>
        public void ResetLifeTime()
        {
            if (isReady)
            {
                _readyTime = Time.time;
            }
        }

        /// <summary>
        /// 添加依赖
        /// </summary>
        /// <param name="target"></param>
        internal void AddDependency(ABObject target)
        {
            if (target != null && _deps.Add(target))
            {
                target.Retain();
            }
        }

        public override void Dispose()
        {
            UnloadBundle();
            var e = _deps.GetEnumerator();
            while (e.MoveNext())
            {
                ABObject dep = e.Current;
                dep.Release();
            }
            _deps.Clear();
            if (OnUnloaded != null)
            {
                OnUnloaded(this);
            }
            if (_mainObject && !canDestroy)
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
                ReDebug.Log(ReLogType.System, "ABObject", "Unload : " + abData.compositeType + " >> " + abName + "(" + abData.debugName + ")");
                _bundle.Unload(false);
            }
            _bundle = null;
        }

        private void tryLoadMainObjAndSprites()
        {
            if (!_bundle)
                return;

            if (!isScene && _mainObject == null)
            {
                string[] assets = _bundle.GetAllAssetNames();
                if (assets.Length > 0)
                {
                    _mainObject = _bundle.LoadAsset(assets[0]);
                }
                else
                {
                    isScene = true;
                }
            }
            if (!isScene && _sprites == null)
            {
                _sprites = _bundle.LoadAllAssets<Sprite>();
            }
        }

    }

}
