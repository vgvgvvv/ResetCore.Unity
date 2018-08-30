using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using EditorEx;

public class TestBaseWindow : EditorWindowSubWindow {

    protected override void DoWindow(int unusedWindow)
    {
        base.DoWindow(unusedWindow);
        GUILayout.Button("Hi");
        GUI.DragWindow();
    }


}
