using UnityEngine;
using System.Collections;
using UnityEditor;
using ResetCore.Data;
using System;
using ResetCore.Asset;
#if DATA_GENER
using ResetCore.Excel;
#endif
using System.IO;
using UnityEngine.UI;
using ResetCore.NAsset;

namespace ResetCore.UGUI
{
    [CustomEditor(typeof(UILocalization), false)]
    public class UILocalizationCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
#if DATA_GENER
            UILocalization local = target as UILocalization;
            //base.OnInspectorGUI();
            local.key = EditorGUILayout.TextField("Key", local.key);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Open Excel"))
            {
                EditorUtility.OpenWithDefaultApp(PathConfig.LanguageDataExcelPath);
            }

            if (GUILayout.Button("Open Const"))
            {
                UnityEngine.Object obj = EditorResources.GetAsset<UnityEngine.Object>("LanguageConst", "ResetCore", "Localization") as UnityEngine.Object;
                AssetDatabase.OpenAsset(obj);
            }
            EditorGUILayout.EndHorizontal();

            if (!File.Exists(PathConfig.LanguageDataPath))
            {
                EditorGUILayout.HelpBox("You have not exported the localization file, export please!", MessageType.Error);
            }

            if (GUILayout.Button("Export Localization"))
            {
                Source2Localization.ExportExcelFile();
            }

            if(local.GetComponent<Text>() != null)
            {
                if (LanguageManager.GetWord(local.key) != local.GetComponent<Text>().text)
                {
                    if (GUILayout.Button("Fix Text"))
                    {
                        Fix();
                    }
                }
            }
            

            Array types = Enum.GetValues(typeof(LanguageConst.LanguageType));
            if (local.gameObject.GetComponent<Text>() != null)
            {
                foreach (LanguageConst.LanguageType type in types)
                {
                    GUILayout.Label(type.ToString());
                    string helpTxt = LanguageManager.GetWord(local.key, type);
                    EditorGUILayout.HelpBox(helpTxt, MessageType.None);
                }
            }
            else if (local.gameObject.GetComponent<Image>() != null)
            {
                string defSp = LanguageManager.GetWord(local.key, LanguageConst.defaultLanguage);
                if (!string.IsNullOrEmpty(defSp))
                {
                    local.gameObject.GetComponent<Image>().sprite = AssetLoader.GetSpriteByR(defSp);
                }
                foreach (LanguageConst.LanguageType type in types)
                {
                    GUILayout.Label(type.ToString());
                    string helpTxt = LanguageManager.GetWord(local.key, type);
                    if (string.IsNullOrEmpty(helpTxt)) continue;
                    GUILayout.Label(helpTxt);
                    GUILayout.Label(AssetLoader.GetSpriteByR(helpTxt).texture, GUILayout.Width(50), GUILayout.Height(50));
                }
            }
#else
            EditorGUILayout.HelpBox("You need import \"DATA_GENER\" module to use this function", MessageType.Error);
#endif
        }

        private void Fix()
        {
#if DATA_GENER
            UILocalization local = target as UILocalization;
            Text txt = local.gameObject.GetComponent<Text>();
            Image img = local.gameObject.GetComponent<Image>();

            int id;
            if (string.IsNullOrEmpty(local.key) || int.TryParse(local.key.Substring(1), out id))
            {
                local.key = LanguageManager.GetAvalibleKey();
            }

            if (txt != null)
            {
                string key = local.key;
                
                if (LanguageManager.TryGetKey(txt.text, out key))
                {
                    local.key = key;
                    return;
                }
                else
                {
                    LanguageManager.Refresh();
                    if (!LanguageManager.ContainKey(local.key))
                    {
                        LanguageManager.AddNewWord(local.key, txt.text);
                        AssetDatabase.Refresh();
                    }
                    else
                    {
                        local.key = "_" + local.key;
                        Fix();
                    }
                    LanguageManager.Refresh();
                }
            }
            else
            {
                Debug.unityLogger.LogError("DataGener", "不存在Text");
            }
#endif
        }

    }

}
