using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ResetCore.Debugger;
using ResetCore.Util;
using UnityEngine;

namespace ResetCore.SAsset
{
    public class ABManager : MonoSingleton<ABManager>
    {

        private ABContext context = new ABContext();


        #region 生命周期

        public override void Init()
        {
            context.dataHelper.Init();
            ReCoroutineTaskManager.Instance.LoopTodoByTime(() =>
            {
                UnloadUnusedBundle();
            }, 5, -1, gameObject);
        }

        

        void OnDestroy()
        {
            context.Reset();
        }


        #endregion 生命周期

        #region 公开函数

        /// <summary>
        /// 异步加载Bundle
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="location"></param>
        /// <param name="suffix"></param>
        /// <param name="callBack"></param>
        public void LoadAsync(uint hash, string location, string suffix, Action<ABObject> callBack = null)
        {

            //从缓存中寻找
            ABObject abObject = context.FindABObjectCache(hash);
            if(abObject != null)
            {
                if(callBack != null)
                    callBack(abObject);
                return;
            }

            var loader = ABLoader.GetLoader(hash, context);

            #region 异常情况处理
            if (loader == null)
            {
                ReDebug.LogError(ReLogType.System, "ABManager", string.Format("Cannot create ABLoader, location={0}{1}, name={2}", location, suffix, hash));

                if (callBack != null)
                    callBack(null);

                return;
            }

            //如果已经加载完成
            if (loader.isComplete)
            {
                ReDebug.LogError(ReLogType.System, "ABManager", String.Format("Cannot be here, name={0}", hash));
                if (callBack != null)
                    callBack(loader.abObject);

                return;
            }
            #endregion 异常情况处理

            loader.Load(false, callBack);

        }

        /// <summary>
        /// 同步加载Bundle
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="location"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public ABObject LoadSync(uint hash, string location, string suffix)
        {

            //从缓存中寻找
            ABObject abObject = context.FindABObjectCache(hash);
            if (abObject != null)
            {
                return abObject;
            }

            var loader = ABLoader.GetLoader(hash, context);

            #region 异常情况处理
            if (loader == null)
            {
                ReDebug.LogError(ReLogType.System, "ABManager", string.Format("Cannot create ABLoader, location={0}{1}, name={2}", location, suffix, hash));
                return null;
            }

            //如果已经加载完成
            if (loader.isComplete)
            {
                ReDebug.LogError(ReLogType.System, "ABManager", String.Format("Cannot be here, name={0}", hash));
                return loader.abObject;
            }
            #endregion 异常情况处理

            loader.Load(true);

            return loader.abObject;

        }

        /// <summary>
        /// 卸载无用的Bundle
        /// </summary>
        /// <param name="force"></param>
        public void UnloadUnusedBundle(bool force = false)
        {
            if (context.isLoading && !force)
                return;

            int unloadCount = 0;

        }


        #endregion 公开函数

        #region 私有函数





        #endregion

    }

}
