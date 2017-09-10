using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using ResetCore.Util;

namespace ResetCore.NetPost
{
    public class PackageReciver
    {
        /// <summary>
        /// 当前服务器
        /// </summary>
        private BaseServer server;
        public PackageReciver(BaseServer server)
        {
            this.server = server;
        }

        /// <summary>
        /// 包队列
        /// </summary>
        private List<Package> packageList = new List<Package>();

        /// <summary>
        /// 处理队列
        /// </summary>
        private ActionQueue handleQueue = new ActionQueue();

        /// <summary>
        /// 数据缓冲区
        /// </summary>
        private byte[] packetBuffer = new byte[0];

        public void ReceivePackage(int len, byte[] data)
        {
            byte[] finalData = data.SubArray(0, len);

            packetBuffer = packetBuffer.Concat(finalData);

            //标记是否有完整的包接收到
            bool hasCompletePacket = false;
            do
            {
                int packSize = GetDataLength(packetBuffer);
                //Debug.Log("packSize " + packSize);
                //Debug.Log("packetBuffer.Length " + packetBuffer.Length);
                if (packSize == 0)
                {
                    break;
                }
                hasCompletePacket = packetBuffer.Length >= packSize;
                
                if (hasCompletePacket)
                {
                    byte[] packBytes = ArrayEx.CutHeadByLength(ref packetBuffer, packSize);
                    handleQueue.AddAction((act) =>
                    {
                        NetPackageHandler.HandlePackage(server, Package.PrasePackage(packBytes), act);
                    });
                }

            } while (hasCompletePacket);

        }

        /// <summary>
        /// 重置接收器
        /// </summary>
        public void Reset()
        {
            handleQueue.Clean();
            packageList = null;
            packageList = new List<Package>();
            packetBuffer = null;
            packetBuffer = new byte[0];
        }

        /// <summary>
        /// 获取包长度
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private int GetDataLength(byte[] data)
        {
            if (data.Length < Package.headLength)
                return 0;

            byte[] lengthByte = data.SubArray(0, 4);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(lengthByte);
            int length = BitConverter.ToInt32(lengthByte, 0);

            return length;
        }

    }
}
