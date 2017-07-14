using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System;

namespace ResetCore.ReAssembly
{
    /// <summary>
    /// 用于从Assembly中加载出Component然后加到物体上面
    /// </summary>
    public class ScriptComponentLoader : MonoBehaviour
    {
        //assembly名称
        public string assemblyName;
        
        //类型名称
        public string componentName;
        
        //记录属性的
        public ScriptableObject srcObj;

        //是否已经初始化
        public bool inited = false;

        /// <summary>
        /// 默认Assembly
        /// </summary>
        private static Assembly defaultCSharpAssembly;
        public static Assembly DefaultCSharpAssembly
        {
            get
            {
               return AssemblyManager.DefaultCSharpAssembly;
            }
        }

        void Awake()
        {
            Assembly assemble = DefaultCSharpAssembly;
            if(!string.IsNullOrEmpty(assemblyName))
            {
                assemble = AssemblyManager.GetAssembly(assemblyName);
            }
            System.Type type = assemble.GetType(componentName);
            if (type == null)
            {
                Debug.LogError("script " + componentName + " can not be found in " + (string.IsNullOrEmpty(assemblyName)? "defaultCSharpAssembly" : assemblyName));
                return;
            }
            Component comp;
            if (componentName != null)
            {
                comp = gameObject.AddComponent(type);
                FieldInfo[] fieldInfos = comp.GetType().GetFields();
                Type objType = srcObj.GetType();
                for (int i = 0; i < fieldInfos.Length; i++)
                {
                    object value = objType.GetField(fieldInfos[i].Name).GetValue(srcObj);
                    fieldInfos[i].SetValue(comp, value);
                }
            }

            
            Destroy(this);
        }
    }

}
