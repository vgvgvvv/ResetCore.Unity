using UnityEngine;
using System.Collections;
using System;

namespace ResetCore.PlatformHelper
{
    public class AndroidDevice : Device
    {

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="json"></param>
        public override void SendMessage(string eventName, string json)
        {
            JavaManager.mainActivityObject.Call("OnReceiveUnityMessage", eventName, json);
        }


    }

}
