using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ResetCore.ScriptObj
{
    public static class ScriptableObjectEx
    {
#if UNITY_EDITOR
        /// <summary>
        /// 将可序列化对象保存到路径（相对于Assets路径）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="path"></param>
        public static void Save<T>(this T obj, string path) where T : ScriptableObject
        {
            AssetDatabase.CreateAsset(obj, path);
        }
#endif
    }
}
