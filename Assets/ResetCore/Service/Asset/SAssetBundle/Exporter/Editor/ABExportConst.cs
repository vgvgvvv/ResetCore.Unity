using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace ResetCore.SAsset
{
    public class ABExportConst
    {
        public static DirectoryInfo AssetDir = new DirectoryInfo(Application.dataPath);
        public static string AssetPath = AssetDir.FullName;
        public static DirectoryInfo ProjectDir = AssetDir.Parent;
        public static string ProjectPath = ProjectDir.FullName;

        /// <summary>
        /// 不同平台的导出路径
        /// </summary>
        public static string PlatformBundleSavePath
        {
            get
            {
                string result;
                switch (EditorUserBuildSettings.activeBuildTarget)
                {
                    case UnityEditor.BuildTarget.Android:
                        result = ABPathResolver.ANDROID_BUNDLE_STREAM_PATH;
                        break;
                    case UnityEditor.BuildTarget.iOS:
                        result = ABPathResolver.IOS_BUNDLE_STREAM_PATH;
                        break;
                    default:
                        result = ABPathResolver.DEFAULT_BUNDLE_STREAM_PATH;
                        break;
                }
                return result;
            }
        }

    }
}

