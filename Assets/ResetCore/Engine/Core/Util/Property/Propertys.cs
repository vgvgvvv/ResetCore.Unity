using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;
using LitJson;

namespace ResetCore.Util
{
    public class Propertys : MonoBehaviour
    {
        private Dictionary<string, string> propertyDict; 
        
        private Propertys() { }

        /// <summary>
        /// 加载Xml
        /// </summary>
        /// <param name="path"></param>
        public static Propertys LoadXml(string xml)
        {
            Propertys res = new Propertys();
            res.propertyDict = new Dictionary<string, string>();
            XDocument xDoc = XDocument.Parse(xml);
            foreach (XElement ele in xDoc.Root.Elements())
            {
                if (res.propertyDict.ContainsKey(ele.Name.LocalName))
                {
                    Debug.unityLogger.LogError("Util", "键值重复：" + ele.Name.LocalName);
                }
                res.propertyDict.Add(ele.Name.LocalName, ele.Value);
            }
            return res;
        }       

        /// <summary>
        /// 加载Json
        /// </summary>
        /// <param name="json"></param>
        public static Propertys LoadJson(string json)
        {
            Propertys res = new Propertys();
            res.propertyDict = new Dictionary<string, string>();
            JsonData data = new JsonData(json);
            foreach (string key in data.Keys)
            {
                if (res.propertyDict.ContainsKey(key))
                {
                    Debug.unityLogger.LogError("Util", "键值重复：" + key);
                }
                res.propertyDict.Add(key, data[key].ToString());
            }
            return res;
        }

        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetProperty(string name)
        {
            string res = "";
            if(!propertyDict.TryGetValue(name, out res))
            {
                Debug.unityLogger.LogError("Util", "不存在键值！：" + name);
            }
            return res;
        }

        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetProperty(string name, string value)
        {
            if (!propertyDict.ContainsKey(name))
            {
                Debug.unityLogger.LogError("Util", "不存在键值！：" + name);
            }
            else
            {
                propertyDict[name] = value;
            }
        }
    }
}
