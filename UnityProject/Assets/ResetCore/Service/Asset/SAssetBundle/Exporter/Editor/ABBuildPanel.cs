using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ResetCore.SAsset
{
    public class ABBuildPanel : EditorWindow
    {
        private ABBuildSetting setting = new ABBuildSetting();

        [MenuItem("Tools/SAsset/Builder Panel")]
        static void Open()
        {
            GetWindow<ABBuildPanel>("ABSystem", true);
        }

        void OnGUI()
        {
            GUILayout.BeginHorizontal();
            setting.binFormat = (ABFormat)EditorGUILayout.EnumPopup(setting.binFormat);
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical();
            if (GUILayout.Button("Build Test"))
            {
                ABBuilder builder = new ABBuilder(setting.binFormat);
                builder.BuildAssetBundles(new ABFliter("Test", new List<ABFliter.FileFliter>()
                {
                    new ABFliter.FileFliter("Assets/Test/TestSAsset/Res/", "*.jpg", "*.prefab"),
                }));
                ABMerger.MergeBundles();
                showOK();
            }
            GUILayout.EndVertical();
        }

        private void showOK()
        {
            EditorUtility.DisplayDialog("AssetBundle Build Finish", "AssetBundle Build Finish!", "OK");
        }

    }
}
