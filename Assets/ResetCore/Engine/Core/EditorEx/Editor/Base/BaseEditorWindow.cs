using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class BaseEditorWindow<T> : EditorWindow where T: BaseEditorWindow<T> {

    public static T window;
    public static void ShowMainWindow()
    {
        window =
            EditorWindow.GetWindow(typeof(T), true, "TestEditorUI") as T;
        window.Show();
    }

    public virtual void OnGUI()
    {
        DrawMenu();
    }


    protected void DrawMenu()
    {
        Event currentEvent = Event.current;
        if (currentEvent.type == EventType.ContextClick)
        {
            Vector2 mousePos = currentEvent.mousePosition;
            // Now create the menu, add items and show it
            GenericMenu menu = new GenericMenu();
            DrawMenu(menu);
            menu.ShowAsContext();
            currentEvent.Use();
        }
    }

    /// <summary>
    /// 绘制菜单
    /// </summary>
    /// <param name="menu"></param>
    protected virtual void DrawMenu(GenericMenu menu) { }

}
