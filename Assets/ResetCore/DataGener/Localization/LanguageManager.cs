using UnityEngine;
using System.Collections;
using ResetCore.Util;
using System.Collections.Generic;
using System.IO;
using ResetCore.Xml;
using ResetCore.Event;
using System.Xml.Linq;

namespace ResetCore.Data
{
    public class LanguageManager : Singleton<LanguageManager>
    {

        public static LanguageConst.LanguageType currentLanguage = LanguageConst.defaultLanguage;
        private static Dictionary<int, Dictionary<string, string>> _allLanguageDict;
        private static Dictionary<int, Dictionary<string, string>> allLanguageDict
        {
            get
            {
                if(_allLanguageDict == null)
                {
                    _allLanguageDict = Instance.GetLanguageDict();
                }
                return _allLanguageDict;
            }
        }

        /// <summary>
        /// 获取单词
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetWord(string key)
        {
            return GetWord(key, currentLanguage);
        }

        public static string GetWord(string key, LanguageConst.LanguageType type)
        {
            
            if (!ContainKey(key, type))
            {
                return string.Empty;
            }

            return allLanguageDict[(int)type][key];
        }

        /// <summary>
        /// 是否存在该键值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool ContainKey(string key)
        {
            return ContainKey(key, currentLanguage);
        }
        public static bool ContainKey(string key, LanguageConst.LanguageType type)
        {

            if (!allLanguageDict[(int)type].ContainsKey(key))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 尝试获取单词的键值
        /// </summary>
        /// <param name="word"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool TryGetKey(string word, out string key, LanguageConst.LanguageType type = LanguageConst.defaultLanguage)
        {
            key = null;
            foreach (KeyValuePair<string, string> kvp in allLanguageDict[(int)type])
            {
                if(word.Trim().Equals(kvp.Value.Trim()))
                {
                    key = kvp.Key;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 添加新的字段
        /// </summary>
        /// <param name="key"></param>
        /// <param name="word"></param>
        public static void AddNewWord(string key, string word)
        {
            XDocument xDoc = XDocument.Load(PathConfig.LanguageDataPath);
            var lanEles = xDoc.Root.Elements();
            int i = 1;
            foreach(XElement lanEle in lanEles)
            {
                lanEle.Add(new XElement(key, word));
                i++;
            }
            xDoc.Save(PathConfig.LanguageDataPath);

        }

        /// <summary>
        /// 获取可用Key
        /// </summary>
        /// <returns></returns>
        public static string GetAvalibleKey()
        {
            int key = 1;
            string keyName;
            do
            {
                keyName = "_" + key;
                key++;
            } while (ContainKey(keyName));
            return keyName;
        }

        /// <summary>
        /// 刷新库
        /// </summary>
        public static void Refresh()
        {
            _allLanguageDict = Instance.GetLanguageDict();
        }

        /// <summary>
        /// 设置当前语言
        /// </summary>
        /// <param name="type"></param>
        public static void SetLanguageType(LanguageConst.LanguageType type)
        {
            currentLanguage = type;
            EventDispatcher.TriggerEvent(InnerEvents.UGUIEvents.OnLocalize);
        }

        private Dictionary<int, Dictionary<string, string>> GetLanguageDict()
        {
            var result = new Dictionary<int, Dictionary<string, string>>();
            if (!XMLParser.LoadIntMap(Path.GetFileNameWithoutExtension(PathConfig.LanguageDataPath), 
                out result, PathConfig.GetLocalGameDataResourcesPath(PathConfig.DataType.Localization)))
            {
                return result;
            }
            return result;
        }

    }

}
