using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace ResetCore.NetPost
{
    public static class HandlerConst
    {
        /// <summary>
        /// 请求Id 1~1000
        /// </summary>
        public enum RequestId
        {
            //通常消息Id为 1~100
            RegistChannelHandler = 2,//将用户连接到频道中
            NetObjectJoinUpHandler = 3,//场景物体注册到场景中
            RequsetSceneHandler = 4,//请求场景
            NetObjectRemoveHandler = 5,//从场景中移除物体
            GetChannelIdHandler = 6,//得到当前频道id
            DisconnectSceneHandler = 7,//断开频道

            SceneSnapshotHandlerId = 10000,//场景快照通用请求Id

            //NetBehavior消息Id为1XX
            NetTransform = 101,//Transform
        }
        
        /// <summary>
        /// 凡是会收到回复的都需要添加Handler以进行处理
        /// </summary>
        public static Dictionary<RequestId, NetPackageHandler> handlerDict = new Dictionary<RequestId, NetPackageHandler>()
        {
            {RequestId.RegistChannelHandler, new ResistChannelHandler()},
            {RequestId.GetChannelIdHandler, new GetChannelIdHandler()},
            {RequestId.SceneSnapshotHandlerId, new NetSceneSnapShotHandler() },

            {RequestId.NetTransform, new BaseNetObjectHandler()},
        };

        
    }

}
