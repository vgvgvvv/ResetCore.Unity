#define UTIL
using UnityEditor;
using UnityEngine;

public class DefUTIL
{
    private static readonly string symbolName = "UTIL";
    [InitializeOnLoadMethod]
    public static void DefineAop()
    {
        
        var symbolsStr = PlayerSettings.GetScriptingDefineSymbolsForGroup
            (EditorUserBuildSettings.selectedBuildTargetGroup);
        if (symbolsStr.Length > 0)
        {
            var symbols = symbolsStr.Split(';');
            if (!symbols.Contains(symbolName))
            {
                symbolsStr = symbolsStr + ";" + symbolName;
            }
        }
        else
        {
            symbolsStr = symbolName;
        }

        PlayerSettings.SetScriptingDefineSymbolsForGroup
            (EditorUserBuildSettings.selectedBuildTargetGroup, symbolsStr);
    }
}