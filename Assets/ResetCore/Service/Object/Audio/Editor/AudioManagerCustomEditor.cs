using UnityEngine;
using System.Collections;
using UnityEditor;
using ResetCore.Asset;

namespace ResetCore.ResObject
{
    [CustomEditor(typeof(AudioManager), false)]
    public class AudioManagerCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            
            base.OnInspectorGUI();
#if !DATA_GENER
            AudioManager am = target as AudioManager;
            if (am.localization)
            {
                EditorGUILayout.HelpBox("If you want to use localization, please open the DataGener module", MessageType.Info);
            }
#endif
            if(GUILayout.Button("Open Mixer", GUILayout.Height(30)))
            {
                UnityEngine.Object obj = EditorResources.GetAsset<UnityEngine.Object>("AudioManagerMixer", "ResetCore", "Object") as UnityEngine.Object;
                AssetDatabase.OpenAsset(obj);
            }
        }

    }
}
