using UnityEngine;
using UnityEditor;
using System.IO;

namespace ResetCore.Data
{
    public class DataTools
    {
        [MenuItem("Tools/GameData/Clear All GameData files")]
        static void CleanGameData()
        {
            Debug.Log(PathConfig.localGameDataSourceRoot);
            Debug.Log(PathConfig.localGameDataClassRoot);

            if (Directory.Exists(PathConfig.localGameDataSourceRoot))
                Directory.Delete(PathConfig.localGameDataSourceRoot, true);

            if (Directory.Exists(PathConfig.localGameDataClassRoot))
                Directory.Delete(PathConfig.localGameDataClassRoot, true);
            AssetDatabase.Refresh();
        }
    }
}
