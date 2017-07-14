using UnityEngine;
using System.Collections;

namespace ResetCore.Util
{
    public class Alert
    {
        /// <summary>
        /// 如果为空则发出错误警告
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static bool AlertIfNull(object obj, string errorMessage)
        {
            if (obj == null)
            {
                Debug.unityLogger.Log("NullAlert", errorMessage);
                return false;
            }
            return true;
        }

    }
}
