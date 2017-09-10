using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;

using ResetCore.Asset;
using ResetCore.Util;
using System.IO;
using ResetCore.Data;

namespace ResetCore.Xml
{
    public class XMLParser
    {
        //创建表
        public static bool LoadIntMap(string fileName, out Dictionary<int, Dictionary<string, string>> dicFromXml, string rootPath = null)
        {

            XDocument xDoc = XDocument.Parse(DataUtil.LoadFile(fileName, rootPath));
            XElement root = xDoc.Root;
            dicFromXml = new Dictionary<int, Dictionary<string, string>>();
            if (xDoc == null) return false;
            int id = 1;
            //Debug.Log("Elements.Count" + root.Elements());
            foreach (XElement item in root.Elements())
            {
                Dictionary<string, string> propDic = new Dictionary<string, string>();
                foreach (XElement propItem in item.Elements())
                {
                    string key = propItem.Name.LocalName;
                    if (!propDic.ContainsKey(key))
                    {
                        propDic.Add(key, propItem.Value);
                    }
                    else
                    {
                        Debug.unityLogger.LogError("XMLPraser", "已经拥有相同的键值" + key);
                    }

                }
                dicFromXml.Add(id, propDic);
                id++;
            }
            return true;
        }
        
        //创建Instance
        public static bool LoadInstance(string fileName, out Dictionary<string, string> dicFromXml, string rootPath = null)
        {
            XDocument xDoc = XDocument.Parse(DataUtil.LoadFile(fileName, rootPath));
            XElement root = xDoc.Root;
            dicFromXml = new Dictionary<string, string>();

            if (xDoc == null)
            {
                Debug.LogError("Cant Prase your xml!");  
                return false;
            }

            foreach (XElement item in root.Elements())
            {
                string key = item.Name.LocalName;
                if (!dicFromXml.ContainsKey(key))
                {
                    dicFromXml.Add(key, item.Value);
                }
                else
                {
                    Debug.unityLogger.LogError("XMLPraser", "已经拥有相同的键值" + key);
                }
            }
            return true;
        }

    }
}

