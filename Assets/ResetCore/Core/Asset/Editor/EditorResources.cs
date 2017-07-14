using UnityEngine;
using System.Collections;
using UnityEditor;

namespace ResetCore.Asset
{
    public class EditorResources
    {
        /// <summary>
        /// 获取资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="fileNameFilter">文件名过滤器</param>
        /// <param name="pathFilter">路径过滤器</param>
        /// <returns></returns>
        public static T GetAsset<T>(string fileNameFilter, params string[] pathFilter) where T : UnityEngine.Object
        {
            string fullPath = GetFullPath(fileNameFilter, pathFilter);
            if (fullPath == null) return null;
            return AssetDatabase.LoadAssetAtPath(fullPath, typeof(T)) as T;
        }

        /// <summary>
        /// 是否包含该资源
        /// </summary>
        /// <param name="fileNameFilter"></param>
        /// <param name="pathFilter"></param>
        /// <returns></returns>
        public static bool Contains(string fileNameFilter, params string[] pathFilter)
        {
            return GetFullPath(fileNameFilter, pathFilter) != null;
        }

        /// <summary>
        /// 获取完整路径
        /// </summary>
        /// <param name="fileNameFilter">文件名过滤器</param>
        /// <param name="pathFilter">路径过滤器</param>
        /// <returns></returns>
        public static string GetFullPath(string fileNameFilter, params string[] pathFilter)
        {
            string[] guids = AssetDatabase.FindAssets(fileNameFilter);
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                int i = 0;
                for (; i < pathFilter.Length; i++)
                    if (!path.Contains(pathFilter[i])) break;
                if (i == pathFilter.Length)
                    return path;
            }
            return null;
        }

       
    }

}
