using UnityEngine;
using System.Collections;
using System.Xml.Linq;
using ResetCore.Util;
using System.Collections.Generic;
using System;

namespace ResetCore.Xml
{
    public static class XElementEx
    {

        /// <summary>
        /// 获取当前节点下的子节点数量
        /// </summary>
        /// <param name="_el"></param>
        /// <returns></returns>
        public static int GetElementNum(this XElement _el)
        {
            if (!_el.HasElements) return 0;
            int num = 0;
            IEnumerator e = _el.Elements().GetEnumerator();
            while (e.MoveNext())
            {
                ++num;
            }
            return num;
        }


        /// <summary>
        /// 从节点中获取数据
        /// </summary>
        /// <param name="_el"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object ReadValueFromElement(this XElement _el, Type type)
        {
            object genericArgument;
            if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>)))
            {
                System.Type type2 = type.GetGenericArguments()[0];
                List<XElement> list = new List<XElement>(_el.Elements());
                object constructor = type.GetConstructor(System.Type.EmptyTypes).Invoke(null);
                foreach (XElement el in list)
                {
                    genericArgument = el.ReadValueFromElement(type2);
                    type.GetMethod("Add").Invoke(constructor, new object[] { genericArgument });
                }
                return constructor;
            }
            else if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Dictionary<,>)))
            {
                System.Type[] genericArguments = type.GetGenericArguments();
                Dictionary<string, XElement> dictionary = _el.GetElementDict();
                var constructor = type.GetConstructor(System.Type.EmptyTypes).Invoke(null);
                if (!genericArguments[0].Equals(typeof(string)))
                {
                    Debug.LogError("键值必须为字符串！");
                    return null;
                }
                foreach (KeyValuePair<string, XElement> pair in dictionary)
                {
                    genericArgument = pair.Value.ReadValueFromElement(genericArguments[1]);
                    type.GetMethod("Add").Invoke(constructor, new object[] { pair.Key, genericArgument });
                }
                return constructor;
            }
            else
            {
                return StringEx.GetValue(_el.Value, type);
            }
        }

        /// <summary>
        /// 从节点中读取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_el"></param>
        /// <param name="_defValue"></param>
        /// <returns></returns>
        public static T ReadValueFromElement<T>(this XElement _el, T _defValue = default(T))
        {
            Type type = typeof(T);
            return (T)ReadValueFromElement(_el, typeof(T));
        }

        /// <summary>
        /// 从节点中读取Dictionary
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_root"></param>
        /// <returns></returns>
        public static Dictionary<string, T> ReadDictionaryFromElement<T>(this XElement root)
        {
            Dictionary<string, T> _dictionary = new Dictionary<string, T>();
            foreach (XElement el in root.Elements())
            {
                if (_dictionary.ContainsKey(el.Name.ToString()))
                    Debug.LogError("同一元素在XML中重复定义");
                _dictionary.Add(el.Name.ToString(), el.ReadValueFromElement<T>());
            }
            return _dictionary;
        }

        private static Dictionary<string, XElement> GetElementDict(this XElement root)
        {
            Dictionary<string, XElement> _dictionary = new Dictionary<string, XElement>();
            foreach (XElement el in root.Elements())
            {
                if (_dictionary.ContainsKey(el.Name.ToString()))
                    Debug.LogError("同一元素在XML中重复定义");
                _dictionary.Add(el.Name.ToString(), el);
            }
            return _dictionary;
        }

        /// <summary>
        /// 从节点中读取List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="root"></param>
        /// <returns></returns>
        public static List<T> ReadListFromElement<T>(this XElement root)
        {
            List<T> list = new List<T>();
            foreach(XElement el in root.Elements())
            {
                list.Add(el.ReadValueFromElement<T>());
            }
            return list;
        }

        /// <summary>
        /// 找到所有名称匹配的节点
        /// </summary>
        /// <param name="element">父节点</param>
        /// <param name="nameList">匹配列表</param>
        /// <returns></returns>
        public static List<XElement> FindIncludeElement(this XElement element, List<string> nameList)
        {
            List<XElement> result = new List<XElement>();
            foreach(var child in element.Elements())
            {
                if (nameList.Contains(child.Name.LocalName))
                {
                    result.Add(child);
                }
            }
            return result;
        }


    }

}

