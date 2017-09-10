using ResetCore.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ResetCore.ReAssembly
{
    [CustomEditor(typeof(ScriptLoaderByJson))]
    public class ScriptLoaderByJsonEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            ScriptLoaderByJson loader = (ScriptLoaderByJson)target;
            var comps = loader.gameObject.GetComponents<Component>();

            foreach(var comp in comps)
            {
                var compType = comp.GetType();
                if (compType == typeof(ScriptLoaderByJson) || compType.Assembly.GetName().Name == "UnityEngine")
                    continue;

                if(GUILayout.Button("Gen Json To" + compType.Name))
                {
                    GetComponentInfomation(loader, compType);
                    loader.srcObj = JsonObject.NewInstance(comp).ToJson();
                }
            }
        }

        private void GetComponentInfomation(ScriptLoaderByJson scrLoader, System.Type compType)
        {
            scrLoader.assemblyName = compType.Assembly.GetName().Name;
            scrLoader.componentName = compType.FullName;
        }
    }

}
