using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class BakeScenes : EditorWindow
{
    // 存放场景对象的数组
    public Object[] scenes;

    // Lists and string array for easier management
    // 为便于管理用的列表和字符串
    List<string> sceneList = new List<string>();
    private int sceneIndex = 0;
    private string[] scenePath;

    // Editor text
    // 编辑器文本
    string bakeButton = "Bake";
    string status = "Idle...";
    System.DateTime timeStamp;

    // 菜单项
    [MenuItem("Tools/Bake Scenes")]
    public static void ShowWindow()
    {
        EditorWindow window = GetWindow(typeof(BakeScenes), false, "Bake Scenes");
        window.autoRepaintOnSceneChange = true;
    }

    // 获取焦点时更新编辑器文本信息
    void OnFocus()
    {
        status = "Idle...";
        if (!Lightmapping.isRunning)
        {
            bakeButton = "Bake";
        }
    }

    void OnGUI()
    {
        // "target" can be any class derrived from ScriptableObject
        // (could be EditorWindow, MonoBehaviour, etc)
        // target可以是任何由ScriptableObject派生的类（如EditorWindow, MonoBehaviour等）
        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);
        SerializedProperty scenesProperty = so.FindProperty("scenes");

        EditorGUILayout.PropertyField(scenesProperty, true); // True means show children
        so.ApplyModifiedProperties(); // Remember to apply modified properties

        if (GUILayout.Button(bakeButton)) // Button to start bake process
        {
            InitializeBake();
        }
        EditorGUILayout.LabelField("Status: ", status);
        so.Update();
    }

    // 若当前没有烘焙任务，则设置代理、场景，并开始烘焙，
    // 否则，停止光照贴图并更新编辑器文本
    void InitializeBake()
    {
        if (!Lightmapping.isRunning)
        {
            Lightmapping.completed = null;
            Lightmapping.completed = SaveScene;
            Lightmapping.completed += BakeNewScene;
            SetScenes();
            BakeNewScene();
        }
        else
        {
            Lightmapping.Cancel();
            UpdateBakeProgress();
        }
    }

    // 创建要烘焙场景的字符串数组
    private bool SetScenes()
    {
        // Reset values
        sceneList.Clear();
        sceneIndex = 0;

        // 获取场景路径并存储在列表中                  
        if (scenes.Length == 0)
        {
            status = "No scenes found";
            bakeButton = "Bake";
            return false;
        }
        else
        {
            for (int i = 0; i < scenes.Length; i++)
            {
                sceneList.Add(AssetDatabase.GetAssetPath(scenes[i]));
            }

            // Sort and put scene paths in array
            // 排序并将场景路径放进数组
            scenePath = sceneList.ToArray();
            return true;
        }
    }

    // 循环进行场景烘焙，并更新进度
    private void BakeNewScene()
    {
        if (sceneIndex < scenes.Length)
        {
            EditorApplication.OpenScene(scenePath[sceneIndex]);
            timeStamp = System.DateTime.Now;
            Lightmapping.BakeAsync();
            UpdateBakeProgress();
            sceneIndex++;
        }
        else
        {
            DoneBaking("done");
        }
    }

    // 更新烘焙进度
    private void UpdateBakeProgress()
    {
        if (Lightmapping.isRunning)
        {
            status = "Baking " + (sceneIndex + 1).ToString() +
            " of " + scenes.Length.ToString();
            bakeButton = "Cancel";
        }
        else if (!Lightmapping.isRunning)
        {
            DoneBaking("cancel");
        }
    }

    // 开始新的烘焙前保存当前场景
    private void SaveScene()
    {
        System.TimeSpan bakeSpan = System.DateTime.Now - timeStamp;
        string bakeTime = string.Format("{0:D2}:{1:D2}:{2:D2}",
        bakeSpan.Hours, bakeSpan.Minutes, bakeSpan.Seconds);
        Debug.Log("(" + sceneIndex.ToString() + "/" +
        scenes.Length.ToString() + ") " + "Done baking: " +
        EditorApplication.currentScene + " after " + bakeTime +
        " on " + System.DateTime.Now.ToString());
        EditorApplication.SaveScene();
    }

    // 完成烘焙后，更新编辑器文本
    private void DoneBaking(string reason)
    {
        Lightmapping.completed = null;
        sceneList.Clear();
        sceneIndex = 0;

        if (reason == "done")
        {
            status = "Bake is done";
            bakeButton = "Bake";
        }
        else if (reason == "cancel")
        {
            status = "Canceled";
            bakeButton = "Bake";
        }
    }
}