using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ResetCore.Util;
using UnityEditor;
using UnityEditor.Callbacks;

namespace ResetCore.ReAssembly
{
    public class CodeInjector
    {

        static CodeInjector()
        {
            UnityScripsCompiling.onScriptCompiled += InjectMothod;
        }

        #region 打包注入
        //是否已经生成
        private static bool hasGen = false;
        [PostProcessBuild(1000)]
        private static void OnPostprocessBuildPlayer(BuildTarget buildTarget, string buildPath)
        {
            hasGen = false;
        }

        [PostProcessScene]
        public static void InjectMothodOnPost()
        {
            if (hasGen == true) return;
            hasGen = true;

            InjectMothod();
        }
        #endregion

        #region 编辑器下注入
        
        public static void InjectMothod()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;
            CodeInjectorSetting setting = new CodeInjectorSetting();
            setting.RunInject();

        }

        #endregion
    }
}
