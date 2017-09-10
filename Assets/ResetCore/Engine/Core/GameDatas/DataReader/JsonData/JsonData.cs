using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;
using ResetCore.Util;
using ResetCore.Json;

namespace ResetCore.Data.GameDatas.Json
{
    public class JsonData
    {

        public int id
        {
            get;
            protected set;
        }
        public static readonly string nameSpace = "ResetCore.Data.GameDatas.Json";
        public static readonly string m_fileExtention = ".json";
        protected static Dictionary<int, T> GetDataMap<T>()
        {
            Type type = typeof(T);
            FieldInfo field = type.GetField("fileName");
            Dictionary<int, T> dictionary;
            if (field != null)
            {
                string fileName = field.GetValue(null) as string;
                dictionary = (new JsonDataController().FormatXMLData<T>(fileName));
            }
            else
            {
                dictionary = new Dictionary<int, T>();
            }
            return dictionary;
        }
    }

    public abstract class JsonData<T> : JsonData where T : JsonData<T>
    {
        private static Dictionary<int, T> m_dataMap;

        public static Dictionary<int, T> dataMap
        {
            get
            {
                if (JsonData<T>.m_dataMap == null)
                {
                    JsonData<T>.m_dataMap = JsonData.GetDataMap<T>();
                }
                return JsonData<T>.m_dataMap;
            }
            set
            {
                JsonData<T>.m_dataMap = value;
            }
        }

        /// <summary>
        /// 选择满足条件的
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static T Select(Func<T, bool> condition)
        {
            return JsonData<T>.dataMap.Values.FirstOrDefault((data) =>
            {
                return condition(data);
            });
        }
    }

    class JsonDataController
    {

        private static JsonDataController m_instance;

        public Dictionary<int, T> FormatXMLData<T>(string fileName)
        {
            //Dictionary<int, T> result = new Dictionary<int, T>();

            Dictionary<int, Dictionary<string, string>> strDictionary = new Dictionary<int, Dictionary<string, string>>();
            if(!JsonParser.LoadIntMap(fileName, out strDictionary)){
                Debug.unityLogger.LogError("GameData", "Load Failed");
                return new Dictionary<int, T>();
            }

            return DataUtil.ParserStringDict2ClassDict<T>(strDictionary);
        }

    }

}
