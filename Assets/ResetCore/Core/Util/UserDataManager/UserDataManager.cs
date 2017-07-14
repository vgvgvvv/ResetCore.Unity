using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResetCore.Util
{
    public class UserDataManager
    {
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        public static void SetData<T>(string name, string groupName, T obj)
        {
            string key = GetKey(name, groupName);
            PlayerPrefs.SetString(key, obj.ConverToString());
        }

        /// <summary>
        /// 获取数据，如果找不到则使用默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T GetDataOrDef<T>(string name, string groupName, T defValue)
        {
            string key = GetKey(name, groupName);
            if (PlayerPrefs.HasKey(key))
            {
                return PlayerPrefs.GetString(key).GetValue<T>();
            }
            return defValue;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="groupName"></param>
        /// <param name="defValue"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetDataOrDef(string name, string groupName, object defValue, Type type)
        {
            string key = GetKey(name, groupName);
            if (PlayerPrefs.HasKey(key))
            {
                return PlayerPrefs.GetString(key).GetValue(type);
            }
            return defValue;
        }


        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static T GetData<T>(string name, string groupName)
        {
            string key = GetKey(name, groupName);
            return PlayerPrefs.GetString(key).GetValue<T>();
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="groupName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetData(string name, string groupName, Type type)
        {
            string key = GetKey(name, groupName);
            return PlayerPrefs.GetString(key).GetValue(type);
        }

        /// <summary>
        /// 是否有数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static bool HasData(string name, string groupName)
        {
            string key = GetKey(name, groupName);
            return PlayerPrefs.HasKey(key);
        }

        private static string GetKey(string name, string groupName)
        {
            return groupName + "|" + name;
        }

    }

}
