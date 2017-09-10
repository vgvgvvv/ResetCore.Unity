using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class TestEditorUI : BaseEditorWindow<TestEditorUI>
{
    [MenuItem("EditorUI/Show TestEditorUI")]
    public static void ShowWindow()
    {
        ShowMainWindow();
    }

    public List<TestBaseWindow> subWindowList = new List<TestBaseWindow>();

    public Vector2 scrollPos = Vector2.zero;


    public override void OnGUI()
    {
        base.OnGUI();
        if (GUILayout.Button("AddNewSubWindowList", GUILayout.Width(200), GUILayout.Height(20)))
        {
            var newSubwindow = BaseSubEditorWindow<TestBaseWindow>.Create(this, new Rect(100, 100, 200, 200));
            subWindowList.Add(newSubwindow);
        }

        // Set up a scroll view
        scrollPos = GUI.BeginScrollView(new Rect(0, 0, position.width, position.height), scrollPos, new Rect(0, 0, 1000, 1000));
        BeginWindows();
        foreach (var window in subWindowList)
        {
            window.Draw(this);
        }
        EndWindows();
        GUI.EndScrollView();

    }

    protected override void DrawMenu(GenericMenu menu)
    {
        base.DrawMenu(menu);
        menu.AddItem(new GUIContent("MenuItem1"), false, Callback, "item 1");
        menu.AddItem(new GUIContent("MenuItem2"), false, Callback, "item 2");
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("SubMenu/MenuItem3"), false, Callback, "item 3");
    }

    void Callback(object obj)
    {
        Debug.Log("asdasd");
    }

}
