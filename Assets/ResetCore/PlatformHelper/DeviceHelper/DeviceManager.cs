using UnityEngine;
using System.Collections;
using ResetCore.Util;
using System.Collections.Generic;
using System;
using System.Xml.Linq;
using System.IO;
using ResetCore.Event;

namespace ResetCore.PlatformHelper
{
    public class DeviceManager : MonoSingleton<DeviceManager>
    {

        private Device _currentDevice = null;

        private Dictionary<string, BaseSdkListener> sdkListenerDict = new Dictionary<string, BaseSdkListener>();

        public Device currentDevice
        {
            get
            {
                if (_currentDevice == null)
                {
                    _currentDevice = Device.GetDevice();
                }
                return _currentDevice;
            }
        }

        public override void Init()
        {
            base.Init();
            TextAsset constXmlAsset = Resources.Load<TextAsset>("PlatformConst");
            if(constXmlAsset == null)
            {
                Debug.unityLogger.LogError("PlatformHelper", "PlatformConst加载失败");
                return;
            }
            Propertys constProp = Propertys.LoadXml(constXmlAsset.text);
            string sdkList = constProp.GetProperty("SdkList");
            if (!string.IsNullOrEmpty(sdkList))
            {
                LoadAllSdk(sdkList);
            }
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// 加载所有的sdk
        /// </summary>
        /// <param name="sdkList"></param>
        private void LoadAllSdk(string sdkList)
        {
            List<string> list = sdkList.GetValue<List<string>>();
            foreach(string sdk in list)
            {
                RegistSdk(sdk);
            }
        }

        /// <summary>
        /// 注册Sdk
        /// </summary>
        /// <param name="sdkName"></param>
        private void RegistSdk(string sdkName)
        {
#if DLLMANAGER
            Type sdkType = ReAssembly.AssemblyManager.GetDefaultAssemblyType(sdkName + "SdkListener");

            if(sdkType == null)
            {
                Debug.unityLogger.LogError("PlatformHelper", "未找到类：" + sdkName);
                return;
            }

            BaseSdkListener listener = Activator.CreateInstance(sdkType) as BaseSdkListener;
            
            if(listener == null)
            {
                Debug.unityLogger.LogError("PlatformHelper", "创建失败：" + sdkName);
                return;
            }

            sdkListenerDict.Add(sdkName, listener);
#else
            Debug.logger.LogError("PlatformHelper", "创建失败：需要引入DLLMANAGER模块");
#endif
        }


        /// <summary>
        /// 用于接受来自设备的Json消息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        public void ReceiveMessageByJson(string name, string json)
        {
            foreach(BaseSdkListener listener in sdkListenerDict.Values)
            {
                listener.ReceiveMessageByJson(name, json);
            }
        }
        /// <summary>
        /// 用于接受来自设备的byte消息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        public void ReceiveMessageByByte(string name, byte[] data)
        {
            foreach (BaseSdkListener listener in sdkListenerDict.Values)
            {
                listener.ReceiveMessageByByte(name, data);
            }
        }

        /// <summary>
        /// 通过发送消息来传递
        /// </summary>
        /// <param name="eventName"></param>
        public void ReveiveMessageBySendEvent(string eventName, string json)
        {
            EventDispatcher.TriggerEvent<string>(eventName, json);
        }

    }

}
