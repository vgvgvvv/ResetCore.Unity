using UnityEngine;
using System.Collections;
using UnityEditor;
using ResetCore.Asset;
using System.IO;
using System.Collections.Generic;
using ResetCore.Util;

namespace ResetCore.UGUI
{
    [CustomEditor(typeof(BaseUI), true)]
    public class UGUICustomEditor : Editor
    {
        UIScriptGener scriptGener = new UIScriptGener();

        public override void OnInspectorGUI()
        {
            if (target == null) return;

            if (!EditorApplication.isPlaying)
            {
                ShowInfo();
                CheckPrefab();
                ShowGenScript();
            }
            base.OnInspectorGUI();
        }

        private void ShowInfo()
        {
            BaseUI ui = target as BaseUI;

            if(ui is BaseNormalUI)
                GUILayout.Label("This is Normal UI", GUIHelper.MakeHeader(30));
            if(ui is BasePopupUI)
                GUILayout.Label("This is Popup UI", GUIHelper.MakeHeader(30));
            if (ui is BaseTopUI)
                GUILayout.Label("This is Top UI", GUIHelper.MakeHeader(30));

            EditorGUILayout.HelpBox("You can Edit UI Property Here\n" +
                "UI Path is \"" + UIConst.uiPrefabPath + "\" now",
                MessageType.Info);
        }

        private void CheckPrefab()
        {
            BaseUI ui = target as BaseUI;
            string uiPrefabPath = UIConst.uiPrefabPath;

            if (!UIConst.UIPrefabNameDic.ContainsKey(ui.uiName))
            {
                EditorGUILayout.HelpBox("There is no prefab name setted! Open the UIConst To Issue this problem.", MessageType.Error);
                if (GUILayout.Button("Open UIConst"))
                {
                    Object obj = EditorResources.GetAsset<Object>("UIConst", "ResetCore", "UGUI") as Object;
                    AssetDatabase.OpenAsset(obj);
                }
                return;
            }

            string prefabNameWithEx = UIConst.UIPrefabNameDic[ui.uiName];
            string prefabName = Path.GetFileNameWithoutExtension(prefabNameWithEx);
            //检查是否是Prefab
            if (PrefabUtility.GetPrefabType(ui.gameObject) != PrefabType.PrefabInstance 
                && PrefabUtility.GetPrefabType(ui.gameObject) != PrefabType.Prefab)
            {
                GameObject prefabAlreadyExist = EditorResources.GetAsset<GameObject>(prefabName, "Resources", uiPrefabPath) as GameObject;
                if (prefabAlreadyExist == null)
                {
                    EditorGUILayout.HelpBox("Lose prefab! You should Create a Prefab", MessageType.Warning);
                    if (GUILayout.Button("Create Prefab"))
                    {
                        string path = PathEx.Combine(PathConfig.assetResourcePath, uiPrefabPath, prefabNameWithEx).Replace("\\", "/");
                        string absolutePath = PathEx.Combine(PathConfig.resourcePath, uiPrefabPath, prefabNameWithEx);
                        PathEx.MakeFileDirectoryExist(absolutePath);
                        Debug.unityLogger.Log("Create a ui prefab : " + path);
                        GameObject go = PrefabUtility.CreatePrefab(path, ui.gameObject);
                        PrefabUtility.ConnectGameObjectToPrefab(ui.gameObject, go);
                        AssetDatabase.Refresh();
                    }
                }
                else
                {
                    
                    EditorGUILayout.HelpBox("There is already a same name prefab there!", MessageType.Error);
                    if(GUILayout.Button("Check Prefab"))
                    {
                        
                        Debug.unityLogger.Log(EditorResources.GetFullPath(prefabName, "Resources", uiPrefabPath));
                        //Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Prefabs/Cube.prefab");
                        EditorGUIUtility.PingObject(prefabAlreadyExist);
                        Selection.activeObject = prefabAlreadyExist;
                    }
                }
            }
            else
            {
                Object prefabObj;
                string prefabAssetName;
                if (PrefabUtility.GetPrefabType(ui) == PrefabType.Prefab)
                {
                    prefabObj = ui;
                    prefabAssetName = ui.gameObject.name;
                    Debug.Log(ui.gameObject.name);
                }
                else
                {
                    prefabObj = PrefabUtility.GetPrefabParent(ui.gameObject);
                    prefabAssetName = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(prefabObj.GetInstanceID()));
                }

                //检查Prefab名与UIName类型是否匹配
                if (prefabName != prefabAssetName)
                {
                    EditorGUILayout.HelpBox("Your prefab name is not fit your UITypeName, Please reselect!\n"+
                        "UIName = " + prefabName + " but \nPrefabName = " + prefabAssetName, MessageType.Error);
                    UIConst.UIName? fitUIName = null;
                    foreach (KeyValuePair<UIConst.UIName, string> kvp in UIConst.UIPrefabNameDic)
                    {
                        if (kvp.Value == prefabAssetName + ".prefab")
                        {
                            fitUIName = kvp.Key;
                        }
                    }

                    if (fitUIName != null)
                    {
                        if (GUILayout.Button("Fix Issue"))
                        {
                            ui.uiName = (UIConst.UIName)fitUIName;
                        }
                    }
                   
                }
            }
        }

        private void ShowGenScript()
        {
            if(GUILayout.Button("Create View Script", GUILayout.Height(30)))
            {
                scriptGener.GenViewScript((BaseUI)target);
            }
            if(GUILayout.Button("Create Model Script", GUILayout.Height(30)))
            {
                scriptGener.GenModelScript((BaseUI)target);
            }
            if(GUILayout.Button("Create Binder Script", GUILayout.Height(30)))
            {
                scriptGener.GenBindScript((BaseUI)target);
            }
        }
    }

}
