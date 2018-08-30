using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;
using ResetCore.Util;
using ResetCore.Asset;
using System.IO;
using System.Text;

namespace ResetCore.Data.GameDatas.Protobuf
{
    public class ProtobufData
    {
        public int id
        {
            get;
            protected set;
        }

        public static readonly string ex = ".bytes";
        public static readonly string nameSpace = "ResetCore.Data.GameDatas.Protobuf";
        protected static Dictionary<int, T> GetDataMap<T>()
        {
            Type type = typeof(T);
            FieldInfo field = type.GetField("fileName");
            Dictionary<int, T> dictionary = null;
            if (field != null)
            {
                string fileName = field.GetValue(null) as string;
                dictionary = (ProtobufDataController<T>.Instance.FormatData(fileName));
            }
            else
            {
                dictionary = new Dictionary<int, T>();
            }
            return dictionary;
        }
    }

    public abstract class ProtobufData<T> : ProtobufData where T : ProtobufData<T>
    {
        private static Dictionary<int, T> m_dataMap;

        public static Dictionary<int, T> dataMap
        {
            get
            {
                if (ProtobufData<T>.m_dataMap == null)
                {
                    ProtobufData<T>.m_dataMap = ProtobufData.GetDataMap<T>();
                }
                return ProtobufData<T>.m_dataMap;
            }
            set
            {
                ProtobufData<T>.m_dataMap = value;
            }
        }

        public static T Select(Func<T, bool> condition)
        {
            return ProtobufData<T>.dataMap.Values.FirstOrDefault((data) =>
            {
                return condition(data);
            });
        }
    }

    public class ProtobufDataController<T> : Singleton<ProtobufDataController<T>>
    {
        public Dictionary<int, T> FormatData(string fileName)
        {
            TextAsset asset = 
                Resources.Load<TextAsset>(PathConfig.GetLocalGameDataResourcesPath(PathConfig.DataType.Protobuf) + fileName);

            if(asset == null)
            {
                Debug.unityLogger.LogError("Load Assets", "Cant find the file:" + fileName);
                return null;
            }

            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(asset.text));

            Dictionary<int, T> resDict = new Dictionary<int, T>();
            //Type listType = typeof(List<T>);

            List<T> resList = ProtoBuf.Serializer.Deserialize<List<T>>(ms);
            int listCount = resList.Count;

            for (int i = 0; i < listCount; i++)
            {
                resDict.Add(i+1, resList[i]);
            }

            return resDict;
        }
    }
}
