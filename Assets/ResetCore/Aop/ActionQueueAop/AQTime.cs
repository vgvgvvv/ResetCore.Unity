using UnityEngine;
using System.Collections;
using System.Diagnostics;

namespace ResetCore.AOP
{
    public static class AQTime
    {

        public static AQAopManager ShowUseTime(this AQAopManager aqMgr)
        {
            aqMgr.Work((act) =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                act();
                sw.Stop();
                UnityEngine.Debug.unityLogger.Log("一共用时" + sw.ElapsedMilliseconds + "毫秒");

            });
            return aqMgr;
        }
    }

}
