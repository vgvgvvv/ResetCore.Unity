using System.Collections;
using System.Collections.Generic;
using ResetCore.Util;
using UnityEngine;

namespace ResetCore.SAsset
{

    public class ABContext
    {

        public readonly int LOADING_LIMIT = 4; // 同时最大的加载数
        public readonly int UNLOAD_LIMIT = 20; // 一次最大卸载数量

        /// <summary>
        /// 用于获取所有Assetbundle信息
        /// </summary>
        public readonly ABDataHelper dataHelper = new ABDataHelper();

        /// <summary>
        /// 已加载的Bundle
        /// </summary>
        public readonly Dictionary<uint, ABObject> loadedABDict = new Dictionary<uint, ABObject>();

        /// <summary>
        /// 正在加载的词典
        /// </summary>
        public readonly Dictionary<uint, ABLoader> loadingLoaderDict = new Dictionary<uint, ABLoader>();

        /// <summary>
        /// 目前申请加载的Loader队列
        /// </summary>
        public readonly Queue<ABLoader> waitingLoaderQueue = new Queue<ABLoader>();

        /// <summary>
        /// 目前正在加载的数目
        /// </summary>
        public int loadingCount { get; set; }

        /// <summary>
        /// 是否正在加载
        /// </summary>
        public bool isLoading { get { return loadingLoaderDict.Count > 0; } }


        /// <summary>
        /// 寻找缓存
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public ABObject FindABObjectCache(uint hash)
        {
            ABObject res = null;
            loadedABDict.TryGetValue(hash, out res);
            return res;
        }

        

        /// <summary>
        /// 重置上下文
        /// </summary>
        public void Reset()
        {
            loadingCount = 0;
            loadingLoaderDict.ForEach((k, v) =>
            {
                v.loadTask.Stop();
            });
            loadedABDict.ForEach((k, v) =>
            {
                v.Dispose();
            });
            loadedABDict.Clear();
            loadingLoaderDict.Clear();
            waitingLoaderQueue.Clear();
        }

    }
}
