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
    /// 导入的代码上下文
    /// </summary>
    public class ImportCodeContext : Attribute
    {
        public Type[] importContexts { get; private set; }
        public ImportCodeContext(params Type[] importContexts)
        {
            this.importContexts = importContexts;
        }
    }

    /// <summary>
    /// 导入的Xml上下文
    /// </summary>
    public class ImportXmlContext : Attribute
    {
        public string[] importContexts { get; private set; }
        public ImportXmlContext(params string[] importContexts)
        {
            this.importContexts = importContexts;
        }
    }

    public class CodeContext : Context
    {
        /// <summary>
        /// 上下文类型
        /// </summary>
        public Type type { get; private set; }

        //用于查询的函数表
        protected Dictionary<string, Dictionary<int, MethodInfo>> methodDict = new Dictionary<string, Dictionary<int, MethodInfo>>();
        protected Dictionary<string, Dictionary<int, object>> objectDict = new Dictionary<string, Dictionary<int, object>>();

        //通过名字查询的函数表
        protected Dictionary<string, MethodInfo> methodDictWithName = new Dictionary<string, MethodInfo>();
        protected Dictionary<string, object> objectDictWithName = new Dictionary<string, object>();

        //所有成员函数
        private readonly MethodInfo[] methods;


        protected CodeContext()
        {
            type = GetType();
            methods = type.GetMethods();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        protected void Init()
        {
            ImportContexts();

            InitMethodDict();
            InitMethodDictWithName();
        }

        //导入上下文
        protected override void ImportContexts()
        {
            ImportCodeContext[] importsCodeContext = type.GetCustomAttributes(typeof(ImportCodeContext), true) as ImportCodeContext[];
            if (importsCodeContext != null)
            {
                for (int i = 0; i < importsCodeContext.Length; i++)
                {
                    var contextTypes = importsCodeContext[i].importContexts;
                    for (int j = 0; j < contextTypes.Length; j++)
                    {
                        var contextType = typeof(CodeContext<>).MakeGenericType(contextTypes[i]);
                        var context = contextType.GetProperty("context").GetValue(null, null) as CodeContext;
                        importedContexts.Add(context);
                    }
                }
            }
            
            ImportXmlContext[] importsXmlContext = type.GetCustomAttributes(typeof(ImportXmlContext), true) as ImportXmlContext[];
            if (importsXmlContext != null)
            {
                for (int i = 0; i < importsXmlContext.Length; i++)
                {
                    var contextPath= importsXmlContext[i].importContexts;
                    for (int j = 0; j < contextPath.Length; j++)
                    {
                        var context = XmlContext.GetContext(contextPath[i]);
                        importedContexts.Add(context);
                    }
                        
                }
            }
                
        }

        //通过普通方式
        protected void InitMethodDict()
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
        protected void InitMethodDictWithName()
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
        public override object GetNewInstance(Type objType, params object[] args)
        {
            if (!methodDict.ContainsKey(objType.Name) || !methodDict[objType.Name].ContainsKey(args.Length))
            {
                for (int i = 0; i < importedContexts.Count; i++)
                {
                    var subRes = importedContexts[i].GetNewInstance(objType, args);
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
        public override object GetNewInstance(string name, params object[] args)
        {
            if (!methodDictWithName.ContainsKey(name))
            {
                for (int i = 0; i < importedContexts.Count; i++)
                {
                    var subRes = importedContexts[i].GetNewInstance(name, args);
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
        public override object GetSingleton(Type objType, params object[] args)
        {
            if (!objectDict.ContainsKey(objType.Name) || !objectDict[objType.Name].ContainsKey(args.Length))
            {
                return GetNewInstance(objType, args);
            }
            return objectDict[objType.Name][args.Length];
        }

        /// <summary>
        /// 获取单例对象
        /// </summary>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public override object GetSingleton(string name, params object[] args)
        {
            if (!objectDictWithName.ContainsKey(name))
                return GetNewInstance(name, args);
            return objectDictWithName[name];
        }
    }

    public class CodeContext<T> : CodeContext where T : CodeContext<T>
    {

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
                    _context.Init();
                }
                return _context;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public static T InitContext()
        {
            return context;
        }
        
    }


}

