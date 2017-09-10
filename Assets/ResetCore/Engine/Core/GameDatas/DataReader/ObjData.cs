using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using ResetCore.Data;
using ResetCore.Util;
using System.Linq;

namespace ResetCore.Data.GameDatas.Obj
{

    [Serializable]
    public class ObjData
    {
        public int id
        {
            get;
            protected set;
        }

        public static readonly string nameSpace = "ResetCore.Data.GameDatas.Obj";
        public static readonly string ex = ".asset";

        protected static Dictionary<int, T> GetDataMap<T>() where T : ObjData<T>
        {
            Type type = typeof(T);
            FieldInfo field = type.GetField("fileName");
            Dictionary<int, T> dictionary = new Dictionary<int, T>();
            if (field != null)
            {
                string fileName = field.GetValue(null) as string;
                dictionary = new ObjDataController().FormatObjData<T>(fileName);
            }
            else
            {
                return new Dictionary<int, T>();
            }
            return dictionary;
        }
    }
    [Serializable]
    public abstract class ObjData<T> : ObjData where T : ObjData<T>
    {
        private static Dictionary<int, T> m_dataMap;

        public static Dictionary<int, T> dataMap
        {
            get
            {
                if (ObjData<T>.m_dataMap == null)
                {
                    ObjData<T>.m_dataMap = ObjData.GetDataMap<T>();
                }
                return ObjData<T>.m_dataMap;
            }
            set
            {
                ObjData<T>.m_dataMap = value;
            }
        }


        public static T Select(Func<T, bool> condition)
        {
            return ObjData<T>.dataMap.Values.FirstOrDefault((data) =>
            {
                return condition(data);
            });
        }
    }

    public class ObjDataController
    {

        protected readonly string m_fileExtention = ".asset";

        public Dictionary<int, T> FormatObjData<T>(string fileName) where T : ObjData<T>
        {

            var obj = DataUtil.LoadScriptableObject(fileName);// Resources.Load("GameData/Obj/" + fileName);
            Type bundleType = Type.GetType(ObjData.nameSpace + "." + typeof(T).Name + "Bundle" + ",Assembly-CSharp");

            Dictionary<int, T> dict = new Dictionary<int, T>();
            List<T> dataArray = bundleType.GetField("dataArray").GetValue(obj) as List<T>;
            for(int i = 0; i < dataArray.Count; i++)
            {
                dict.Add(i + 1, dataArray[i]);
            }

            return dict;
        }

    }
}
