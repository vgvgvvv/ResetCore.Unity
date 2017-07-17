using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResetCore.Util
{
    public interface ITask
    {

        /// <summary>
        /// 结果
        /// </summary>
        object Result { get; }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        object Process();
    }

    public abstract class Task<TOut> : ITask
    {
        /// <summary>
        /// 本次反应堆中已经完成的任务
        /// </summary>
        private static readonly List<ITask> FinishedTasks = new List<ITask>();
        /// <summary>
        /// 本次反应堆中正在执行的任务
        /// </summary>
        private static readonly List<ITask> RunningTasks = new List<ITask>();


        /// <summary>
        /// 结果
        /// </summary>
        public object Result { get; private set; }
        

        /// <summary>
        /// 任务所依赖的任务
        /// </summary>
        protected readonly BetterDictionary<string, ITask> DependenceDictionary = new BetterDictionary<string, ITask>();

        /// <summary>
        /// 处理任务
        /// </summary>
        /// <returns></returns>
        public object Process()
        {
            if (FinishedTasks.Contains(this))
                return Result;

            if (RunningTasks.Contains(this))
            {
                Debug.LogError("任务中存在循环依赖" + GetType().Name);
                return null;
            }
            RunningTasks.Add(this);

            var results = new BetterDictionary<string, object>();
            foreach (var keyValuePair in DependenceDictionary)
            {
                results.Add(keyValuePair.Key, keyValuePair.Value.Process());
            }

            var res = DoProcess(results);
            Result = res;
            RunningTasks.Remove(this);
            FinishedTasks.Add(this);

            return res;
        }

        /// <summary>
        /// 需要重写的过程函数
        /// </summary>
        /// <returns></returns>
        protected abstract object DoProcess(BetterDictionary<string, object> results);

        /// <summary>
        /// 添加依赖
        /// </summary>
        /// <param name="name"></param>
        /// <param name="task"></param>
        public void AddDependence(string name, ITask task)
        {
            DependenceDictionary.Add(name, task);
        }

        /// <summary>
        /// 移除依赖
        /// </summary>
        /// <param name="name"></param>
        /// <param name="task"></param>
        public void RemoveDependence(string name, ITask task)
        {
            DependenceDictionary.Remove(name);
        }

        /// <summary>
        /// 开始任务
        /// </summary>
        public void StartTask()
        {
            FinishedTasks.Clear();
            RunningTasks.Clear();

            Process();

        }

        
    }
}
