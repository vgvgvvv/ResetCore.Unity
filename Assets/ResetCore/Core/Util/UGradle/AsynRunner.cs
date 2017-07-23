using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResetCore.Util
{
    public class AsynRunner
    {

        /// <summary>
        /// 是否已经完成
        /// </summary>
        public bool isDone { get; private set; }

        /// <summary>
        /// 迭代器
        /// </summary>
        public IEnumerator e { get; private set; }

        private AsynRunner() { }

        public static AsynRunner Create(IEnumerator e)
        {
            AsynRunner runner = new AsynRunner();
            runner.isDone = false;
            runner.e = e;
            return runner;
        }

        public void Start()
        {
            
        }

        private IEnumerator Run()
        {

        }


    }
}

