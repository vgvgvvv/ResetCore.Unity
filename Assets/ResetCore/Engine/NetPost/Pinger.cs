using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ResetCore.NetPost
{
    public class Pinger
    {
        /// <summary>
        /// 尝试ping主机
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool Ping(string ip)
        {
            System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping();
            System.Net.NetworkInformation.PingOptions options = new System.Net.NetworkInformation.PingOptions();
            options.DontFragment = true;
            string data = "Test Data!";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 1000; // Timeout 时间，单位：毫秒  
            System.Net.NetworkInformation.PingReply reply = p.Send(ip, timeout, buffer, options);
            if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                return true;
            else
                return false;
        }
    }

}
