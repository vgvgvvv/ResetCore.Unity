using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using ResetCore.Util;

namespace ResetCore.IOC
{
    /// <summary>
    /// 作为Bean
    /// </summary>
    public class Bean : Attribute
    {
        public string name { get; private set; }
        public Bean(string name = null)
        {
            this.name = name;
        }
    }

    /// <summary>
    /// 导入的上下文
    /// </summary>
    public class Import : Attribute
    {
        public Type[] importContexts { get; private set; }
        public Import(params Type[] importContexts)
        {
            this.importContexts = importContexts;
        }
    }

    public class Context
    {
        /// <summary>
        /// 上下文类型
        /// </summary>
        public Type type { get; private set; }

        
        //所有成员函数
        private MethodInfo[] methods;

        //用于查询的函数表
        private Dictionary<string, Dictionary<int, MethodInfo>> methodDict = new Dictionary<string, Dictionary<int, MethodInfo>>();
        private Dictionary<string, Dictionary<int, object>> objectDict = new Dictionary<string, Dictionary<int, object>>();

        //通过名字查询的函数表
        private Dictionary<string, MethodInfo> methodDictWithName = new Dictionary<string, MethodInfo>();
        private Dictionary<string, object> objectDictWithName = new Dictionary<string, object>();

        private List<Context> importedContexts = new List<Context>();

        protected Context()
        {
            type = GetType();
            methods = type.GetMethods();
            ImportContexts();

            InitMethodDict();
            InitMethodDictWithName();
        }

        //导入上下文
        private void ImportContexts()
        {
            Import[] imports = type.GetCustomAttributes(typeof(Import), true) as Import[];
            for(int i = 0; i < imports.Length; i++)
            {
                var contextTypes = imports[i].importContexts;
                for(int j = 0; j < contextTypes.Length; j++)
                {
                    var contextType = typeof(Context<>).MakeGenericType(contextTypes[i]);
                    var context = contextType.GetProperty("context").GetValue(null, null) as Context;
                    importedContexts.Add(context);
                }
            }
        }

        //通过普通方式
        private void InitMethodDict()
        {
            for(int i = 0; i < methods.Length; i++)
            {
                if (methods[i].ReturnType == typeof(void))
                    return;

                var namedAttr = methods[i].GetFirstAttribute<Bean>(true);
                if (namedAttr == null)
                    return;

                int pararmNum = methods[i].GetParameters().Length;
                string returnTypeName = methods[i].ReturnType.Name;

                if (!methodDict.ContainsKey(returnTypeName))
                {
                    methodDict.Add(returnTypeName, new Dictionary<int, MethodInfo>());
                }
                methodDict[returnTypeName].Add(pararmNum, methods[i]);
            }
        }

        //通过名字加入表
        private void InitMethodDictWithName()
        {
            for (int i = 0; i < methods.Length; i++)
            {
                var namedAttr = methods[i].GetFirstAttribute<Bean>(true);
                if (namedAttr == null || string.IsNullOrEmpty(namedAttr.name))
                    return;
                methodDictWithName.Add(namedAttr.name, methods[i]);
            }
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public object GetNew(Type objType, params object[] args)
        {
            if(!methodDict.ContainsKey(objType.Name) || !methodDict[objType.Name].ContainsKey(args.Length))
            {
                for(int i = 0; i < importedContexts.Count; i++)
                {
                    var subRes = importedContexts[i].GetNew(objType, args);
                    if (subRes != null)
                        return subRes;
                }
                return null;
            }
            var res = methodDict[objType.Name][args.Length].Invoke(this, args);
            if (!objectDict.ContainsKey(objType.Name))
            {
                objectDict.Add(objType.Name, new Dictionary<int, object>());
            }
            objectDict[objType.Name][args.Length] = res;
            return res;
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public object GetNew(string name, params object[] args)
        {
            if (!methodDictWithName.ContainsKey(name))
            {
                for (int i = 0; i < importedContexts.Count; i++)
                {
                    var subRes = importedContexts[i].GetNew(name, args);
                    if (subRes != null)
                        return subRes;
                }
                return null;
            }
            var res = methodDictWithName[name].Invoke(this, args);
            if (!objectDictWithName.ContainsKey(name))
            {
                objectDictWithName[name] = res;
            }
            return res;
        }

        /// <summary>
        /// 获取单例对象
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public object GetSingleton(Type objType, params object[] args)
        {
            if(!objectDict.ContainsKey(objType.Name) || !objectDict[objType.Name].ContainsKey(args.Length))
            {
                return GetNew(objType, args);
            }
            return objectDict[objType.Name][args.Length];
        }

        /// <summary>
        /// 获取单例对象
        /// </summary>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public object GetSingleton(string name, params object[] args)
        {
            if (!objectDictWithName.ContainsKey(name))
                return GetNew(name, args);
            return objectDictWithName[name];
        } 

    }

    public class Context<T> : Context where T : Context<T>
    {
        //用于存放已经创建过的Context
        private Dictionary<string, Context<T>> contextDict = new Dictionary<string, Context<T>>();

        private static T _context;
        /// <summary>
        /// 获取上下文
        /// </summary>
        public static T context
        {
            get
            {
                if(_context == null)
                {
                    _context = Activator.CreateInstance<T>();
                }
                return _context;
            }
        }
        
    }
}

