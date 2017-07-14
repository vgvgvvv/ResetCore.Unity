using UnityEngine;
using System.Collections.Generic;
using System;
using System.Reflection;
using ResetCore.Util;
using System.Linq;
using ResetCore.Xml;

namespace ResetCore.Data.GameDatas.Xml
{
    public abstract class XmlData
    {
        public int id
        {
            get;
            protected set;
        }

        public static readonly string nameSpace = "ResetCore.Data.GameDatas.Xml";
        public static readonly string m_fileExtention = ".xml";
        protected static Dictionary<int, T> GetDataMap<T>()
        {
            Type type = typeof(T);
            FieldInfo field = type.GetField("fileName");
            Dictionary<int, T> dictionary;
            if (field != null)
            {
                string fileName = field.GetValue(null) as string;
                dictionary = new XmlDataController().FormatXMLData<T>(fileName);
            }
            else
            {
                dictionary = new Dictionary<int, T>();
            }
            return dictionary;
        }

    }

    public abstract class XmlData<T> : XmlData where T : XmlData<T>
    {
        private static Dictionary<int, T> m_dataMap;

        public static Dictionary<int, T> dataMap
        {
            get
            {
                if (XmlData<T>.m_dataMap == null)
                {
                    XmlData<T>.m_dataMap = XmlData.GetDataMap<T>();
                }
                return XmlData<T>.m_dataMap;
            }
            set
            {
                XmlData<T>.m_dataMap = value;
            }
        }

        /// <summary>
        /// 选择满足条件的
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static T Select(Func<T, bool> condition) 
        {
            return XmlData<T>.dataMap.Values.FirstOrDefault((data) =>
            {
                return condition(data);
            });
        }
    }

    class XmlDataController
    {

        private static XmlDataController m_instance;

        //
        public Dictionary<int, T> FormatXMLData<T>(string fileName)
        {
            Dictionary<int, Dictionary<string, string>> dictionary = new Dictionary<int, Dictionary<string, string>>();
            if (!XMLParser.LoadIntMap(fileName, out dictionary))
            {
                //加载失败
                Debug.unityLogger.LogError("GameData", "数据加载失败！");
                return new Dictionary<int, T>();
            }
            return DataUtil.ParserStringDict2ClassDict<T>(dictionary);
        }

    }
}
