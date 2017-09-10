using UnityEngine;
using UnityEditor;

public static class GUIHelper
{
    public static GUIStyle MakeHeader(float fontSize = 12)
    {
        GUIStyle headerStyle = new GUIStyle(GUI.skin.label);
        headerStyle.fontSize = 12;
        headerStyle.fontStyle = FontStyle.Bold;

        return headerStyle;
    }
}