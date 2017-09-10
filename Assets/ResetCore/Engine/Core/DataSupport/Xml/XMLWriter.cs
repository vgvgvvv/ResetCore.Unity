using UnityEngine;
using System.Collections;
using System.Xml.Linq;
using System.IO;
using System.Collections.Generic;
using ResetCore.Util;

namespace ResetCore.Xml
{
    /// <summary>
    /// 用于将数据写入XML
    /// </summary>
    public static class XMLWriter
    {

        /// <summary>
        /// 向指定XML输入字符串内容
        /// </summary>
        /// <param name="uri">文件的位置</param>
        /// <param name="nodeNames">上层的结点名</param>
        /// <param name="Tag">要加入的子结点的名字</param>
        /// <param name="str">要加入的子节点的内容</param>
        public static void WriteValueToXML<T>(string uri, string[] nodeNames, T value)
        {
            XDocument _XDoc;
            if (!File.Exists(uri))
            {
                _XDoc = new XDocument();
                _XDoc.Save(uri);
            }
            else
            {
                _XDoc = XDocument.Load(uri);
            }

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
        /// 向指定XML输入数组内容
        /// </summary>
        /// <param name="uri">文件的位置</param>
        /// <param name="nodeNames">上层的结点名</param>
        /// <param name="Tag">要加入的子结点们的名字</param>
        /// <param name="arr">要加入的子节点们的内容</param>
        public static void WriteListToXML<T>(string uri, string[] nodeNames, List<T> arr, string tag = "item")
        {
            XDocument _XDoc = XDocument.Load(uri);
            if (_XDoc == null)
            {
                _XDoc.Save(uri);
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
            _Root.RemoveAll();
            foreach (T ob in arr)
            {
                _Root.Add(new XElement(tag, StringEx.ConverToString(ob)));
            }
            _XDoc.Save(uri);
        }

        /// <summary>
        /// 向指定XML输入Dictionary内容
        /// </summary>
        /// <param name="uri">文件的位置</param>
        /// <param name="nodeNames">上层的结点名</param>
        /// <param name="_dic">要写入XML的Dictionary结构体</param>
        public static void WriteDictionaryToXML<T>(string uri, string[] nodeNames, Dictionary<string, T> _dic)
        {
            XDocument _XDoc = XDocument.Load(uri);
            if (_XDoc == null)
            {
                _XDoc.Save(uri);
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
            _Root.RemoveAll();
            foreach (string _id in _dic.Keys)
            {
                _Root.Add(new XElement(_id, StringEx.ConverToString(_dic[_id])));
            }
            _XDoc.Save(uri);
        }

        /// <summary>
        /// 打开XDocument
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static XDocument Open(string uri)
        {
            return XDocument.Load(uri);
        }

        /// <summary>
        /// 将值加入XML
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xDoc"></param>
        /// <param name="nodeNames"></param>
        /// <param name="value"></param>
        public static XDocument WriteValue<T>(this XDocument xDoc, string[] nodeNames, T value)
        {
            if (xDoc.Root == null)
            {
                xDoc.Add(new XElement("Root"));
            }

            XElement _Root = xDoc.Root;
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
            return xDoc;
        }

        /// <summary>
        /// 加入列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="nodeNames"></param>
        /// <param name="arr"></param>
        /// <param name="tag"></param>
        public static XDocument WriteList<T>(this XDocument xDoc, string[] nodeNames, List<T> arr, string tag = "item")
        {

            XElement _Root = xDoc.Root;
            for (int i = 0; i < nodeNames.Length; i++)
            {
                if (_Root.Element(nodeNames[i]) == null)
                {
                    _Root.Add(new XElement(nodeNames[i]));
                }
                _Root = _Root.Element(nodeNames[i]);
            }
            _Root.RemoveAll();
            foreach (T ob in arr)
            {
                _Root.Add(new XElement(tag, StringEx.ConverToString(ob)));
            }
            return xDoc;
        }

        /// <summary>
        /// 加入词典
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xDoc"></param>
        /// <param name="nodeNames"></param>
        /// <param name="_dic"></param>
        /// <returns></returns>
        public static XDocument WriteDictionary<T>(this XDocument xDoc, string[] nodeNames, Dictionary<string, T> _dic)
        {
            XElement _Root = xDoc.Root;
            for (int i = 0; i < nodeNames.Length; i++)
            {
                if (_Root.Element(nodeNames[i]) == null)
                {
                    _Root.Add(new XElement(nodeNames[i]));
                }
                _Root = _Root.Element(nodeNames[i]);
            }
            _Root.RemoveAll();
            foreach (string _id in _dic.Keys)
            {
                _Root.Add(new XElement(_id, StringEx.ConverToString(_dic[_id])));
            }
            return xDoc;
        }

        /// <summary>
        /// 提交修改
        /// </summary>
        /// <param name="xDoc"></param>
        /// <param name="uri"></param>
        public static void Submit(this XDocument xDoc, string uri)
        {
            xDoc.Save(uri);
        }
    }
}


