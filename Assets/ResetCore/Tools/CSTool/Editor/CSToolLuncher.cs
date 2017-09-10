using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class CSToolLuncher {

    /// <summary>
    /// 运行指定可执行文件
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="command"></param>
    public static void Launch(string fileName, string command)
    {
        Process myProcess = new Process();

        UnityEngine.Debug.unityLogger.Log(command);

        ProcessStartInfo myProcessStartInfo = new ProcessStartInfo(fileName, command);

        myProcess.StartInfo = myProcessStartInfo;

        myProcess.Start();
    }
}
