using ResetCore.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResetCore.UGradle
{
    public class Project
    {
        /// <summary>
        /// 依赖的属性
        /// </summary>
        private DictionaryList<string, Property> propertys = new DictionaryList<string, Property>();

        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 任务列表
        /// </summary>
        public DictionaryList<string, Task> taskDict = new DictionaryList<string, Task>();

        public Project(string name, string description)
        {
            this.name = name;
            this.description = description;
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="task"></param>
        public void AddTask(Task task)
        {
            if(task.project != null && task.project != this)
            {
                Debug.unityLogger.LogError("添加任务错误", "该任务已经在别的工程当中了");
                return;
            }
            task.project = this;
            taskDict.Add(task.name, task);
        }

        /// <summary>
        /// 应用属性
        /// </summary>
        /// <param name="prop"></param>
        public void Apply(Property prop)
        {
            propertys.Add(prop.name, prop);
        }

        public void Start()
        {
            ReCoroutineManager.AddCoroutine(Run());
        }

        /// <summary>
        /// 执行
        /// </summary>
        private IEnumerator<float> Run()
        {
            var tasks = new ReCoroutine[taskDict.Count];
            for(int i = 0; i < taskDict.Count; i++)
            {
                //按顺序执行
                yield return ReCoroutine.Wait(taskDict.GetValueAt(i).GetCoroutine());
            }
        }
    }
}
