using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ResetCore.Event;

[InitializeOnLoad]
public class UnityScripsCompiling : AssetPostprocessor
{
    public delegate void onScriptCompiledFunc();
    public static onScriptCompiledFunc onScriptCompiled { get; set; }

    static UnityScripsCompiling()
    {
        EditorApplication.update += Update;
    }

    public static void OnPostprocessAllAssets(
    String[] importedAssets,
    String[] deletedAssets,
    String[] movedAssets,
    String[] movedFromAssetPaths)
    {
        List<string> importedKeys = new List<string>() { "Assets/ResetCore" };
        for (int i = 0; i < importedAssets.Length; i++)
        {
            for (int j = 0; j < importedKeys.Count; j++)
            {
                if (importedAssets[i].Contains(importedKeys[j]))
                {
                    PlayerPrefs.SetInt("ImportScripts", 1);
                    return;
                }
            }
        }
    }

    private static void Update()
    {
        bool importScripts = Convert.ToBoolean(PlayerPrefs.GetInt("ImportScripts", 1));
        if (importScripts && !EditorApplication.isCompiling)
        {
            OnUnityScripsCompilingCompleted();
            importScripts = false;
            PlayerPrefs.SetInt("ImportScripts", 0);
            EditorApplication.update -= Update;
        }
    }

    private static void OnUnityScripsCompilingCompleted()
    {
        Debug.Log("Unity Scrips Compiling completed.");
        if (onScriptCompiled != null)
            onScriptCompiled();
    }
}