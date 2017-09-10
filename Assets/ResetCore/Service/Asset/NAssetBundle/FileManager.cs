using ResetCore.Util;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ResetCore.NAsset
{
    public class FileManager
    {
        /// <summary>
        /// Resources下存在的文件
        /// </summary>
        private static List<string> _resourcesList;
        public static List<string> resourcesList
        {
            get
            {
                if(_resourcesList == null)
                {
                    var ta = Resources.Load<TextAsset>("ResourcesList");
                    if(ta == null)
                    {
                        Debug.LogError("未找到资源列表，请重新生成");
                    }
                    _resourcesList = ta.text.GetValue<List<string>>();
                }
                return _resourcesList;
            }
        }

        private static List<string> _streamingList;
        /// <summary>
        /// 流媒体中存在的文件
        /// </summary>
        public static List<string> streamingList
        {
            get
            {
                if (_streamingList == null)
                {
                    var ta = Resources.Load<TextAsset>("StreamingList");
                    if (ta == null)
                    {
                        Debug.LogError("未找到资源列表，请重新生成");
                    }
                    _streamingList = ta.text.GetValue<List<string>>();
                }
                return _streamingList;
            }
        }

        /// <summary>
        /// Persistent文件目录
        /// </summary>
        private static string persistentDataPath = null;

        /// <summary>
        /// Persistent文件路径
        /// </summary>
        /// <param name="name">文件名</param>
        /// <returns>路径</returns>
        public static string PersistentFilePath(string name)
        {
            if (persistentDataPath == null)
                persistentDataPath = Application.persistentDataPath;

            return persistentDataPath + "/" + name;
        }

        /// <summary>
        /// Persistent文件WWW路径
        /// </summary>
        /// <param name="name">文件名</param>
        /// <returns>路径</returns>
        public static string PersistentFileWWWPath(string name)
        {
            return "file:///" + PersistentFilePath(name);
        }

        /// <summary>
        /// Stream文件路径
        /// </summary>
        /// <param name="name">文件名</param>
        /// <returns>路径</returns>
        public static string StreamFilePath(string name)
        {
            return Application.streamingAssetsPath + "/" + name;
        }

        /// <summary>
        /// Stream文件WWW路径
        /// </summary>
        /// <param name="name">文件名</param>
        /// <returns>路径</returns>
        public static string StreamFileWWWPath(string name)
        {
            string path = StreamFilePath(name);

            if (!path.StartsWith("jar"))
                path = "file://" + path;

            return path;
        }

        /// <summary>
        /// Resource下是否存在文件
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsResourceFileExist(string name)
        {
            if (resourcesList == null || resourcesList.Count == 0)
                return false;
            return resourcesList.Contains(name);
        }

        /// <summary>
        /// 流媒体文件夹下是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsStreamFileExist(string name)
        {
            if (streamingList == null || streamingList.Count == 0)
                return false;
            return streamingList.Contains(name);
        }

        /// <summary>
        /// 沙盒文件是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsPersistentFileExist(string name)
        {
            //真实路径
            string path = PersistentFilePath(name);

            //判断文件是否存在
            bool exist = File.Exists(path);

            //返回结果
            return exist;
        }

        /// <summary>
        /// 清空Persistent的全部数据
        /// </summary>
        public static void EmptyPersistentData()
        {
            //基地址
            string basePath = Application.persistentDataPath;

            //删除所有文件
            string[] files = Directory.GetFiles(basePath, "*", SearchOption.AllDirectories);
            foreach (string file in files)
                File.Delete(file);

            //删除所有文件夹
            string[] dirs = Directory.GetDirectories(basePath, "*", SearchOption.AllDirectories);
            foreach (string dir in dirs)
                Directory.Delete(dir, true);
        }

        /// <summary>
        /// 清空PersistentData中的资源（只清空Stream中包含的资源）
        /// </summary>
        public static void ClearPersistentDataInStream()
        {
            //清空资源
            foreach (string file in streamingList)
                DeletePersistentFile(file);

        }

        /// <summary>
        /// 清空PersistentData中的资源（只清空Resources中包含的资源）
        /// </summary>
        public static void ClearPersistentDataInResources()
        {
            //清空资源
            foreach (string file in resourcesList)
                DeletePersistentFile(file);
        }

        /// <summary>
        /// 删除Persistent文件
        /// </summary>
        /// <param name="file">文件名</param>
        public static void DeletePersistentFile(string file)
        {
            if (File.Exists(Application.persistentDataPath + "/" + file))
                File.Delete(Application.persistentDataPath + "/" + file);
        }
    }
}

