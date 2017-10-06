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
            setting._binFormat = (ABFormat)EditorGUILayout.EnumPopup(setting._binFormat);
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical();
            if (GUILayout.Button("Build Lua"))
            {
                //EncryLua.EncryLuaZip();
                //BuildAssetBundles(setting._binFormat, LuaFilter);
                //ABMerger.MergeBundles();
                showOK();
            }
            if (GUILayout.Button("Build Bytes"))
            {
                //BuildAssetBundles(_binFormat ? ABFormat.Bin : ABFormat.Text, BytesFilter);
                //ABMerger.MergeBundles();
                showOK();
            }
            if (GUILayout.Button("Build UI"))
            {
                //BuildAssetBundles(_binFormat ? ABFormat.Bin : ABFormat.Text, UiFilter);
                //ABMerger.MergeBundles();
                showOK();
            }
            if (GUILayout.Button("Build Models"))
            {
                //BuildAssetBundles(_binFormat ? ABFormat.Bin : ABFormat.Text, ModelFilter);
                //ABMerger.MergeBundles();
                showOK();
            }
            if (GUILayout.Button("Build Anims"))
            {
                //BuildAssetBundles(_binFormat ? ABFormat.Bin : ABFormat.Text, AnimFilter);
                //ABMerger.MergeBundles();
                showOK();
            }
            if (GUILayout.Button("Build Effects"))
            {
                //BuildAssetBundles(_binFormat ? ABFormat.Bin : ABFormat.Text, EffectFilter);
                //ABMerger.MergeBundles();
                showOK();
            }
            if (GUILayout.Button("Build Scenes"))
            {
                //BuildAssetBundles(_binFormat ? ABFormat.Bin : ABFormat.Text, SceneFilter);
                //ABMerger.MergeBundles();
                showOK();
            }

            GUILayout.Space(10);
            if (GUILayout.Button("Build All"))
            {
                //EncryLua.EncryLuaZip();
                //BuildAssetBundles(_binFormat ? ABFormat.Bin : ABFormat.Text, LuaFilter);
                //BuildAssetBundles(_binFormat ? ABFormat.Bin : ABFormat.Text, BytesFilter);
                //BuildAssetBundles(_binFormat ? ABFormat.Bin : ABFormat.Text, UiFilter);
                //BuildAssetBundles(_binFormat ? ABFormat.Bin : ABFormat.Text, ModelFilter);
                //BuildAssetBundles(_binFormat ? ABFormat.Bin : ABFormat.Text, AnimFilter);
                //BuildAssetBundles(_binFormat ? ABFormat.Bin : ABFormat.Text, EffectFilter);
                //BuildAssetBundles(_binFormat ? ABFormat.Bin : ABFormat.Text, SceneFilter);
                //ABMerger.MergeBundles();
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
