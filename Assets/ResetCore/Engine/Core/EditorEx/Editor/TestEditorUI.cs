using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using EditorEx;

public class TestEditorUI : BaseEditorWindow<TestEditorUI>
{
    [MenuItem("EditorUI/Show TestEditorUI")]
    public static void ShowWindow()
    {
		TestEditorUI.ShowMainWindow();
    }

    public List<TestBaseWindow> subWindowList = new List<TestBaseWindow>();

    public Vector2 scrollPos = Vector2.zero;


    //public override void OnGUI()
    //{
    //    base.OnGUI();
    //    if (GUILayout.Button("AddNewSubWindowList", GUILayout.Width(200), GUILayout.Height(20)))
    //    {
    //        var newSubwindow = BaseSubEditorWindow<TestBaseWindow>.Create(this, new Rect(100, 100, 200, 200));
    //        subWindowList.Add(newSubwindow);
    //    }

    //    // Set up a scroll view
    //    scrollPos = GUI.BeginScrollView(new Rect(0, 0, position.width, position.height), scrollPos, new Rect(0, 0, 1000, 1000));
    //    BeginWindows();
    //    foreach (var window in subWindowList)
    //    {
    //        window.Draw(this);
    //    }
    //    EndWindows();
    //    GUI.EndScrollView();

    //}

    protected override void OnInit()
    {
        mainPanel.mainTexure = EditorGUIUtility.whiteTexture;
        var menuProvider = new MenuProvider(mainPanel);
        menuProvider.menu.AddItem(new GUIContent("asd", EditorGUIUtility.whiteTexture), false, ()=> { Debug.Log("asd"); });
        menuProvider.menu.AddItem(new GUIContent("asd1", EditorGUIUtility.whiteTexture), false, () => { Debug.Log("asd1"); });
    }


}
