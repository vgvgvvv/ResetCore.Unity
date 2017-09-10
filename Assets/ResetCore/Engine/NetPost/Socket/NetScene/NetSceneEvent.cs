using UnityEngine;
using System.Collections;

namespace ResetCore.NetPost
{
    public static class NetSceneEvent
    {

        public static readonly string NetSceneReady = "NetSceneEvent.NetSceneReady";
        public static readonly string NetSceneDisconnect = "NetSceneEvent.NetSceneDisconnect";

        public static readonly string NetBehaviorAddToScene = "NetSceneEvent.NetBehaviorAddToScene";
        public static readonly string NetBehaviorRemoveFromScene = "NetSceneEvent.NetBehaviorRemoveFromScene";
        /// <summary>
        /// 获取处理事件的名字
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="handlerId"></param>
        /// <returns></returns>
        public static string GetNetBehaviorEventName(HandlerConst.RequestId handlerId)
        {
            return "NetBehavior" + "_" + handlerId;
        }
    }

}
