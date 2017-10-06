using ResetCore.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResetCore.UGradle
{
    public enum TaskActionType
    {
        Sync,
        Asyn
    }

    public class Task
    {

        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 输入
        /// </summary>
        private Dictionary<string, object> input { get; set; }

        /// <summary>
        /// 输出
        /// </summary>
        private Dictionary<string, object> output { get; set; }

        /// <summary>
        /// 同步方法
        /// </summary>
        private List<Action> syncActionList { get; set; }

        /// <summary>
        /// 异步方法
        /// </summary>
        private List<IEnumerator<float>> asynActionList { get; set; }

        /// <summary>
        /// 该任务为同步或者异步
        /// </summary>
        public TaskActionType actionType { get; private set; }

        /// <summary>
        /// 所处的工程
        /// </summary>
        public WorkFlow workFlow { get; set; }

        /// <summary>
        /// 是否已经完成
        /// </summary>
        public bool isDone { get; private set; }

        /// <summary>
        /// 是否正在运行
        /// </summary>
        public bool isRunning { get; private set; }

        /// <summary>
        /// 依赖的任务
        /// </summary>
        public DictionaryList<string, Task> taskDict = new DictionaryList<string, Task>();

        public Task(string name, string description, TaskActionType actionType)
        {
            this.name = name;
            this.description = description;
            this.actionType = actionType;

            input = new Dictionary<string, object>();
            output = new Dictionary<string, object>();
            isDone = false;

            if (actionType == TaskActionType.Sync)
                syncActionList = new List<Action>();
            else
                asynActionList = new List<IEnumerator<float>>();

            workFlow = null;
        }

        /// <summary>
        /// 依赖任务
        /// </summary>
        /// <param name="task"></param>
        public Task DependsOn(Task task)
        {
            taskDict.Add(task.name, task);
            return this;
        }

        /// <summary>
        /// 首先做
        /// </summary>
        /// <param name="closure"></param>
        public Task AddSync(Action closure)
        {
            if (actionType == TaskActionType.Asyn)
            {
                asynActionList.Add(Sync2Aync(closure));
            }
            else
            {
                syncActionList.Add(closure);
            }
            return this;
        }

        private IEnumerator<float> Sync2Aync(Action closure)
        {
            closure();
            yield return 0;
        }

        /// <summary>
        /// 添加异步行为
        /// </summary>
        /// <param name="e"></param>
        public Task AddAync(IEnumerator<float> e)
        {
            if (actionType == TaskActionType.Sync)
            {
                Debug.unityLogger.LogError("添加异步行为", "同步任务中不允许加入异步行为");
                return this;
            }
            asynActionList.Add(e);
            return this;
        }


        /// <summary>
        /// 获取输入参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T FromInput<T>(string key)
        {
            return (T) (input[key]);
        }

        /// <summary>
        /// 获取输出参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T FromOutput<T>(string key)
        {
            return (T)(output[key]);
        }

        /// <summary>
        /// 添加输出参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="outObj"></param>
        public Task AddOutput(string name, object outObj)
        {
            output.Add(name, outObj);
            return this;
        }

        /// <summary>
        /// 获取执行携程
        /// </summary>
        /// <returns></returns>
        public ReCoroutine GetCoroutine()
        {
            if (isRunning)
            {
                return ReCoroutineManager.AddCoroutine(WaitForFinish());
            }
            return ReCoroutineManager.AddCoroutine(Run());
        }
        
        /// <summary>
        /// 等待该任务执行完成
        /// </summary>
        /// <returns></returns>
        private IEnumerator<float> WaitForFinish()
        {
            while (isRunning)
            {
                yield return 0;
            }
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        private IEnumerator<float> Run()
        {
            if(isDone)
                yield break;

            var tasks = new ReCoroutine[taskDict.Count];
            for (int i = 0; i < taskDict.Count; i++)
            {
                tasks[i] = taskDict.GetValueAt(i).GetCoroutine();
            }
            ///同时执行所有的任务
            yield return ReCoroutine.WaitForAllCoroutines(tasks);
            if(this.actionType == TaskActionType.Sync)
            {
                //同步按顺序执行
                for(int i = 0; i < syncActionList.Count; i++)
                {
                    syncActionList[i]();
                }
            }
            else
            {
                //异步按顺序执行
                for (int i = 0; i < asynActionList.Count; i++)
                {
                    yield return ReCoroutine.Wait(asynActionList[i]);
                }
            }

            isDone = true;
        }
    }
}
