using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ResetCore.Util;

namespace ResetCore.CSTool
{
    public class CommandLineHelper
    {
        /// <summary>
        /// 获取命令行参数表，每个 -XX 后面只能跟一个参数
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetCommandArgs()
        {
            var commandDict = new Dictionary<string, string>();
            string[] args = System.Environment.GetCommandLineArgs();
            for(int i = 0; i < args.Length - 1; i+=2 )
            {
                if (args[i].StartsWith("-"))
                {
                    commandDict.Add(args[i].Substring(1), args[i + 1]);
                }
            }
            return commandDict;
        }

        /// <summary>
        /// 获取特定命令参数的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="argName"></param>
        /// <returns></returns>
        public static T GetCommandArg<T>(string argName)
        {
            return GetCommandArgs()[argName].GetValue<T>();
        }
    }
}

