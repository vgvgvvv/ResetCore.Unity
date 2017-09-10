using UnityEngine;
using System.Collections;
using LitJson;
using ResetCore.Util;

namespace ResetCore.Json
{
    public static class JsonDataEx
    {

        /// <summary>
        /// 将JsonData保存到路径
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path"></param>
        public static void Save(this JsonData data, string path)
        {
            FileEx.SaveText(data.ToJson(), path);
        }
        /// <summary>
        /// 将JsonData转换成类型的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T ConvertTo<T>(this JsonData data)
        {
            return JsonMapper.ToObject<T>(data.ToJson());
        }

        /// <summary>
        /// 将对象转换为JsonData
        /// </summary>
        /// <param name="data"></param>
        public static JsonData ToJsonData(this object data)
        {
            string json = JsonMapper.ToJson(data);
            return JsonMapper.ToObject(json);
        }
    }
}
