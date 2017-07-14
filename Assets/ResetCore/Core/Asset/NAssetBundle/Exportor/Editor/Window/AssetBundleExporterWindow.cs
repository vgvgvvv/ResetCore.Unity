using ResetCore.Util;
using ResetCore.Util.RichText;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ResetCore.NAsset
{
    public class AssetBundleExporterWindow : EditorWindow
    {

        private AssetBundleExporter abe = new AssetBundleExporter();

        //显示窗口的函数
        [MenuItem("Tools/AssetBundle/AssetBundle Gener")]
    static void ShowMainWindow()
        {
            AssetBundleExporterWindow window =
                EditorWindow.GetWindow(typeof(AssetBundleExporterWindow), false, "AssetBundle生成器") as AssetBundleExporterWindow;
            window.Show();
        }

        private void OnGUI()
        {
            ShowError();
            EditorGUILayout.Space();
            ShowTool();
            EditorGUILayout.Space();
            ShowBundles();
            EditorGUILayout.Space();
            ShowExport();
        }

        private void ShowError()
        {
            if(abe.error.Count != 0)
            {
                foreach(var error in abe.error)
                {
                    EditorGUILayout.LabelField(error.MakeColor(Color.red), GUILayout.Width(200));
                }
                if (GUILayout.Button("清除错误"))
                {
                    abe.error.Clear();
                }
            }
        }

        private void ShowTool()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(abe.outputPath);

            if (GUILayout.Button("重载导出路径", GUILayout.Width(200)))
            {
                abe.outputPath = EditorUtility.OpenFolderPanel("选择导出路径", abe.overrideOutputPath, abe.overrideOutputPath);
                if(string.IsNullOrEmpty(abe.outputPath))
                    abe.outputPath = NAssetPaths.defOutputBundlePath;
            }
            GUILayout.EndHorizontal();

            abe.currentTarget = (BuildTarget)EditorGUILayout.EnumPopup("导出平台", abe.currentTarget, GUILayout.Width(400));
            abe.options = (BuildAssetBundleOptions)EditorGUILayout.EnumMaskField("导出选项", abe.options, GUILayout.Width(400));
        }

        bool isShowBundle = false;
        bool showNameOnly = true;
        Dictionary<string, bool> showBundle;
        private void ShowBundles()
        {
            isShowBundle = EditorGUILayout.Foldout(isShowBundle, "展示所有Bundle");
            if (!isShowBundle)
                return;
            showNameOnly = EditorGUILayout.Toggle("只显示文件名", showNameOnly);
            if (GUILayout.Button("刷新资源列表", GUILayout.Width(200)))
            {
                abe.RefreshAssetBundleDict();
            }
            if (abe.assetBundleDict == null)
                return;
            if(showBundle == null)
                showBundle = new Dictionary<string, bool>();
            foreach (var key in abe.assetBundleDict.Keys)
            {
                if(!showBundle.ContainsKey(key))
                    showBundle.Add(key, false);
            }

            foreach (var kvp in abe.assetBundleDict)
            {
                showBundle[kvp.Key] = EditorGUILayout.Foldout(showBundle[kvp.Key], kvp.Key);
                if (showBundle[kvp.Key])
                {
                    EditorGUILayout.LabelField("内容列表（包含依赖）", GUIHelper.MakeHeader(10));
                    foreach (var info in kvp.Value)
                    {
                        string name = info;
                        if (showNameOnly)
                        {
                            name = FileEx.GetFileName(name);
                        }
                        EditorGUILayout.LabelField(name);
                    }
                }
            }
        }

        private void ShowExport()
        {
            if(GUILayout.Button("导出", GUILayout.Width(200)))
            {
                abe.BuildAssetBundle();
            }
            if (GUILayout.Button("更新资源列表", GUILayout.Width(200)))
            {
                abe.GenResourcesFolderList();
                abe.GenStreamingFolderList();
            }
        }
    }

}
