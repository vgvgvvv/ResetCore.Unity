using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ResetCore.Util
{
    public static class DictionaryEx
    {

        /// <summary>
        /// 根据权值获取值，Key为值，Value为权值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="powersDict"></param>
        /// <returns></returns>
        public static T GetRandomWithPower<T>(this Dictionary<T, int> powersDict)
        {
            List<T> keys = new List<T>();
            List<int> values = new List<int>();
            foreach (T key in powersDict.Keys)
            {
                keys.Add(key);
                values.Add(powersDict[key]);
            }
            int finalKeyIndex = values.GetRandomWithPower();
            return keys[finalKeyIndex];
        }


    }

}
