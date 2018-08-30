using UnityEngine;
using System.Collections.Generic;
using System;

namespace ResetCore.Util
{
    /// <summary>
    /// 将非主线程的行为加入到主线程当中
    /// </summary>
    public class MonoActionPool : MonoSingleton<MonoActionPool>
    {

        private static List<Action> monoActionPool = new List<Action>();


        public override void Init()
        {
            base.Init();
            GameObject.DontDestroyOnLoad(gameObject);
            gameObject.hideFlags |= HideFlags.HideAndDontSave;
        }

        /// <summary>
        /// 向主线程加入行为
        /// </summary>
        /// <param name="act"></param>
        public static void AddAction(Action act)
        {
            MonoActionPool.Instance.Create();
            monoActionPool.Add(act);
        }

        /// <summary>
        /// 创建由单例基类自动完成
        /// </summary>
        private void Create() { }

        // Update is called once per frame
        void Update()
        {
            if(monoActionPool != null)
            {
                monoActionPool.ForEach((act) =>
                {
                    if(act != null)
                        act();
                });
            }
            monoActionPool.Clear();
        }
    }
}

