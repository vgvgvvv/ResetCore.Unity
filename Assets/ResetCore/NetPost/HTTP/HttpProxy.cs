using UnityEngine;
using System.Collections;
using ResetCore.Util;
using LitJson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace ResetCore.NetPost
{
    public class HttpProxy : MonoSingleton<HttpProxy>
    {

        public void AsynDownloadJsonData(string url, JsonData jsonData, Action<JsonData> callback, Action<float> progressAct)
        {
            string json = jsonData.ToJson();
            PrintJson(json);

            WWWForm form = new WWWForm();

            Dictionary<string, string> headers = form.headers;
            headers["Content-Type"] = "application/json";

            byte[] bytes = Encoding.UTF8.GetBytes(json);
            WWW www = new WWW(url, bytes, headers);
            StartCoroutine(WaitForRequest(www, callback, progressAct));
        }

        #region 私有函数
        private static void PrintJson(string json)
        {
            Debug.Log("###############################");
            Debug.Log(json);
            Debug.Log("###############################");
        }

        //等待返回
        private IEnumerator WaitForRequest(WWW www, Action<JsonData> finishAct, Action<float> progressAct = null)
        {

            long starttime = DateTime.Now.Ticks;
            float timeout = 3.0f;

            while (!www.isDone)
            {
                Debug.unityLogger.Log("下载中");
                if (IsTimeout(starttime, timeout))
                {
                    Debug.LogError("超时！");
                    HandleTimeout(finishAct);
                    www.Dispose();
                    yield break;
                }
                if (progressAct != null)
                    progressAct(www.progress);
                yield return www;

            }
            Debug.Log(www.text);
            if (www.isDone)
            {
                
                if (!string.IsNullOrEmpty(www.error))
                {
                    HandleError(www, finishAct);
                }
                else
                {
                    HandleFinalWWW(www, finishAct);
                }
                www.Dispose();
            }
        }

        private static bool IsTimeout(long starttime, float timeout)
        {
            return (DateTime.Now.Ticks - starttime) / 10000000.0f > timeout;
        }
        private static void HandleTimeout(Action<JsonData> action)
        {
            Debug.unityLogger.Log("超时！");
            if (action != null)
            {
                action("time");
            }
        }
        private static void HandleFinalWWW(WWW www, Action<JsonData> action)
        {
            Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            Debug.Log("GetPackageTask !!! is:  " + www.text);
            Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");

            if (action != null)
            {
                JsonReader reader = new JsonReader(www.text);
                JsonData data = JsonMapper.ToObject(reader);
                action(data);
            }
        }
        private static void HandleError(WWW www, Action<JsonData> action)
        {
            if (action != null)
            {
                action("erro");
            }

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.unityLogger.LogError("Net", "网络异常");
            }
            else if (www.error == "couldn't connect to host" || www.error.Contains("Failed to connect to")
                || www.error.Contains("Could not connect to the server")
                || www.error.Contains("404 Not Found"))
            {
                Debug.unityLogger.LogError("Net", "网络异常");
            }
            Debug.LogError("www is error! " + www.error);
            Debug.Log("www is error! " + www.error);
        }
        #endregion


        public static string GetMD5String(string str)
        {
            MD5 md5 = MD5.Create();
            byte[] data = Encoding.UTF8.GetBytes(str);
            byte[] data2 = md5.ComputeHash(data);
            return GetbyteToString(data2);
        }
        private static string GetbyteToString(byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }
            return sb.ToString();
        }

        //数据库加密
        public static string GenerateFileMD5Upper(string url)
        {
            if (File.Exists(url) == false)
                return string.Empty;

            byte[] fileByte = File.ReadAllBytes(url);

            if (fileByte == null)
                return string.Empty;

            byte[] hashByte = new MD5CryptoServiceProvider().ComputeHash(fileByte);

            return byteArrayToString(hashByte);
        }
        private static string byteArrayToString(byte[] arrInput)
        {
            StringBuilder sOutput = new StringBuilder(arrInput.Length);

            for (int i = 0; i < arrInput.Length; i++)
            {
                sOutput.Append(arrInput[i].ToString("X2"));
            }
            return sOutput.ToString();
        }
    }

}
