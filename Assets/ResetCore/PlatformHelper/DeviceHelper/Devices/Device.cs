using UnityEngine;
using System.Collections;

namespace ResetCore.PlatformHelper
{
    public abstract class Device
    {

        protected Device() { }

        /// <summary>
        /// 工厂模式创建Device
        /// </summary>
        /// <returns></returns>
        public static Device GetDevice()
        {
            if (Application.platform == RuntimePlatform.Android)
                return new AndroidDevice();
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
                return new IOSDevice();
            else if (Application.platform == RuntimePlatform.WindowsEditor ||
                Application.platform == RuntimePlatform.WindowsPlayer)
                return new PCDevice();
            return null;
        }

        /// <summary>
        /// 发送消息，以Json形式发送
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="json"></param>
        public abstract void SendMessage(string eventName, string json = "");

        /// <summary>
        /// 重启应用
        /// </summary>
        public void RestartApp()
        {
            SendMessage(DeviceCommand.RESTART);
        }

        /// <summary>
        /// 安装应用
        /// </summary>
        public void InstallApp(string path)
        {
            SendMessage(DeviceCommand.RESTART, path);
        }


        /// <summary>
        /// 设备等级
        /// </summary>
        private int deviceLevel = -100;

        /// <summary>
        /// 设备内存等级
        /// </summary>
        private int memoryLevel = -100;

        /// <summary>
        /// 获取设备内存等级
        /// 1.800M内存及以下的机器
        /// 2.1.6G内存及以下的机器
        /// 3.1.6G及以上的机器
        /// </summary>
        public virtual int MemoryLevel
        {
            get
            {
                //是否判断过内存等级
                if (memoryLevel != -100)
                    return memoryLevel;

                int mem = SystemInfo.systemMemorySize;

                //小于800M内存的机器
                if (mem < 800)
                    memoryLevel = 1;
                //小于1.6G内存的机器
                else if (mem < 1600)
                    memoryLevel = 2;
                //大于1.6G内存的机器
                else
                    memoryLevel = 3;

                //返回机器内存
                return memoryLevel;
            }
        }

        /// <summary>
        /// 获取设备等级：
        /// 0.未知
        /// 1.低端机
        /// 2.中端机
        /// 3.高端机
        /// </summary>
        public virtual int DeviceLevel
        {
            get
            {
                if (deviceLevel != -100)
                    return deviceLevel;

                ////是否特殊机型
                //int special = DeviceManager.CheckSpecialDevice();
                //if (special != 0)
                //    return special;

                //未知机型
                if (SystemInfo.processorFrequency == 0 || SystemInfo.systemMemorySize == 0)
                    deviceLevel = 0;

                //低端机
                if (SystemInfo.processorFrequency < 1100 || SystemInfo.systemMemorySize < 800)
                    deviceLevel = 1;

                //高端机
                if (SystemInfo.processorFrequency > 1600 && SystemInfo.systemMemorySize > 1600)
                    deviceLevel = 3;

                //其他都是中端机
                if (deviceLevel == -100)
                    deviceLevel = 2;

                if (Application.platform == RuntimePlatform.IPhonePlayer)
                    deviceLevel = 3;

                return deviceLevel;
            }
        }

        /// <summary>
        /// 获取安卓设备ID
        /// </summary>
        /// <remarks>
        /// 设备ID计算方法：
        /// MD5(IMEI + Mac + SerialNO)
        /// </remarks>
        public virtual string DeviceID
        {
            get
            {
                //采用Unity的算法
                return SystemInfo.deviceUniqueIdentifier.ToLower();
            }
        }
    }

}
