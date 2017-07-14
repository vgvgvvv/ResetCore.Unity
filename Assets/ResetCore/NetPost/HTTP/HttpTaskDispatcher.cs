using UnityEngine;
using System.Collections;
using ResetCore.Util;
using System.Collections.Generic;
using System;
using LitJson;
using System.Text;

namespace ResetCore.NetPost
{
    /// <summary>
    /// 网络任务分发器，负责分发网络任务
    /// </summary>
    public class HttpTaskDispatcher
    {

        private static Dictionary<string, ActionQueue> taskTable = new Dictionary<string, ActionQueue>();


        /// <summary>
        /// 添加自定义任务
        /// </summary>
        /// <param name="task"></param>
        /// <param name="queueName"></param>
        public static void AddNetPostTask(HttpPostTask task, string url = ServerConst.HttpNetPostURL, string queueName = "Defualt")
        {
            Action<Action> postAct = (act) =>
            {
                task.Start(act, url);
            };
            GetQueue(queueName).AddAction(postAct);
        }

        /// <summary>
        /// 添加通用任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="taskParams"></param>
        /// <param name="finishCall"></param>
        /// <param name="progressCall"></param>
        /// <param name="queueName"></param>
        public static void AddNetPostTask(int taskId, Dictionary<string, object> taskParams = null, string url = ServerConst.HttpNetPostURL
            , Action<JsonData> finishCall = null, Action<float> progressCall = null, string queueName = "Defualt")
        {
            CommonNetTask task = new CommonNetTask(taskId, taskParams, finishCall, progressCall);
            AddNetPostTask(task, url, queueName);
        }

        /// <summary>
        /// 添加GET方法的Task
        /// </summary>
        /// <param name="url"></param>
        /// <param name="taskParams"></param>
        /// <param name="finishCall"></param>
        /// <param name="processCall"></param>
        /// <param name="queueName"></param>
        public static void AddNetGetTask(string url, Dictionary<string, object> taskParams = null
            , Action<WWW> finishCall = null, Action<float> processCall = null, string queueName = "Defualt")
        {
            StringBuilder urlBuilder = new StringBuilder(url);
            if(taskParams != null)
            {
                urlBuilder.Append("?");
                int current = 1;
                foreach(var kvp in taskParams)
                {
                    urlBuilder.Append(kvp.Key).Append("=").Append(kvp.Value.ConverToString());
                    if (current != taskParams.Count)
                        urlBuilder.Append("&");
                    current++;
                }
            }
            GetQueue(queueName).AddAction((act) =>
            {
                CoroutineTaskManager.Instance.AddTask(GetTask(urlBuilder.ToString(), finishCall, processCall), (bo)=> 
                {
                    act();
                });
            });
        }

        private static IEnumerator GetTask(string url, Action<WWW> finishCall = null, Action<float> processCall = null)
        {
            WWW www = new WWW(url);

            while (!www.isDone)
            {
                processCall(www.progress);
                yield return www;
            }

            finishCall(www);
        }


        private static ActionQueue GetQueue(string queueName)
        {
            if (!taskTable.ContainsKey(queueName))
            {
                taskTable.Add(queueName, new ActionQueue());
            }
            return taskTable[queueName];
        }
    }

}
