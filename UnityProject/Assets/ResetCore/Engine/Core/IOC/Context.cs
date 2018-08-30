using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace ResetCore.IOC
{
    public abstract class Context
    {
        /// <summary>
        /// 导入的上下文
        /// </summary>
        protected List<Context> importedContexts = new List<Context>();

        
        /// <summary>
        /// 获取新实例
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public abstract object GetNewInstance(Type objType, params object[] args);

        /// <summary>
        /// 获取新实例
        /// </summary>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public abstract object GetNewInstance(string name, params object[] args);

        /// <summary>
        /// 获取单例
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public abstract object GetSingleton(Type objType, params object[] args);

        /// <summary>
        /// 获取单例
        /// </summary>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public abstract object GetSingleton(string name, params object[] args);


        /// <summary>
        /// 导入上下文
        /// </summary>
        protected abstract void ImportContexts();
    }
}
