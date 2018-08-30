using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;
using ResetCore.CodeDom;
using ResetCore.Util;
using System.IO;
using ResetCore.ScriptObj;

namespace ResetCore.ReAssembly
{
    [CustomEditor(typeof(ScriptComponentLoader), false)]
    public class ScriptComponentLoaderEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            ScriptComponentLoader scrLoader = target as ScriptComponentLoader;
            Component[] componentArray = scrLoader.GetComponents<Component>();

            //GUILayout.Label("Assembly Name : " + scrLoader.assemblyName);
            //GUILayout.Label("Component Name : " + scrLoader.componentName);
            
            ///初始化类文件
            if (scrLoader.inited == false)
            {
                EditorGUILayout.HelpBox("Init Property CS File", MessageType.Info);

                for (int i = 0; i < componentArray.Length; i++)
                {
                    System.Type compType = componentArray[i].GetType();
                    if (compType == typeof(ScriptComponentLoader) || compType.Assembly.GetName().Name == "UnityEngine")
                        continue;

                    if (GUILayout.Button("Gen PropertyScr " + compType.Name))
                    {
                        GetComponentInfomation(scrLoader, compType);
                        List<FieldInfo> fieldList = FindFieldInfoDict(componentArray[i]);
                        GenScriptableObject(fieldList, scrLoader, componentArray[i]);
                        scrLoader.inited = true;
                    }
                }
            }
            ///初始化数值
            else if (scrLoader.gameObject.GetComponent(scrLoader.componentName) != null)
            {
                Component comp = scrLoader.gameObject.GetComponent(scrLoader.componentName);
                if(comp == null)
                {
                    scrLoader.inited = false;
                }

                if (scrLoader.srcObj == null)
                {
                    EditorGUILayout.HelpBox("Update Property Object Please!", MessageType.Error);
                }
                else
                {
                    EditorGUILayout.HelpBox("You are ready!", MessageType.Info);
                    ShowField(scrLoader);
                }

                if(comp != null)
                {
                    if (GUILayout.Button("Update PropertyObj " + comp.GetType().Name))
                    {
                        UpdateScriptableObject(scrLoader, comp);
                    }
                    if (GUILayout.Button("Remove " + comp.GetType().Name))
                    {
                        DestroyImmediate(comp);
                    }
                }
            }
            ///去除Component后
            else
            {
                EditorGUILayout.HelpBox("You are ready!", MessageType.Info);
                ShowField(scrLoader);

                if (GUILayout.Button("Revert Component " + scrLoader.componentName))
                {
                    RevertComponent(scrLoader);
                }
            }

            if (scrLoader.inited == true)
            {
                ///重置
                if (GUILayout.Button("Reset"))
                {
                    scrLoader.inited = false;
                }
            }
        }

        private void GetComponentInfomation(ScriptComponentLoader scrLoader, System.Type compType)
        {
            scrLoader.assemblyName = compType.Assembly.GetName().Name;
            scrLoader.componentName = compType.FullName;
        }

        private List<FieldInfo> FindFieldInfoDict(Component comp)
        {
            Type compType = comp.GetType();
            FieldInfo[] fieldInfos = compType.GetFields();

            var propertyDict = new List<FieldInfo>();

            for (int i = 0; i < fieldInfos.Length; i++)
            {
                object[] loadableAttrs = fieldInfos[i].GetCustomAttributes(typeof(LoadableAttribute), false);
                if (loadableAttrs.Length == 0) continue;
                propertyDict.Add(fieldInfos[i]);
            }

            return propertyDict;
        }

        private void GenScriptableObject(List<FieldInfo> list, ScriptComponentLoader scrLoader, Component comp)
        {
            CodeGener gener = new CodeGener("ResetCore.ReAssembly.Data", GetScrObjName(comp));

            gener
                .AddImport("UnityEngine")
                .AddBaseType("ScriptableObject");
            foreach (FieldInfo info in list)
            {
                gener.AddMemberField(info.FieldType, info.Name, null, System.CodeDom.MemberAttributes.Public);
            }
            gener.GenCSharp(DllManagerConst.scriptableCSOutputPath);
        }

        private void UpdateScriptableObject(ScriptComponentLoader scrLoader, Component comp)
        {
            if(scrLoader.srcObj == null)
            {
                scrLoader.srcObj = ScriptableObject.CreateInstance(GetScrObjName(comp));
            }
            
            FieldInfo[] fieldInfos = scrLoader.srcObj.GetType().GetFields();
            Type compType = comp.GetType();
            for(int i = 0; i < fieldInfos.Length; i++)
            {
                object value = compType.GetField(fieldInfos[i].Name).GetValue(comp);
                fieldInfos[i].SetValue(scrLoader.srcObj, value);
            }
        }

        private void RevertComponent(ScriptComponentLoader scrLoader)
        {
            Assembly assemble = AssemblyManager.DefaultCSharpAssembly;
            if (!string.IsNullOrEmpty(scrLoader.assemblyName))
            {
                assemble = AssemblyManager.GetAssembly(scrLoader.assemblyName);
            }
            System.Type type = assemble.GetType(scrLoader.componentName);
            if (type == null)
            {
                Debug.LogError("script " + scrLoader.componentName + " can not be found in " + (string.IsNullOrEmpty(scrLoader.assemblyName) ? "defaultCSharpAssembly" : scrLoader.assemblyName));
                return;
            }
            Component comp;
            if (scrLoader.componentName != null)
            {
                comp = scrLoader.gameObject.AddComponent(type);
                FieldInfo[] compFieldInfos = comp.GetType().GetFields();
                Type objType = scrLoader.srcObj.GetType();
                for (int i = 0; i < compFieldInfos.Length; i++)
                {
                    object value = objType.GetField(compFieldInfos[i].Name).GetValue(scrLoader.srcObj);
                    compFieldInfos[i].SetValue(comp, value);
                }
            }
        }

        private void ShowField(ScriptComponentLoader scrLoader)
        {
            FieldInfo[] fieldInfos = scrLoader.srcObj.GetType().GetFields();
            for (int i = 0; i < fieldInfos.Length; i++)
            {
                GUILayout.Label(fieldInfos[i].FieldType.Name + " " + fieldInfos[i].Name + " = " + fieldInfos[i].GetValue(scrLoader.srcObj));
            }
        }

        private string GetScrObjName(Component comp)
        {
            return comp.gameObject.name + "_" + comp.GetType().Name;
        }

        private string GetScrObjCSPath(Component comp)
        {
            return PathEx.Combine(DllManagerConst.scriptableCSOutputPath, GetScrObjName(comp) + ".cs");
        }
    }
}
