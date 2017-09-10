using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;


namespace ResetCore.UGUI
{
    public static class UIConst
    {
        //存储Prefab的路径
        public static string uiPrefabPath = "UI";

      

        public enum UIName
        {
            TestUI,
            HAHAUI
        }

        public static Dictionary<UIName, string> UIPrefabNameDic = new Dictionary<UIName, string>
        {
            {UIName.TestUI, "TestUI.prefab" },
            {UIName.HAHAUI, "HAHAUI.prefab" }
        };

        public static Dictionary<UIName, string> UIPrefabBundleDic = new Dictionary<UIName, string>
        {
            {UIName.TestUI, "ui" },
            {UIName.HAHAUI, "ui" }
        };

        #region inner
        //默认sprite包名
        public const string defaultPackage = "default";
        //
        public static readonly string spritePrefabPath = "SpritePacker";
        //UGUI资源文件夹目录
        public static readonly string ResourcesPath = PathConfig.resourcePath;
        //SpritePrefab所在的地址
        public static readonly string spritePrefabPathAbstractPath = ResourcesPath + spritePrefabPath;
        #endregion

    }
}

