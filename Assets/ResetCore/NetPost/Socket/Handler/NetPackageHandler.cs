using UnityEngine;
using System.Collections;
using System;
using ResetCore.Event;
using ResetCore.Util;

namespace ResetCore.NetPost
{
    public abstract class NetPackageHandler
    {
        /// <summary>
        /// 处理服务器
        /// </summary>
        protected BaseServer ownerServer;
        /// <summary>
        /// 处理函数
        /// </summary>
        /// <param name="package"></param>
        /// <param name="act"></param>
        protected abstract void Handle(Package package, Action act = null);

        /// <summary>
        /// 处理包行为，分配给相应的Handler
        /// </summary>
        /// <param name="package"></param>
        /// <param name="act"></param>
        public static void HandlePackage(BaseServer server, Package package, Action act = null)
        {
            //触发响应事件（针对有响应的请求）
            HandlerConst.RequestId id = EnumEx.GetValue<HandlerConst.RequestId>(package.eventId);
            Debug.unityLogger.Log("收到请求！" + id.ToString());
            if (HandlerConst.handlerDict.ContainsKey(id))
            {
                HandlerConst.handlerDict[id].ownerServer = server;
                HandlerConst.handlerDict[id].Handle(package, act);
            }
            else
            {
                Debug.unityLogger.Log("不存在id：" + id.ToString());
            }
            EventDispatcher.TriggerEventWithTag<Package>(ServerEvent.GetResponseEvent(package.requestId), package, server);
            if (act != null) {
				act ();
			}
        }

    }

   
}
