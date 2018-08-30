using UnityEngine;
using System.Collections;
using System;
using ResetCore.Util;
using System.Collections.Generic;
using System.Reflection;
using ResetCore.Xml;

namespace ResetCore.Data.GameDatas.Xml
{
    public class PrefData
    {
        public static readonly string nameSpace = "ResetCore.Data.GameDatas.Xml";

        public static readonly string m_fileExtention = ".xml";

        protected static T GetInstance<T>()
        {
            Type type = typeof(T);
            FieldInfo field = type.GetField("fileName");
            T instance;
            if (field != null)
            {
                string fileName = field.GetValue(null) as string;
                instance = (T)(PrefDataController.Instance.FormatXMLData(fileName, type));
            }
            else
            {
                instance = default(T);
            }
            return instance;
        }
    }

    public abstract class PrefData<T> : PrefData where T : PrefData<T>
    {
        private static T m_Instance;
        public static T instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = PrefData.GetInstance<T>();
                }
                return m_Instance;
            }
            set
            {
                m_Instance = value;
            }
        }
    }

    public class PrefDataController : Singleton<PrefDataController>
    {

        public object FormatXMLData(string fileName, Type type)
        {
            object result;
            result = Activator.CreateInstance(type);
            try
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                if (!XMLParser.LoadInstance(fileName, out dictionary))
                {
                    //加载失败
                    Debug.unityLogger.LogError("GameData", "Load Failed！");
                    return result;
                }
                //Debug.logger.Log("dictionary.count" + dictionary.Count);
                PropertyInfo[] properties = type.GetProperties();
                //为Instance赋值
                foreach(PropertyInfo prop in properties)
                {
                    prop.SetValue(result, dictionary[prop.Name].GetValue(prop.PropertyType), null);
                }

            }
            catch (Exception exception)
            {
                Debug.unityLogger.LogException(exception);
            }
            
            return result;
        }

    }
}
