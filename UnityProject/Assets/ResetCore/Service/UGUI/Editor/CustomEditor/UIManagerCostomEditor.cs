using UnityEngine;
using System.Collections;
using UnityEditor;
using ResetCore.Util;
using ResetCore.Asset;
using System.Text;

namespace ResetCore.UGUI
{
    [CustomEditor(typeof(UIManager), false)]
    public class UIManagerCostomEditor : Editor
    {
        UIManager uiManager;
        Canvas canvas;
        Transform normalRoot;
        Transform popupRoot;
        Transform topRoot;

        public override void OnInspectorGUI()
        {
            ShowFunction();
            EditorGUILayout.Space();
            ShowInfo();
        }

        private void ShowFunction()
        {
            uiManager = target as UIManager;
            canvas = uiManager.canvas;

            EditorGUILayout.HelpBox("This is UIManager you can Manage all the UIInfo here", MessageType.None);

            if (GUILayout.Button("Open UIConst", GUILayout.Height(50), GUILayout.MaxWidth(200)))
            {
                Object obj = EditorResources.GetAsset<Object>("UIConst", "ResetCore", "UGUI") as Object;
                AssetDatabase.OpenAsset(obj);
            }
            //EditorGUILayout.Space();
            //if (GUILayout.Button("PSD2UGUI", GUILayout.Height(50), GUILayout.MaxWidth(200)))
            //{
            //    //PSD2UGUI.PsdFile2UGUI();
            //}

            EditorGUILayout.Space();

            UIConst.uiPrefabPath = EditorGUILayout.TextField("UI Prefab Path", UIConst.uiPrefabPath, GUILayout.MaxWidth(300));
        }

        private void ShowInfo()
        {
            StringBuilder result = new StringBuilder();
            System.Array uiNames = System.Enum.GetValues(typeof(UIConst.UIName));
            result.Append("There is " + uiNames.Length + " kinds of UI\n");

            foreach (UIConst.UIName name in uiNames)
            {
                result.Append(name.ToString() + "\n");
            }
            EditorGUILayout.HelpBox(result.ToString(), MessageType.None);
        }
    }

}
