using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using ResetCore.Asset;
using System;

namespace ResetCore.IOC
{
    public class XmlContext : Context
    {
        //用于存放已经创建过的Context
        private static Dictionary<string, XmlContext> contextDict = new Dictionary<string, XmlContext>();

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

            ImportContexts();
            //TODO 初始化需要注入的对象

            bundleLoader.UnloadBundle(bundleName, false);
        }

        //TODO 导入上下文
        protected override void ImportContexts()
        {
            var impotrsEle = xDoc.Root.Element("imports");
            var importXmlEles = impotrsEle.Elements("importxml");
            var importCodeEles = impotrsEle.Elements("importcode");
        }

        public override object GetNewInstance(Type objType, params object[] args)
        {
            throw new NotImplementedException();
        }

        public override object GetNewInstance(string name, params object[] args)
        {
            throw new NotImplementedException();
        }

        public override object GetSingleton(Type objType, params object[] args)
        {
            throw new NotImplementedException();
        }

        public override object GetSingleton(string name, params object[] args)
        {
            throw new NotImplementedException();
        }
    }

}
