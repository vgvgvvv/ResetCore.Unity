using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using LitJson;

namespace ResetCore.NetPost
{
    public class ExampleNetTask : HttpPostTask
    {

        public ExampleNetTask(Dictionary<string, object> taskParams, Action<JsonData> finishCall = null, Action<float> progressCall = null)
            : base(taskParams, finishCall, progressCall)
        {

        }

        public override int taskId
        {
            get { return (int)TaskId.TEST_TASK; }
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnProgress(float progress)
        {
            base.OnProgress(progress);
        }

        protected override void OnFinish(LitJson.JsonData backJsonData)
        {
            base.OnFinish(backJsonData);
            Debug.unityLogger.Log(backJsonData.ToString());
        }
    }
}

