using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TestBaseWindow : BaseSubEditorWindow<TestBaseWindow> {

    protected override void DoWindow(int unusedWindow)
    {
        GUILayout.Button("Hi");
        GUI.DragWindow();
    }


}
