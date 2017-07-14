using UnityEngine;
using System.Collections;
using System.Xml.Linq;
using ResetCore.Util;
using System.Collections.Generic;
using System.IO;

namespace ResetCore.Xml
{
    public static class XDocumentEx
    {

        public static XElement GetElement(this XDocument _XDoc, string[] _nodeNames)
        {
            XElement _Root = _XDoc.Root;
            for (int i = 0; i < _nodeNames.Length; i++)
            {

                if (_Root == null)
                {
                    Debug.Log(_nodeNames[i] + "结点不存在");
                    return null;
                }

                _Root = _Root.Element(_nodeNames[i]);
            }
            if (_Root == null)
            {
                Debug.Log(_nodeNames[_nodeNames.Length - 1] + "结点不存在");
                return null;
            }
            return _Root;
        }

        /// <summary>
        /// 从XML中读取数值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_XDoc"></param>
        /// <param name="nodeNames"></param>
        /// <param name="_defValue"></param>
        /// <returns></returns>
        public static T ReadValueFromXML<T>(this XDocument _XDoc, string[] nodeNames, T _defValue = default(T))
        {
            XElement _Root = _XDoc.Root;
            for (int i = 0; i < nodeNames.Length; i++)
            {

                if (_Root == null)
                {
                    return _defValue;
                }

                _Root = _Root.Element(nodeNames[i]);
            }
            if (_Root == null)
            {
                return _defValue;
            }
            return StringEx.GetValue<T>(_Root.Value);
        }

        /// <summary>
        /// 从XML中读取Dictionary
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="_XDoc"></param>
        /// <param name="nodeNames"></param>
        /// <returns></returns>
        public static Dictionary<string, T> ReadStringDictionaryFromXML<T>(this XDocument _XDoc, string[] nodeNames)
        {
            XElement _Root = _XDoc.Root;

            for (int i = 0; i < nodeNames.Length; i++)
            {

                if (_Root == null)
                {
                    //Debug.Log("结点不存在,返回空StringDictionary");
                    return new Dictionary<string, T>();
                }

                _Root = _Root.Element(nodeNames[i]);
            }

            Dictionary<string, T> _dictionary = new Dictionary<string, T>();
            foreach (XElement el in _Root.Elements())
            {
                if (_dictionary.ContainsKey(el.Name.ToString()))
                    Debug.Log("同一元素在XML中重复定义");
                _dictionary.Add(el.Name.ToString(), StringEx.GetValue<T>(el.Value));
            }
            return _dictionary;
        }

        /// <summary>
        /// 将一个值保存至某个Xml中
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="_XDoc">文档</param>
        /// <param name="uri">路径</param>
        /// <param name="nodeNames">文档中路径</param>
        /// <param name="value"></param>
        public static void WriteValueToXML<T>(this XDocument _XDoc, string uri, string[] nodeNames, T value)
        {

            if (_XDoc.Root == null)
            {
                _XDoc.Add(new XElement(Path.GetFileNameWithoutExtension(uri)));
            }

            XElement _Root = _XDoc.Root;
            for (int i = 0; i < nodeNames.Length; i++)
            {
                if (_Root.Element(nodeNames[i]) == null)
                {
                    _Root.Add(new XElement(nodeNames[i]));
                }
                _Root = _Root.Element(nodeNames[i]);
            }

            XElement newRoot = new XElement(_Root.Name);
            XElement parent = _Root.Parent;

            _Root.Remove();

            parent.Add(newRoot);
            newRoot.Value = StringEx.ConverToString(value);
            _XDoc.Save(uri);
        }

        /// <summary>
        /// 不会出现乱码的保存方式
        /// </summary>
        /// <param name="xDoc"></param>
        /// <param name="path"></param>
        public static void SafeSaveWithoutDeclaration(this XDocument xDoc, string path)
        {
            PathEx.MakeDirectoryExist(path);
            File.WriteAllText(path, xDoc.ToString());
        }

        /// <summary>
        /// 加载Xml如果不存在则创建
        /// </summary>
        /// <param name="path"></param>
        public static XDocument LoadOrCreate(string path)
        {
            XDocument xDoc;
             if (!File.Exists(path))
            {
                xDoc = new XDocument();
                xDoc.Add(new XElement("Root"));
                xDoc.SafeSaveWithoutDeclaration(path);
            }
            else
            {
                xDoc = XDocument.Load(path);
            }
            return xDoc;
        }

       
    }

}
