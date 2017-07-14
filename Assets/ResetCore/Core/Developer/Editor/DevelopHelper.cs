#if RESET_DEVELOPER

//using UnityEngine;
//using UnityEditor;
//using System.Collections;
//using System.IO;
//using ResetCore.Asset;
//using ResetCore.ModuleControl;

//public static class DevelopHelper {

    
//    //将在工程目录下写好的ExtraTool打包至ResetCore根目录下
//    [MenuItem("Tools/DeveloperTools/Compress Extra Tools")]
//    public static void ExportExtraToolsToPackage()
//    {
//        if (File.Exists(PathConfig.ExtraToolPathInPackage))
//        {
//            File.Delete(PathConfig.ExtraToolPathInPackage);
//        }
//        CompressHelper.CompressDirectory(PathConfig.ExtraToolPath, PathConfig.ExtraToolPathInPackage);
//        Debug.logger.Log("Compress To " + PathConfig.ExtraToolPathInPackage);
//    }

//    [MenuItem("Tools/DeveloperTools/Compress SDK")]
//    public static void ExportSDKToPackage()
//    {
//        if (File.Exists(PathConfig.SDKPathInPackage))
//        {
//            File.Delete(PathConfig.SDKPathInPackage);
//        }
//        CompressHelper.CompressDirectory(PathConfig.SDKBackupPath, PathConfig.SDKPathInPackage);
//        Debug.logger.Log("Compress To " + PathConfig.SDKBackupPath);
//    }

//    [MenuItem("Tools/DeveloperTools/Open Todo List")]
//    public static void OpenTodoList()
//    {
//        EditorUtility.OpenWithDefaultApp(PathConfig.ResetCorePath + "TodoList.txt");
//    }

//    [MenuItem("Tools/DeveloperTools/Decompress SDK And Tools")]
//    public static void DecompressSDKAndTools()
//    {
//        //将额外工具移至工程目录
//        ModuleControl.MoveToolsToProject();
//        //将SDK移至工程目录备份
//        ModuleControl.MoveSDKToTemp();
//    }
//}

#endif