using UnityEngine;
using System.Collections;

namespace ResetCore.PlatformHelper
{
    public class JavaManager
    {

        /// <summary>
        /// Java对象
        /// </summary>
        private static AndroidJavaObject _mainActivityObject = null;

        /// <summary>
        /// Java对象
        /// </summary>
        public static AndroidJavaObject mainActivityObject
        {
            get
            {
                if (_mainActivityObject == null)
                {
                    AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    _mainActivityObject = jc.GetStatic<AndroidJavaObject>("currentActivity");
                }

                return _mainActivityObject;
            }
        }


    }
}

