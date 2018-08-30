using UnityEngine;
using System.Collections;
using ResetCore.Util;
using System.Collections.Generic;


namespace ResetCore.Util
{
    public class CoroutineTaskManager : MonoSingleton<CoroutineTaskManager>
    {
        private static Dictionary<string, CoroutineTask> taskList = new Dictionary<string, CoroutineTask>();
        
        /// <summary>
        /// 协程任务类
        /// </summary>
        public class CoroutineTask
        {
            private static long taskId = 1;
            private IEnumerator iEnumer;
            private System.Action<bool> callBack;


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

            public bool paused
            {
                get;
                private set;
            }

            public CoroutineTask(
                IEnumerator iEnumer, System.Action<bool> callBack = null,
                object bindObject = null, bool autoStart = true)
            {
                string taskName = iEnumer.GetHashCode().ToString();
                this.name = taskName;
                this.iEnumer = iEnumer;
                this.callBack = (comp) =>
                {
                    taskList.Remove(name);
                    if (callBack != null)
                        callBack(comp);
                };

                if (bindObject == null)
                {
                    this.bindObject = CoroutineTaskManager.Instance.gameObject;
                }
                else
                {
                    this.bindObject = bindObject;
                }

                running = false;
                paused = false;

                if (autoStart == true)
                {
                    Start();
                }
                taskId += 1;
            }
            public CoroutineTask(
                string name, IEnumerator iEnumer, System.Action<bool> callBack = null,
                object bindObject = null, bool autoStart = true)
                : this(iEnumer, callBack, bindObject, autoStart)
            {
                this.name = name;
            }

            public void Start()
            {
                running = true;
                CoroutineTaskManager.Instance.StartCoroutine(DoTask());
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

            private IEnumerator DoTask()
            {
                IEnumerator e = iEnumer;
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
                        yield return null;
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
        public void AddTask(string taskName, IEnumerator iEnumer, System.Action<bool> callBack = null, object bindObject = null, bool autoStart = true)
        {
            if (taskList.ContainsKey(taskName))
            {
                //Debug.logger.LogError("添加新任务", "任务重名！" + taskName);
                Restart(taskName);
            }
            else
            {
                taskList.Add(taskName, new CoroutineTask(taskName, iEnumer, callBack, bindObject, autoStart));
            }
            
        }

        /// <summary>
        /// 添加一个新任务
        /// </summary>
        /// <param name="taskName"></param>
        /// <param name="iEnumer"></param>
        /// <param name="callBack"></param>
        /// <param name="autoStart"></param>
        public CoroutineTask AddTask(IEnumerator iEnumer, System.Action<bool> callBack = null, object bindObject = null, bool autoStart = true)
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

        private IEnumerator DoWaitTodo(float time)
        {
            yield return new WaitForSeconds(time);
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

        private IEnumerator DoWaitFrameEndTodo()
        {
            yield return new WaitForEndOfFrame();
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

        private IEnumerator DoWaitUntil(System.Func<bool> predicates)
        {
            while(!predicates()){
                yield return null;
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

        private IEnumerator DoWaitWhile(System.Func<bool> predicates)
        {
            while (predicates())
            {
                yield return null;
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

        private IEnumerator DoLoopByTime(float interval, int loopTime
            , System.Action callBack, float startTime)
        {
            yield return new WaitForSeconds(startTime);
            if(loopTime <= 0)
            {
                loopTime = int.MaxValue;
            }
            int loopNum = 0;
            while (loopNum < loopTime)
            {
                loopNum++;
                callBack();
                yield return new WaitForSeconds(interval);
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

        private IEnumerator DoLoopByEveryFrame(int loopTime
           , System.Action callBack, float startTime)
        {
            yield return new WaitForSeconds(startTime);
            if (loopTime <= 0)
            {
                loopTime = int.MaxValue;
            }
            int loopNum = 0;
            while (loopNum < loopTime)
            {
                loopNum++;
                callBack();
                yield return new WaitForEndOfFrame();
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

        private IEnumerator DoLoopByWhile(float interval, System.Func<bool> predicates, System.Action callBack, float startTime)
        {
            yield return new WaitForSeconds(startTime);

            int loopNum = 0;
            while (predicates())
            {
                loopNum++;
                callBack();
                yield return new WaitForSeconds(interval);
            }
        }

    }

}
