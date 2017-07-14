using UnityEngine;
using System.Collections;
using ResetCore.Util;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
using ResetCore.ModuleControl;
#endif

namespace ResetCore.PlatformHelper
{
    public static class PlatformConst
    {
        #region Common
        /// <summary>
        /// Plugin目录
        /// </summary>
        public static readonly string pluginPath = PathEx.Combine(Application.dataPath, "Plugins");
        #endregion

        #region Android
        /// <summary>
        /// 安卓文件目录
        /// </summary>
        public static readonly string androidPath = Path.Combine(pluginPath, "Android");

#if UNITY_EDITOR
        public static readonly string androidBasePluginPath =
            Path.Combine(ModuleConst.GetSymbolPath(MODULE_SYMBOL.PLATFORM_HELPER), "Android/Editor/BasePlugins/Android");
#endif
        /// <summary>
        /// 安卓Menifest目录
        /// </summary>
        public static readonly string androidMenifestPath = Path.Combine(androidPath, "AndroidManifest.xml");

        /// <summary>
        /// 安卓lib第三方库文件目录
        /// </summary>
        public static readonly string androidLibPath = Path.Combine(androidPath, "libs");

        /// <summary>
        /// 安卓资源文件目录
        /// </summary>
        public static readonly string androidResPath = Path.Combine(androidPath, "res");

        #endregion

    }
}
