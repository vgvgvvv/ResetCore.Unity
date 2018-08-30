using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


namespace ResetCore.Util
{
    public enum CoroutineType
    {
        Update,
        FixedUpdate,
        LateUpdate
    }

    public class ReCoroutine
    {
        /// <summary>
        /// 协程标识符Id
        /// </summary>
        public long id { get; private set; }

        /// <summary>
        /// 迭代类型
        /// </summary>
        public CoroutineType coroutineType { get; private set; }

        /// <summary>
        /// 迭代器
        /// </summary>
        public IEnumerator<float> e { get; private set; }

        /// <summary>
        /// 距离协程开始的当前时间
        /// </summary>
        public float currentTime { get; private set; }

        /// <summary>
        /// 等待到该时间并继续执行
        /// </summary>
        public float untilTime { get; private set; }

        /// <summary>
        /// 是否已经完成
        /// </summary>
        public bool isDone { get; private set; }

        /// <summary>
        /// 正在等待的别的迭代器
        /// </summary>
        private ReCoroutine waitingCoroutine { get; set; }

        /// <summary>
        /// 加锁对象
        /// </summary>
        private static object lockObject = new object();

        /// <summary>
        /// 是否正在等待
        /// </summary>
        /// <returns></returns>
        public bool IsWaiting()
        {
            if (untilTime > currentTime)
                return true;

            if (waitingCoroutine != null)
                return true;

            return false;
        }

        private static long currentId = 0;
        public ReCoroutine(IEnumerator<float> e, CoroutineType type)
        {
            this.e = e;
            this.coroutineType = type;
            this.currentTime = 0;
            this.untilTime = 0;
            this.isDone = false;
            this.id = currentId++;
        }

        /// <summary>
        /// 等待多少时间
        /// </summary>
        /// <param name="waitTime"></param>
        public void Wait(float waitTime)
        {
            if (waitTime == float.NaN) waitTime = 0;
            untilTime = currentTime + waitTime;
        }

        public void Update()
        {
            if (coroutineType == CoroutineType.Update)
            {
                CommonUpdate();
            }
        }

        public void LateUpdate()
        {
            if (coroutineType == CoroutineType.LateUpdate)
            {
                CommonUpdate();
            }
                
        }

        public void FixedUpdate()
        {
            if (coroutineType == CoroutineType.FixedUpdate)
            {
                CommonUpdate();
            }
                
        }

        private void CommonUpdate()
        {
            currentTime += ReCoroutineManager.GetDeltaTime(this);
            
            if (!IsWaiting())
            {
                if (!e.MoveNext())
                {
                    isDone = true;
                }
                Wait(e.Current);

                if(e.Current.Equals(float.NaN))
                {
                    waitingCoroutine = replaceCoroutine;
                }
            }
            else
            {
                if (waitingCoroutine != null && waitingCoroutine.isDone)
                {
                    waitingCoroutine = null;
                }
            }


        }


        /// <summary>
        /// 等待www返回
        /// </summary>
        /// <param name="www"></param>
        /// <returns></returns>
        public static float WaitWWW(WWW www)
        {
            lock (lockObject)
            {
                replaceCoroutine = ReCoroutineManager.AddCoroutine(GetReplaceCoroutine(() => www.isDone), CoroutineType.Update);
            }
            return float.NaN;
        }

        /// <summary>
        /// 等待异步操作
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        public static float WaitAsynOperation(AsyncOperation operation)
        {
            lock (lockObject)
            {
                replaceCoroutine = ReCoroutineManager.AddCoroutine(GetReplaceCoroutine(() => operation.isDone), CoroutineType.Update);
            }
            return float.NaN;
        }

        /// <summary>
        /// 等待线程操作
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        public static float WaitThreadOperation(Action act)
        {
            Thread thread = new Thread(new ThreadStart(act));
            thread.Start();
            lock (lockObject)
                replaceCoroutine = ReCoroutineManager.AddCoroutine(GetReplaceCoroutine(() => !thread.IsAlive), CoroutineType.Update);
            return float.NaN;
        }

        /// <summary>
        /// 等待其他协程
        /// </summary>
        /// <param name="coroutine"></param>
        /// <returns></returns>
        public static float Wait(ReCoroutine coroutine)
        {
            lock (lockObject)
                replaceCoroutine = coroutine;
            return float.NaN;
        }

        /// <summary>
        /// 等待其他协程
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static float Wait(IEnumerator<float> e)
        {
            lock (lockObject)
                replaceCoroutine = ReCoroutineManager.AddCoroutine(e);
            return float.NaN;
        }

        /// <summary>
        /// 等待所有其他携程
        /// </summary>
        /// <param name="coroutines"></param>
        /// <returns></returns>
        public static float WaitForAllCoroutines(params ReCoroutine[] coroutines)
        {
            lock (lockObject)
                replaceCoroutine = ReCoroutineManager.AddCoroutine(
                GetReplaceCoroutine(() => {

                    for(int i = 0; i < coroutines.Length; i++)
                    {
                        if (!coroutines[i].isDone)
                            return false;
                    }
                    return true;
                }), 
                CoroutineType.Update);
            return float.NaN;
        }

        /// <summary>
        /// 等待知道
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static float WaitUntil(Func<bool> condition)
        {
            lock (lockObject)
                replaceCoroutine = ReCoroutineManager.AddCoroutine(GetReplaceCoroutine(condition), CoroutineType.Update);
            return float.NaN;
        }


        /// <summary>
        /// 替代用的Coroutine
        /// </summary>
        public static ReCoroutine replaceCoroutine { get; set; }

        public static IEnumerator<float> GetReplaceCoroutine(Func<bool> func)
        {
            while (!func())
            {
                yield return 0;
            }
        }

    }

}
