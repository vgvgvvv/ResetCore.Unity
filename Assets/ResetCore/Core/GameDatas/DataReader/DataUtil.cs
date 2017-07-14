using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ResetCore.Util;
using System.Reflection;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ResetCore.Data
{
    public class DataUtil
    {
        /// <summary>
        /// 将Dictionary<int, Dictionary<string, string>>转成Dictionary<int, T>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static Dictionary<int, T> ParserStringDict2ClassDict<T>(Dictionary<int, Dictionary<string, string>> dictionary)
        {
            Dictionary<int, T> dataDic = new Dictionary<int, T>();
            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (KeyValuePair<int, Dictionary<string, string>> pair in dictionary)
            {
                T propInstance = Activator.CreateInstance<T>();
                PropertyInfo[] array = properties;
                for (int i = 0; i < array.Length; i++)
                {
                    PropertyInfo propInfo = array[i];
                    if (propInfo.Name == "id")
                    {
                        //Key值为序号
                        propInfo.SetValue(propInstance, pair.Key, null);
                    }
                    else if (pair.Value.ContainsKey(propInfo.Name))
                    {
                        object propValue = StringEx.GetValue(pair.Value[propInfo.Name], propInfo.PropertyType);
                        propInfo.SetValue(propInstance, propValue, null);
                        HandleProperty(propInstance, propInfo);
                    }
                    else
                    {
                        Debug.unityLogger.LogError("Add New Value", propInfo.Name + "Not in the Xml");
                    }
                }
                dataDic.Add(pair.Key, propInstance);
            }
            return dataDic;
        }

        /// <summary>
        /// 处理属性
        /// </summary>
        /// <param name="info"></param>
        public static void HandleProperty(object obj, PropertyInfo info)
        {
            var attrs = info.GetCustomAttributes(true);
            object[] args = new object[] { obj, info };
            foreach (var attr in attrs)
            {
                attr.GetType().GetMethod("HandleProperty").Invoke(null, args);
            }
        }
        /// <summary>
        /// 处理需要导出的Xml中的值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string HandleExportXmlValue(string value, List<string> attrs)
        {
            string res = value;
            foreach (var attr in attrs)
            {
                Type type;
                if (DataAttributes.attributes.TryGetValue(attr, out type))
                {
                    var method = type.GetMethod("HandleExportXmlValue");
                    if (method != null)
                    {
                        res = (string)method.Invoke(null, new object[] { res });
                    }
                }
            }
            return res;
        }
        /// <summary>
        /// 通用加载方式
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileEnd">后缀</param>
        /// <returns></returns>
        public static string LoadFile(string fileName, string rootPath = null)
        {
            TextAsset textAsset = null;
            Debug.Log(PathConfig.GetLocalGameDataResourcesPath(PathConfig.DataType.Pref) + fileName);
            if (rootPath == null)
            {
                textAsset =
                    Resources.Load<TextAsset>(PathConfig.GetLocalGameDataResourcesPath(PathConfig.DataType.Pref) + fileName);
            }
            else
            {
                textAsset =
                    Resources.Load<TextAsset>(PathEx.Combine(rootPath, fileName).Replace("\\", "/"));
            }
            if (textAsset == null)
            {
                Debug.unityLogger.LogError("XMLParser", fileName + " 文本加载失败");
            }
            return textAsset.text;
        }

        /// <summary>
        /// 加载ScriptableObject
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static object LoadScriptableObject(string fileName)
        {
            return Resources.Load("GameData/Obj/" + fileName);
        }
    }

}
