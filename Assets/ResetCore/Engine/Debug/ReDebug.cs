using UnityEngine;
using System.Collections;
using System;

namespace ResetCore.ReDebug
{
    public enum ReLogType
    {
        System = 0,
        Editor,
        GamePlay,

    }

    public static class ReDebug
    {
        /// <summary>
        /// 是否屏蔽Log
        /// </summary>
        public static bool logEnable { get; set; }

        /// <summary>
        /// Log
        /// </summary>
        /// <param name="relogType"></param>
        /// <param name="message"></param>
        public static void Log(ReLogType relogType, string tag, string message, UnityEngine.Object context = null)
        {
            if (!logEnable) return;
            if(context == null)
            {
#if UNITY_2017
                Debug.unityLogger.Log(tag, String.Format("[{0}] {1}", relogType.ToString(), message));
#else
                Debug.logger.Log(tag, String.Format("[{0}] {1}", relogType.ToString(), message));
#endif
            }
            else
            {
#if UNITY_2017
                Debug.unityLogger.Log(tag, String.Format("[{0}] {1}", relogType.ToString(), message), context);
#else
                Debug.logger.Log(tag, String.Format("[{0}] {1}", relogType.ToString(), message), context);
#endif
            }
        }

        /// <summary>
        /// LogWarning
        /// </summary>
        /// <param name="relogType"></param>
        /// <param name="tag"></param>
        /// <param name="message"></param>
        public static void LogWarning(ReLogType relogType, string tag, string message, UnityEngine.Object context = null)
        {
            if (!logEnable) return;
            if (context == null)
            {
#if UNITY_2017
                Debug.unityLogger.LogWarning(tag, String.Format("[{0}] {1}", relogType.ToString(), message));
#else
                Debug.logger.LogWarning(tag, String.Format("[{0}] {1}", relogType.ToString(), message));
#endif
            }
            else
            {
#if UNITY_2017
                Debug.unityLogger.LogWarning(tag, String.Format("[{0}] {1}", relogType.ToString(), message), context);
#else
                Debug.logger.LogWarning(tag, String.Format("[{0}] {1}", relogType.ToString(), message), context);
#endif
            }
        }

        /// <summary>
        /// LogError
        /// </summary>
        /// <param name="relogType"></param>
        /// <param name="tag"></param>
        /// <param name="message"></param>
        public static void LogError(ReLogType relogType, string tag, string message, UnityEngine.Object context = null)
        {
            if (!logEnable) return;
            if (context == null)
            {
#if UNITY_2017
                Debug.unityLogger.LogError(tag, String.Format("[{0}] {1}", relogType.ToString(), message));
#else
                Debug.logger.LogError(tag, String.Format("[{0}] {1}", relogType.ToString(), message));
#endif
            }
            else
            {
#if UNITY_2017
                Debug.unityLogger.LogError(tag, String.Format("[{0}] {1}", relogType.ToString(), message), context);
#else
                Debug.logger.LogError(tag, String.Format("[{0}] {1}", relogType.ToString(), message), context);
#endif
            }
        }

        /// <summary>
        /// LogException
        /// </summary>
        /// <param name="e"></param>
        public static void LogException(Exception e, UnityEngine.Object context = null)
        {
            if (!logEnable) return;
            if (context == null)
            {
#if UNITY_2017
                Debug.unityLogger.LogException(e);
#else
                Debug.logger.LogException(e);
#endif

            }
            else
            {
#if UNITY_2017
                Debug.unityLogger.LogException(e, context);
#else
                Debug.logger.LogException(e, context);
#endif
            }
        }
       
    }

}
