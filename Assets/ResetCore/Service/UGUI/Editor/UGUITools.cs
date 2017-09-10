using UnityEngine;
using System.Collections;
using UnityEditor;
using ResetCore.Asset;

namespace ResetCore.Util
{
    public class UGUITools
    {
        [MenuItem("Tools/UGUI/Create/UIManager"), MenuItem("GameObject/UGUI/UIManager", false, -1)]
        public static void CreateUIManager()
        {
            Object obj = EditorResources.GetAsset<Object>("UIManager", "ResetCore", "Resources", "UGUI");
            GameObject go = GameObject.Instantiate(obj, Vector3.zero, Quaternion.identity) as GameObject;
            go.name = "UIManager";
        }


        [MenuItem("Tools/UGUI/Create/ObjectCanvas"), MenuItem("GameObject/UGUI/ObjectCanvas", false, -1)]
        public static void CreateObjectCanvas()
        {
            Object obj = EditorResources.GetAsset<Object>("ObjectCanvas", "ResetCore", "Resources", "UGUI");
            GameObject go = GameObject.Instantiate(obj) as GameObject;
            go.name = "ObjectCanvas";
            if(Selection.activeGameObject != null)
            {
                go.transform.SetParent(Selection.activeGameObject.transform);

            }
        }
    }
}

