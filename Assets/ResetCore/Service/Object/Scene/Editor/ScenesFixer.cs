using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using UnityEditor.SceneManagement;

public class ScenesFixer {

    private static float process;


    [MenuItem("Assets/场景规范化/将选中场景中空物体的位置设置为世界坐标原点")]
    public static void SetFolderPosition()
    {
        var selection = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
        var paths = (from s in selection
                     let path = AssetDatabase.GetAssetPath(s)
                     where File.Exists(path) && path.EndsWith(".unity")
                     select path).ToArray();

        int sceneNum = 0;
        foreach (string item in paths)
        {
            process = (float)sceneNum / (float)paths.Length;
            EditorUtility.DisplayProgressBar("查找中", "正在修正" + EditorSceneManager.GetActiveScene() + "场景文件，是第" + sceneNum + "个场景", process);
            EditorSceneManager.OpenScene(item, OpenSceneMode.Single);
            FixFolderPosition();

            sceneNum++;
            Debug.Log(item + "检查完毕");
            EditorSceneManager.SaveOpenScenes();
        }
        EditorUtility.ClearProgressBar();

    }

    public static void FixFolderPosition()
    {
        
        List<Transform> FixTransformList;
        FixTransformList = new List<Transform>();
        FixTransformList.Add(GameObject.Find("DynamicData").transform);
        FixTransformList.Add(GameObject.Find("StaticData").transform);
        for (int i = 0; i < FixTransformList.Count; i++)
        {
            FindFolderToMove(FixTransformList[i]);

        }
    }

    private static void FindFolderToMove(Transform parentTran)
    {
        //所有非Prefab都将被当做文件夹，并且一旦遇到预设就退出检查
        if (PrefabUtility.GetPrefabType(parentTran) == PrefabType.None)
        {
            MoveFolderToZero(parentTran);
            if (parentTran.childCount > 0)
            {
                for (int i = 0; i < parentTran.childCount; i++)
                {
                    FindFolderToMove(parentTran.GetChild(i));
                }
            }
        }
    }
    //移动文件夹到0点坐标
    public static void MoveFolderToZero(Transform folder)
    {
        Dictionary<Transform, List<Vector3>> transformInfoDict = new Dictionary<Transform, List<Vector3>>();
        if (folder.childCount > 0)
        {
            for (int i = 0; i < folder.childCount; i++)
            {
                List<Vector3> transformInfo = new List<Vector3>();
                Transform child = folder.GetChild(i);
                transformInfo.Add(child.position);
                transformInfo.Add(child.eulerAngles);
                transformInfo.Add(folder.localScale);
                transformInfoDict.Add(child, transformInfo);
            }
            folder.position = Vector3.zero;
            folder.eulerAngles = Vector3.zero;
            folder.localScale = Vector3.one;
            foreach (KeyValuePair<Transform, List<Vector3>> transformInfo in transformInfoDict)
            {
                Transform child = transformInfo.Key;
                child.position = transformInfo.Value[0];
                child.eulerAngles = transformInfo.Value[1];
                child.localScale = new Vector3(transformInfo.Value[2].x * child.localScale.x, transformInfo.Value[2].y * child.localScale.y, transformInfo.Value[2].z * child.localScale.z);
            }
        }
        else
        {
            folder.position = Vector3.zero;
            folder.eulerAngles = Vector3.zero;
            folder.localScale = Vector3.one;
        }
        

    }
	
}
