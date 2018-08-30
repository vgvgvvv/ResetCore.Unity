using UnityEngine;
using System.Collections;
using UnityEditor;
using ResetCore.Asset;

namespace ResetCore.Event
{
    public class EventSystemMenu
    {
        [MenuItem("Tools/Event/Open Event Const File")]
        static void OpenEventConstFile()
        {
            Object obj = EditorResources.GetAsset<Object>("EventsConst", "ResetCore", "Core", "Event") as Object;
            AssetDatabase.OpenAsset(obj);
        }
    }

}
