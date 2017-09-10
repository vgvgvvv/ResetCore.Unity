using UnityEngine;
using System.Collections;
using ResetCore.Util;
using System;

namespace ResetCore.NetPost
{
    public abstract class NetScene : MonoBehaviour
    {
        protected bool isRunning = false;
        protected float fps = 25;

        /// <summary>
        /// 连接之前
        /// </summary>
        public virtual void OnBeforeConnect()
        {

        }

        /// <summary>
        /// 连接之后
        /// </summary>
        public virtual void OnAfterConnect(bool result)
        {
            
        }

        /// <summary>
        /// 断开连接前
        /// </summary>
        public virtual void OnBeforeDisconnect()
        {

        }

        /// <summary>
        /// 断开连接后
        /// </summary>
        public virtual void OnAfterDisconnect(bool result)
        {

        }

        /// <summary>
        /// 处理来自服务端的快照
        /// </summary>
        public abstract void HandleSnapshot(Package pkg);
    }

    public abstract class NetScene<T> : NetScene
    {

        public override void OnAfterConnect(bool result)
        {
            base.OnAfterConnect(result);
            if (!result)
                return;

            isRunning = true;
        }

        public override void OnBeforeDisconnect()
        {
            base.OnBeforeDisconnect();
            //停止发送快照
            isRunning = false;
        }

        /// <summary>
        /// 创建快照
        /// </summary>
        public abstract T CreateSnapshotData();

        /// <summary>
        /// 发送快照
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="sendType"></param>
        public void SendSceneSnapshot(T value, SendType sendType = SendType.UDP)
        {
            NetSceneManager.Instance.currentServer.Send
                         (HandlerConst.RequestId.SceneSnapshotHandlerId, NetSceneManager.Instance.currentSceneId, value, sendType);
        }

        
    }
}
