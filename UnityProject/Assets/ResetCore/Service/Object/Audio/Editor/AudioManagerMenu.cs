using UnityEngine;
using System.Collections;
using UnityEditor;
using ResetCore.Asset;

namespace ResetCore.ResObject
{
    public static class AudioManagerMenu
    {
        [MenuItem("Tools/Object/Create AudioManager")]
        static void CreateAudioManager()
        {
            Object obj = EditorResources.GetAsset<Object>("AudioManager", "ResetCore", "Resources", "AudioManager");
            GameObject go = GameObject.Instantiate(obj, Vector3.zero, Quaternion.identity) as GameObject;
            go.name = "AudioManager";
        }

    }

}
