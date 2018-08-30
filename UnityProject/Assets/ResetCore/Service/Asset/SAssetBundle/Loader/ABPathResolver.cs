using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ResetCore.SAsset
{
    public class ABPathResolver : MonoBehaviour
    {

        /// <summary>
        /// AB 保存的路径
        /// </summary>
        public const string ANDROID_BUNDLE_SAVE_PATH = "AssetBundles/Android";
        public const string IOS_BUNDLE_SAVE_PATH = "AssetBundles/iOS";
        public const string DEFAULT_BUNDLE_SAVE_PATH = "AssetBundles/Standalone";

        public const string ANDROID_BUNDLE_STREAM_PATH = "Assets/StreamingAssets/Android/AssetBundles";
        public const string IOS_BUNDLE_STREAM_PATH = "Assets/StreamingAssets/iOS/AssetBundles";
        public const string DEFAULT_BUNDLE_STREAM_PATH = "Assets/StreamingAssets/Standalone/AssetBundles";

        /// <summary>
        /// AB 依赖信息文件名
        /// </summary>
        public const string DEPEND_FILE_NAME = "dep.all";

        /// <summary>
        /// 在编辑器模型下将 abName 转为 Assets/... 路径
        /// 这样就可以不用打包直接用了
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        public static string GetEditorModePath(string abName)
        {
            //将 Assets.AA.BB.prefab 转为 Assets/AA/BB.prefab
            abName = abName.Replace(".", "/");
            int last = abName.LastIndexOf("/");

            if (last == -1)
            {
                return abName;
            }
            return string.Format("{0}.{1}", abName.Substring(0, last), abName.Substring(last + 1));
        }

        /// <summary>
        /// 获取 AB 源文件路径（打包进安装包的）
        /// </summary>
        /// <param name="path"></param>
        /// <param name="forWWW"></param>
        /// <returns></returns>
        public static string GetBundleSourceFile(string path, bool forWWW = true)
        {
            string filePath = null;
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    if (forWWW)
                        filePath = string.Format("jar:file://{0}!/assets/Android/AssetBundles/{1}", Application.dataPath, path);
                    else
                        filePath = string.Format("{0}!assets/Android/AssetBundles/{1}", Application.dataPath, path);
                    break;
                case RuntimePlatform.IPhonePlayer:
                    if (forWWW)
                        filePath = string.Format("file://{0}/Raw/iOS/AssetBundles/{1}", Application.dataPath, path);
                    else
                        filePath = string.Format("{0}/Raw/iOS/AssetBundles/{1}", Application.dataPath, path);
                    break;
                default:
                    if (forWWW)
                        filePath = string.Format("file://{0}/StreamingAssets/Standalone/AssetBundles/{1}", Application.dataPath, path);
                    else
                        filePath = string.Format("{0}/StreamingAssets/Standalone/AssetBundles/{1}", Application.dataPath, path);
                    break;
            }
            return filePath;
        }


        /// <summary>
        /// 用于缓存AB的目录，要求可写
        /// </summary>
        public static string BundleCacheDir
        {
            get
            {
                string dir = string.Format("{0}/AssetBundles", Application.persistentDataPath);
                DirectoryInfo cacheDir = new DirectoryInfo(dir);
                if (!cacheDir.Exists)
                {
                    cacheDir.Create();
                }
                return cacheDir.FullName;
            }
        }
    }
}

