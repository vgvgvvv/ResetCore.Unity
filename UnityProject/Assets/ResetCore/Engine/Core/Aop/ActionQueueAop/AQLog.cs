using UnityEngine;
using System.Collections;

namespace ResetCore.AOP
{
    public static class AQLog
    {

        public static AQAopManager Log(this AQAopManager aqMgr, string bgMsg, string edMsg)
        {
            aqMgr.Work((act) =>
            {
                if (bgMsg != null)
                    Debug.unityLogger.Log(bgMsg);
                act();
                if (edMsg != null)
                    Debug.unityLogger.Log(edMsg);
            });
            return aqMgr;
        }

    }

}
