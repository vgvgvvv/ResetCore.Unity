using UnityEngine;
using System.Collections;
using ResetCore.Util;
using System.Collections.Generic;


namespace ResetCore.Util
{
    public class ReCoroutineTaskManager : MonoSingleton<ReCoroutineTaskManager>
    {
        private static Dictionary<string, CoroutineTask> taskList = new Dictionary<string, CoroutineTask>();
        
        /// <summary>
        /// 协程任务类
        /// </summary>
        public class CoroutineTask
        {
            private static long taskId = 1;
            /// <summary>
            /// 内部迭代器
            /// </summary>
            public IEnumerator<float> iEnumer { get; private set; }

            /// <summary>
            /// 回调函数
            /// </summary>
            /// <param name="success"></param>
            public delegate void callBackDelegate(bool success);
            /// <summary>
            /// 回调函数
            /// </summary>
            public callBackDelegate callBack { get; set; }

            


            public string name
            {
                get;
                private set;
            }

            public object bindObject
            {
                get;
                private set;
            }

            public bool running
            {
                get;
                private set;
            }

            public bool isFinished
            {
                get;
                private set;
            }

            public bool paused
            {
                get;
                private set;
            }

            public CoroutineTask(
                IEnumerator<float> iEnumer, System.Action<bool> callBack = null,
                object bindObject = null, bool autoStart = true)
            {
                string taskName = iEnumer.GetHashCode().ToString();
                this.name = taskName;
                this.iEnumer = iEnumer;
                this.callBack += (comp) =>
                {
                    taskList.Remove(name);
                    if (callBack != null)
                        callBack(comp);
                };

                if (bindObject == null)
                {
                    this.bindObject = ReCoroutineTaskManager.Instance.gameObject;
                }
                else
                {
                    this.bindObject = bindObject;
                }

                running = false;
                paused = false;
                isFinished = false;

                if (autoStart == true)
                {
                    Start();
                }
                taskId += 1;
            }
            public CoroutineTask(
                string name, IEnumerator<float> iEnumer, System.Action<bool> callBack = null,
                object bindObject = null, bool autoStart = true)
                : this(iEnumer, callBack, bindObject, autoStart)
            {
                this.name = name;
            }

            public void Start()
            {
                running = true;
                isFinished = false;
                ReCoroutineManager.AddCoroutine(DoTask());
            }

            public void Pause()
            {
                paused = true;
            }

            public void Unpause()
            {
                paused = false;
            }

            public void Stop()
            {
                running = false;
                callBack(false);
            }

            private IEnumerator<float> DoTask()
            {
                IEnumerator<float> e = iEnumer;
                while (running)
                {
                    if (bindObject.Equals(null))
                    {
                        Debug.unityLogger.LogWarning("协程中断", "因为绑定物体被删除所以停止协程");
                        Stop();
                        yield break;
                    }

                    if (paused)
                    {
                        yield return 0;
                    }
                    else
                    {
                        if (e != null && e.MoveNext())
                        {
                            yield return e.Current;
                        }
                        else
                        {
                            running = false;
                            isFinished = true;
                            callBack(true);
                        }
                    }
                }
            }
        }

        public override void Init()
        {
            base.Init();
            taskList = new Dictionary<string, CoroutineTask>();
            GameObject.DontDestroyOnLoad(gameObject);
            gameObject.hideFlags |= HideFlags.HideAndDontSave;
        }


        /// <summary>
        /// 添加一个新任务
        /// </summary>
        /// <param name="taskName"></param>
        /// <param name="iEnumer"></param>
        /// <param name="callBack"></param>
        /// <param name="autoStart"></param>
        public CoroutineTask AddTask(string taskName, IEnumerator<float> iEnumer, System.Action<bool> callBack = null, object bindObject = null, bool autoStart = true)
        {
            if (taskList.ContainsKey(taskName))
            {
                //Debug.logger.LogError("添加新任务", "任务重名！" + taskName);
                Restart(taskName);
                return taskList[taskName];
            }
            else
            {
                CoroutineTask task = new CoroutineTask(taskName, iEnumer, callBack, bindObject, autoStart);
                taskList.Add(taskName, task);
                return task;
            }
        }

        /// <summary>
        /// 添加一个新任务
        /// </summary>
        /// <param name="taskName"></param>
        /// <param name="iEnumer"></param>
        /// <param name="callBack"></param>
        /// <param name="autoStart"></param>
        public CoroutineTask AddTask(IEnumerator<float> iEnumer, System.Action<bool> callBack = null, object bindObject = null, bool autoStart = true)
        {
            CoroutineTask task = new CoroutineTask(iEnumer, callBack, bindObject, autoStart);
            AddTask(task);
            return task;
        }

