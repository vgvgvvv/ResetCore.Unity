using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;

namespace ResetCore.Util
{
    public class PropertyManager : Singleton<PropertyManager>
    {
        private Dictionary<string, Propertys> propertyDict = new Dictionary<string, Propertys>();

        /// <summary>
        /// 加载属性并且加入
        /// </summary>
        /// <param name="name"></param>
        /// <param name="xml"></param>
        public void LoadXmlProperty(string name, string xml)
        {
            if (propertyDict.ContainsKey(name))
            {
                Debug.unityLogger.LogError("Util", "不允许重复加载配置");
                return;
            }

            Propertys prop = Propertys.LoadXml(xml);
            if(prop != null)
            {
                propertyDict.Add(name, prop);
            }
            else
            {
                Debug.unityLogger.LogError("Util", "未能成功加载Property");
            }
        }

        /// <summary>
        /// 加载属性并且加入
        /// </summary>
        /// <param name="name"></param>
        /// <param name="json"></param>
        public void LoadJsonProperty(string name, string json)
        {
            if (propertyDict.ContainsKey(name))
            {
                Debug.unityLogger.LogError("Util", "不允许重复加载配置");
                return;
            }

            Propertys prop = Propertys.LoadJson(json);
            if (prop != null)
            {
                propertyDict.Add(name, prop);
            }
            else
            {
                Debug.unityLogger.LogError("Util", "未能成功加载Property");
            }
        }

        /// <summary>
        /// 网络下载配置
        /// </summary>
        /// <param name="name"></param>
        /// <param name="www"></param>
        public void LoadXmlPropertyByWWW(string name, string www)
        {
            string xml = DownloadManager.Instance.DownLoadText(www);
            LoadXmlProperty(name, xml);
        }

        public void LoadJsonPropertyByWWW(string name, string www)
        {
            string json = DownloadManager.Instance.DownLoadText(www);
            LoadJsonProperty(name, json);
        }

        /// <summary>
        /// 获取属性组
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Propertys GetPropertys(string name)
        {
            if (propertyDict.ContainsKey(name))
            {
                return propertyDict[name];
            }
            return null;
        }

        /// <summary>
        /// 获得属性
        /// </summary>
        /// <param name="name"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        public string GetProperty(string name, string propName)
        {
            return GetPropertys(name).GetProperty(propName);
        }
    }
}
