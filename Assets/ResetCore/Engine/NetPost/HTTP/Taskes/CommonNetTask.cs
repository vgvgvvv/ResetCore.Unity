using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using LitJson;

namespace ResetCore.NetPost
{
    /// <summary>
    /// 通用任务，用于执行一次性的临时任务
    /// </summary>
    public class CommonNetTask : HttpPostTask {

        public CommonNetTask(int taskId, Dictionary<string, object> taskParams
            , Action<JsonData> finishCall = null, Action<float> progressCall = null)
            : base(taskParams, finishCall, progressCall)
        {
            _taskId = taskId;
        }

        private int _taskId;
        public override int taskId
        {
            get { return _taskId; }
        }

    }

}
