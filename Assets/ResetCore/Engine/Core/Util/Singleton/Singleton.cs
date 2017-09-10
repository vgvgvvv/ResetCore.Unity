using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
using System.Diagnostics;

namespace ResetCore.Util
{
    public abstract class Singleton<T> where T : Singleton<T>, new()
    {

        private static T _Instance = null;
        public static T Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = typeof(T).GetConstructor(new Type[] { }).Invoke(new object[] { }) as T;
                    _Instance.Init();
                }
                return _Instance;
            }
        }

        public virtual void Init() { }

    }

}

