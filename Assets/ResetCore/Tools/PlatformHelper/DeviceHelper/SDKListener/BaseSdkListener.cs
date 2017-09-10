using UnityEngine;
using System.Collections;

namespace ResetCore.PlatformHelper
{
    public abstract class BaseSdkListener
    {

        /// <summary>
        /// 用于接受来自设备的Json消息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        public virtual void ReceiveMessageByJson(string name, string data) { }

        /// <summary>
        /// 用于接受来自设备的byte消息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        public virtual void ReceiveMessageByByte(string name, byte[] data) { }
    }
}
