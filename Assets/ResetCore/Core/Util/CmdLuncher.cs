using System;
using UnityEngine;
using System.Collections;
using System.Diagnostics;

namespace ResetCore.Util
{
    public class CmdLuncher
    {
        /// <summary>
        /// 运行可执行程序
        /// </summary>
        /// <param name="runableName">可执行程序路径</param>
        /// <param name="command">命令</param>
        public static Process LaunchExe(string runableName, string command, Action callback = null)
        {

            Process myProcess = new Process();

            UnityEngine.Debug.unityLogger.Log(runableName + " -- is Running -- " + command);

            ProcessStartInfo myProcessStartInfo = new ProcessStartInfo(runableName, command);

            myProcess.StartInfo = myProcessStartInfo;

            myProcess.Start();

            if (callback != null)
            {
                myProcess.WaitForExit();
                callback();
            }

            return myProcess;
        }
    }

}