        /// <summary>
        /// 添加一个新任务
        /// </summary>
        /// <param name="task"></param>
        public CoroutineTask AddTask(CoroutineTask task)
        {
            if (taskList.ContainsKey(task.name))
            {
                //Debug.logger.LogError("添加新任务", "任务重名！" + task.name);
                Restart(task.name);
            }
            else
            {
                taskList.Add(task.name, task);
            }
            return task;
        }

        /// <summary>
        /// 开始一个任务
        /// </summary>
        /// <param name="taskName"></param>
        public void DoTask(string taskName)
        {
            if (!taskList.ContainsKey(taskName))
            {
                Debug.unityLogger.LogError("开始任务", "不存在该任务" + taskName);
                return;
            }
            taskList[taskName].Start();
        }

        /// <summary>
        /// 暂停协程
        /// </summary>
        /// <param name="taskName"></param>
        public void Pause(string taskName)
        {
            if (!taskList.ContainsKey(taskName))
            {
                Debug.unityLogger.LogError("暂停任务", "不存在该任务" + taskName);
                return;
            }
            taskList[taskName].Pause();

        }

        /// <summary>
        /// 取消暂停某个协程
        /// </summary>
        /// <param name="taskName"></param>
        public void Unpause(string taskName)
        {
            if (!taskList.ContainsKey(taskName))
            {
                Debug.unityLogger.LogError("重新开始任务", "不存在该任务" + taskName);
                return;
            }
            taskList[taskName].Unpause();
        }
        
        /// <summary>
        /// 停止特定协程
        /// </summary>
        /// <param name="taskName"></param>
        public void Stop(string taskName)
        {
            if (!taskList.ContainsKey(taskName))
            {
                Debug.unityLogger.LogError("停止任务", "不存在该任务" + taskName);
                return;
            }
            taskList[taskName].Stop();
        }

        public void Restart(string taskName)
        {
            if (!taskList.ContainsKey(taskName))
            {
                Debug.unityLogger.LogError("重新开始任务", "不存在该任务" + taskName);
                return;
            }
            CoroutineTask task = taskList[taskName];
            Stop(taskName);
            AddTask(task);
        }

        /// <summary>
        /// 停止所有协程
        /// </summary>
        public void StopAll()
        {
            List<CoroutineTask> tampList = new List<CoroutineTask>();
            foreach (CoroutineTask task in taskList.Values)
            {
                tampList.Add(task);
            }
            for (int i = 0; i < tampList.Count; i++)
            {
                tampList[i].Stop();
            }
        }

        /// <summary>
        /// 等待一段时间再执行时间
        /// </summary>
        /// <param name="callBack"></param>
        /// <param name="time"></param>
        public CoroutineTask WaitSecondTodo(System.Action callBack, float time, object bindObject = null)
        {
            System.Action<bool> callBack2 = (bo) =>
            {
                if(bo)
                    callBack();
            };
            CoroutineTask task = new CoroutineTask(
                DoWaitTodo(time),
                callBack2, bindObject, true);
            AddTask(task);
            return task;
        }

        public CoroutineTask WaitSecondTodo(System.Action<bool> callBack, float time, object bindObject = null)
        {
            CoroutineTask task = new CoroutineTask(
                DoWaitTodo(time),
                callBack, bindObject, true);
            AddTask(task);
            return task;
        }

        private IEnumerator<float> DoWaitTodo(float time)
        {
            yield return time;
        }

        /// <summary>
        /// 等到下一帧
        /// </summary>
        /// <param name="callBack"></param>
        /// <param name="bindObject"></param>
        /// <returns></returns>
        public CoroutineTask WaitFrameEnd(System.Action callBack, object bindObject = null)
        {
            System.Action<bool> callBack2 = (bo) =>
            {
                if (bo)
                    callBack();
            };
            CoroutineTask task = new CoroutineTask(
                DoWaitFrameEndTodo(),
                callBack2, bindObject, true);
            AddTask(task);
            return task;
        }

        private IEnumerator<float> DoWaitFrameEndTodo()
        {
            yield return Time.deltaTime;
        }

        /// <summary>
        /// 等待直到某个条件成立时
        /// </summary>
        /// <param name="callBack"></param>
        /// <param name="time"></param>
        public CoroutineTask WaitUntilTodo(System.Action callBack, System.Func<bool> predicates, object bindObject = null)
        {
            System.Action<bool> callBack2 = (bo) =>
            {
                if(bo)
                    callBack();
            };
            CoroutineTask task = new CoroutineTask(
                DoWaitUntil(predicates), callBack2, bindObject, true);
            AddTask(task);
            return task;
        }

