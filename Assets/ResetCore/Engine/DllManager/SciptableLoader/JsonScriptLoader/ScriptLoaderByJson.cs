using LitJson;
using ResetCore.Json;
using ResetCore.ReAssembly;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ResetCore.ReAssembly
{
    public class ScriptLoaderByJson : MonoBehaviour
    {

        //assembly名称
        public string assemblyName;

        //类型名称
        public string componentName;

        //记录属性的
        public string srcObj;

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
            if (!string.IsNullOrEmpty(assemblyName))
            {
                assemble = AssemblyManager.GetAssembly(assemblyName);
            }
            System.Type type = assemble.GetType(componentName);
            if (type == null)
            {
                Debug.LogError("script " + componentName + " can not be found in " + (string.IsNullOrEmpty(assemblyName) ? "defaultCSharpAssembly" : assemblyName));
                return;
            }
            Component comp;
            if (componentName != null)
            {
                comp = gameObject.AddComponent(type);

                var data = JsonMapper.ToObject(srcObj);
                if (data != null)
                    JsonObject.Apply(comp, data);
            }


            Destroy(this);
        }
    }

}
