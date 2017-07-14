using UnityEngine;
using System.Collections;
using UnityEditor;
using ResetCore.Asset;

namespace ResetCore.NetPost
{
    public class NetPostTools
    {
        /// <summary>
        /// 打开网络常量
        /// </summary>
        [MenuItem("Tools/Net/Open Net Const")]
        public static void OpenNetConst()
        {
            AssetDatabase.OpenAsset(EditorResources.GetAsset<Object>("ServerConst", "ResetCore/NetPost/"));
        }
    }
}