        private IEnumerator<float> DoWaitUntil(System.Func<bool> predicates)
        {
            while(!predicates()){
                yield return 0;
            }
        }

        /// <summary>
        /// 当条件成立时等待
        /// </summary>
        /// <param name="callBack"></param>
        /// <param name="time"></param>
        public CoroutineTask WaitWhileTodo(System.Action callBack, System.Func<bool> predicates, object bindObject = null)
        {
            System.Action<bool> callBack2 = (bo) =>
            {
                if(bo)
                    callBack();
            };
            CoroutineTask task = new CoroutineTask(
                DoWaitWhile(predicates), callBack2, bindObject, true);
            AddTask(task);
            return task;
        }

        private IEnumerator<float> DoWaitWhile(System.Func<bool> predicates)
        {
            while (predicates())
            {
                yield return 0;
            }
        }

        /// <summary>
        /// 等待所有其他携程任务完成
        /// </summary>
        /// <param name="callBack"></param>
        /// <param name="tasks"></param>
        /// <param name="bindObject"></param>
        /// <returns></returns>
        public CoroutineTask WaitForAllCoroutine(System.Action callBack, CoroutineTask[] tasks, object bindObject = null)
        {
            System.Action<bool> callBack2 = (bo) =>
            {
                if (bo)
                    callBack();
            };
            CoroutineTask task = new CoroutineTask(
                DoWaitForAllCoroutine(tasks), callBack2, bindObject, true);
            AddTask(task);
            return task;
        }

        private IEnumerator<float> DoWaitForAllCoroutine(params CoroutineTask[] coroutines)
        {
            bool isAllFinished = true;
            while (true)
            {
                isAllFinished = true;
                for (int i = 0; i < coroutines.Length; i++)
                {
                    if (!coroutines[i].isFinished)
                    {
                        isAllFinished = false;
                        break;
                    }
                }
                if (isAllFinished == true)
                    break;
                yield return 0;
            }
        }

        /// <summary>
        /// 间隔时间进行多次动作
        /// </summary>
        /// <param name="callBack"></param>
        /// <param name="interval"></param>
        /// <param name="loopTime"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public CoroutineTask LoopTodoByTime(System.Action callBack, float interval
            , int loopTime, object bindObject = null, float startTime = 0)
        {

            CoroutineTask task = new CoroutineTask(
                DoLoopByTime(interval, loopTime, callBack, startTime), null, bindObject, true);
            AddTask(task);
            return task;
        }

        private IEnumerator<float> DoLoopByTime(float interval, int loopTime
            , System.Action callBack, float startTime)
        {
            yield return startTime;
            if(loopTime <= 0)
            {
                loopTime = int.MaxValue;
            }
            int loopNum = 0;
            while (loopNum < loopTime)
            {
                loopNum++;
                callBack();
                yield return interval;
            }
        }

        /// <summary>
        /// 每帧进行循环
        /// </summary>
        /// <param name="callBack"></param>
        /// <param name="loopTime"></param>
        /// <param name="bindObject"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public CoroutineTask LoopByEveryFrame(System.Action callBack, int loopTime = -1
            , object bindObject = null, float startTime = 0)
        {
            CoroutineTask task = new CoroutineTask(
                DoLoopByEveryFrame(loopTime, callBack, startTime), null, bindObject, true);
            AddTask(task);
            return task;
        }

        private IEnumerator<float> DoLoopByEveryFrame(int loopTime
           , System.Action callBack, float startTime)
        {
            yield return startTime;
            if (loopTime <= 0)
            {
                loopTime = int.MaxValue;
            }
            int loopNum = 0;
            while (loopNum < loopTime)
            {
                loopNum++;
                callBack();
                yield return Time.deltaTime;
            }
        }

        /// <summary>
        /// 当满足条件循环动作
        /// </summary>
        /// <param name="callBack"></param>
        /// <param name="predicates"></param>
        /// <param name="loopTime"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public CoroutineTask LoopTodoByWhile(System.Action callBack, float interval, System.Func<bool> predicates, object bindObject = null, float startTime = 0)
        {
            CoroutineTask task = new CoroutineTask(
                DoLoopByWhile(interval, predicates, callBack, startTime), null, bindObject, true);
            AddTask(task);
            return task;
        }

        private IEnumerator<float> DoLoopByWhile(float interval, System.Func<bool> predicates, System.Action callBack, float startTime)
        {
            yield return startTime;

            int loopNum = 0;
            while (predicates())
            {
                loopNum++;
                callBack();
                yield return interval;
            }
        }

    }

}
