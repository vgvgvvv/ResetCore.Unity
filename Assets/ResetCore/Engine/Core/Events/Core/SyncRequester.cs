using UnityEngine;
using System.Collections;
using System;
using ResetCore.Util;

namespace ResetCore.Event
{
    //使用范例：
    //gameObject.AddEventListener<ArrayList>("Request", (array) =>
    //{
    //    CoroutineTaskManager.Instance.WaitSecondTodo(() =>
    //    {
    //        SyncRequester.Response("Response", new ArrayList() { array.Count.ToString() });
    //    }, 5);
    //});

    //SyncRequester.Request("Request", new ArrayList() { "TestData" }, "Response", (array) =>
    //{
    //    Debug.Log(array[0] as string);
    //}, 1000);


    public class SyncRequester
    {
        /// <summary>
        /// 请求异步事件
        /// </summary>
        /// <param name="requestType"></param>
        /// <param name="requestArgs"></param>
        /// <param name="responseType"></param>
        /// <param name="handler"></param>
        /// <param name="timeOut"></param>
        public static void Request(string requestType, ArrayList requestArgs, string responseType, Action<ArrayList> handler, float timeOut = 2)
        {
            string responseString = responseType + "Request";
            //处理返回动作
            Action<ArrayList> responseHandler = null;
            CoroutineTaskManager.CoroutineTask task = null;
            responseHandler = (array)=> {
                handler(array);
                EventDispatcher.RemoveEventListener<ArrayList>(responseString, responseHandler);
                task.Stop();
            };
            //监听返回行为
            EventDispatcher.AddEventListener<ArrayList>(responseString, responseHandler);
            //出发请求事件
            EventDispatcher.TriggerEvent<ArrayList>(requestType, requestArgs);
            //处理超时情况
            task = CoroutineTaskManager.Instance.WaitSecondTodo(() =>
            {
                Debug.unityLogger.LogError("Request", requestType + "request Timeout!");
                EventDispatcher.RemoveEventListener<ArrayList>(responseString, responseHandler);
            }, timeOut);
        }

        /// <summary>
        /// 回应异步事件
        /// </summary>
        /// <param name="responseType"></param>
        /// <param name="responseArgs"></param>
        public static void Response(string responseType, ArrayList responseArgs)
        {
            //返回所请求的消息
            EventDispatcher.TriggerEvent<ArrayList>(responseType + "Request", responseArgs);
        }
    }
}
