using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using ResetCore.Asset;
using System;
using ResetCore.ReAssembly;
using ResetCore.Util;
using ResetCore.Xml;

namespace ResetCore.IOC
{
    public class XmlContext : Context
    {
        //用于存放已经创建过的Context
        private static Dictionary<string, XmlContext> contextDict = new Dictionary<string, XmlContext>();

        private readonly Dictionary<Type, Type> typeTypeBindDict = new Dictionary<Type, Type>();
        private readonly Dictionary<string, Type> typeNameDict = new Dictionary<string, Type>();

        private readonly Dictionary<Type, object> reusePool = new Dictionary<Type, object>();

        private readonly string bundleName;
        private readonly string xmlName;
        private XDocument xDoc;


        /// <summary>
        /// 获取上下文
        /// </summary>
        /// <param name="xmlPath"></param>
        /// <param name="loader"></param>
        /// <returns></returns>
        public static XmlContext GetContext(string xmlPath)
        {
            var strs = xmlPath.Split('.');
            return GetContext(strs[0], strs[1]);
        }

        /// <summary>
        /// 获取上下文
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static XmlContext GetContext(string bundleName, string xmlName)
        {
            IBundleLoader bundleLoader = ApplicationContext.context.GetSingleton(typeof(IBundleLoader)) as IBundleLoader;

            XmlContext context;
            if (!contextDict.TryGetValue(bundleName + xmlName, out context))
            {
                context = new XmlContext(bundleName, xmlName);
                context.Init(bundleLoader);
                contextDict.Add(bundleName + xmlName, context);
            }
            return context;
        }

        protected XmlContext(string bundleName, string xmlName)
        {
            this.bundleName = bundleName;
            this.xmlName = xmlName;
        }

        protected void Init(IBundleLoader bundleLoader)
        {
            bundleLoader.LoadBundleSync(bundleName);
            xDoc = XDocument.Parse(bundleLoader.GetText(bundleName, xmlName).text);

            if (xDoc.Root != null)
            {
                ImportContexts();
                DefineBeans();
            }

            bundleLoader.UnloadBundle(bundleName, false);
        }

        //导入上下文
        protected override void ImportContexts()
        {
            var importsEle = xDoc.Root.Element("imports");
            if(importsEle == null)
                return;
            
            var importXmlEles = importsEle.Elements("importxml");
            var xmlImportE = importXmlEles.GetEnumerator();
            while (xmlImportE.MoveNext())
            {
                var currentEle = xmlImportE.Current;
                var bundleName =  currentEle.GetAttribute("bundleName");
                var xmlName = currentEle.GetAttribute("xmlName");
                if(string.IsNullOrEmpty(bundleName) || string.IsNullOrEmpty(xmlName))
                    continue;

                var context = XmlContext.GetContext(bundleName, xmlName);
                if (context != null)
                {
                    typeNameDict.AddRange(context.typeNameDict);
                    typeTypeBindDict.AddRange(context.typeTypeBindDict);
                }
            }
            xmlImportE.Dispose();
        }

        protected void DefineBeans()
        {
            var beansEle = xDoc.Root.Element("beans");
            if(beansEle == null)
                return;

            var e = beansEle.Elements("bean").GetEnumerator();
            while (e.MoveNext())
            {
                var currentBeanEle = e.Current;
                if(currentBeanEle == null)
                    continue;

                var targetType = AssemblyManager.GetAssemblyType(currentBeanEle.Value);
                var orginTypeName = currentBeanEle.GetAttributeOrDefult("type", null);
                var bindName = currentBeanEle.GetAttributeOrDefult("name", null);
                Type orginType = orginTypeName == null ? null : AssemblyManager.GetAssemblyType(orginTypeName);
                if (orginType != null)
                {
                    typeTypeBindDict.Add(orginType, targetType);
                    if(bindName != null)
                        typeNameDict.Add(orginType.Name, targetType);
                }

                if (bindName != null)
                    typeNameDict.Add(bindName, targetType);
            }
            e.Dispose();
        }


        public override object GetNewInstance(Type objType, params object[] args)
        {
            Type targetType;
            if (typeTypeBindDict.TryGetValue(objType, out targetType))
            {
                return Activator.CreateInstance(targetType);
            }
            return null;
        }

        public override object GetNewInstance(string name, params object[] args)
        {
            Type targetType;
            if (typeNameDict.TryGetValue(name, out targetType))
            {
                return Activator.CreateInstance(targetType);
            }
            return null;
        }

        public override object GetSingleton(Type objType, params object[] args)
        {
            Type targetType;
            object res;
            if (typeTypeBindDict.TryGetValue(objType, out targetType))
            {
                if (!reusePool.TryGetValue(targetType, out res))
                {
                    res = Activator.CreateInstance(targetType);
                    reusePool.Add(targetType, res);
                }
                return res;
            }
            return null;
        }

        public override object GetSingleton(string name, params object[] args)
        {
            Type targetType;
            object res;
            if (typeNameDict.TryGetValue(name, out targetType))
            {
                if (!reusePool.TryGetValue(targetType, out res))
                {
                    res = Activator.CreateInstance(targetType);
                    reusePool.Add(targetType, res);
                }
                return res;
            }
            return null;
        }

    }

}
