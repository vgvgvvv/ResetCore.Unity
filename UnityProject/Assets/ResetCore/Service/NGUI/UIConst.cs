using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ResetCore.NGUI
{
    public class UIConst
    {

        public enum UIName
        {
            TestPanel
        }

        public static readonly Dictionary<UIName, string> UIPrefabNameDic = new Dictionary<UIName, string>()
        {
            {UIName.TestPanel, "TestPanel.prefab"}
        };

        public static readonly Dictionary<UIName, string> UIPrefabBundleDic = new Dictionary<UIName, string>()
        {
            {UIName.TestPanel, "ui"}
        };
    }
}

