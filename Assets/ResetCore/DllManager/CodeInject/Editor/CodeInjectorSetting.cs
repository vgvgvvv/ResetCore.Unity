using Mono.Cecil;
using ResetCore.Asset;
using ResetCore.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using UnityEngine;

namespace ResetCore.ReAssembly
{
    public class CodeInjectorSetting
    {
        private readonly List<string> assemblys = new List<string>();
        private readonly XDocument injectDllConfigXml;


        /// <summary>
        /// 用于复用注入器
        /// </summary>
        private static ObjectPool<string, BaseInjector> injectorPool = ObjectPool.CreatePool<BaseInjector>("InjectorPool");

        public CodeInjectorSetting()
        {
            var injectDllConfig = EditorResources.GetAsset<TextAsset>("InjectDll", "CodeInject");
            injectDllConfigXml = XDocument.Parse(injectDllConfig.text);
            //添加dll
            var dllEles = injectDllConfigXml.Root.XPathSelectElements("Common/item/dll");
            foreach(var dllEle in dllEles)
            {
                if (!assemblys.Contains(dllEle.Value))
                {
                    assemblys.Add(dllEle.Value);
                }
            }

        }


        /// <summary>
        /// 进行注入
        /// </summary>
        public void RunInject()
        {
            foreach(var dllPath in assemblys)
            {
                string path = Path.Combine(PathConfig.projectPath, dllPath);
                var assembly = AssemblyDefinition.ReadAssembly(path);
                DoInjector(assembly);
                SaveAssembly(path, assembly);
            }
        }

        /// <summary>
        /// 添加程序集
        /// </summary>
        /// <param name="scriptPath"></param>
        public void AddAssembly(string scriptPath)
        {
            assemblys.Add(scriptPath);
        }

        /// <summary>
        /// 读取Assembly
        /// </summary>
        /// <param name="path"></param>
        /// <param name="engineList"></param>
        /// <returns></returns>
        private AssemblyDefinition ReadAssembly(string path, List<string> engineList)
        {
            var assemblyResolver = new DefaultAssemblyResolver();
            foreach(string enginePath in engineList)
            {
                assemblyResolver.AddSearchDirectory(enginePath);
            }
            var readerParameters = new ReaderParameters
            {
                AssemblyResolver = assemblyResolver,
                ReadingMode = ReadingMode.Immediate,
                ReadSymbols = true
            };
            var assembly = AssemblyDefinition.ReadAssembly(path, readerParameters);
            return assembly;
        }

        /// <summary>
        /// 保存Assembly
        /// </summary>
        /// <param name="path"></param>
        /// <param name="assembly"></param>
        private void SaveAssembly(string path, AssemblyDefinition assembly)
        {
            Debug.Log(string.Format("WriteAssembly: {0}", path));
            assembly.Write(path, new WriterParameters());
        }

        /// <summary>
        /// 进行代码注入
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="injectList"></param>
        /// <returns></returns>
        private void DoInjector(AssemblyDefinition assembly)
        {
            //注入Class
            foreach (var type in assembly.MainModule.Types)
            {

                DoInjectClass(assembly, type);

                DoInjectMethod(assembly, type);

                DoInjectProperty(assembly, type);
            }
        }

        private void DoInjectClass(AssemblyDefinition assembly, TypeDefinition type)
        {
            if (type.HasCustomAttribute<InjectClass>())
            {
                var injectAttribute = type.GetCustomAttribute<InjectClass>();
                List<string> injectList = new List<string>();
                foreach (var arg in (CustomAttributeArgument[])(injectAttribute.ConstructorArguments[0].Value))
                {
                    injectList.Add(arg.Value as string);
                }
                if (injectList == null) return;

                foreach (string injectKey in injectList)
                {
                    GetMethodInject<BaseClassInjector>(injectKey).DoInjectClass(assembly, type);
                }
            }
        }

        private void DoInjectMethod(AssemblyDefinition assembly, TypeDefinition type)
        {
            if (type.HasCustomAttribute<InjectMethod>())
            {
                //得到注入的属性
                var injectAttribute = type.GetCustomAttribute<InjectMethod>();
                List<string> injectList = new List<string>();
                foreach (var arg in (CustomAttributeArgument[])(injectAttribute.ConstructorArguments[0].Value))
                {
                    injectList.Add(arg.Value as string);
                }
                if (injectList == null) return;


                //得到忽略的属性
                var ignoreInjectAttribute = type.GetCustomAttribute<IgnoreInjectMethod>();
                List<string> ignoreInjectList = new List<string>();
                if (ignoreInjectAttribute != null)
                {
                    foreach (var arg in (CustomAttributeArgument[])(ignoreInjectAttribute.ConstructorArguments[0].Value))
                    {
                        ignoreInjectList.Add(arg.Value as string);
                    }
                }

                //注入Method
                foreach (var method in type.Methods)
                {
                    foreach (string injectKey in injectList)
                    {
                        if (ignoreInjectList.Count > 0 && ignoreInjectList.Contains(injectKey))
                            continue;

                        GetMethodInject<BaseMethodInjector>(injectKey).DoInjectMethod(assembly, method, type);

                    }

                }
            }
            else
            {
                //注入Method
                foreach (var method in type.Methods)
                {
                    if (!method.HasCustomAttribute<InjectMethod>()) continue;

                    var injectAttribute = method.GetCustomAttribute<InjectMethod>();
                    List<string> injectList = new List<string>();
                    foreach (var arg in (CustomAttributeArgument[])(injectAttribute.ConstructorArguments[0].Value))
                    {
                        injectList.Add(arg.Value as string);
                    }
                    if (injectList.Count == 0) continue;

                    foreach (string injectKey in injectList)
                    {
                        GetMethodInject<BaseMethodInjector>(injectKey).DoInjectMethod(assembly, method, type);
                    }
                }
            }
        }

        private void DoInjectProperty(AssemblyDefinition assembly, TypeDefinition type)
        {
            //注入Property
            foreach (var property in type.Properties)
            {
                if (!property.HasCustomAttribute<InjectProperty>()) continue;

                var injectAttribute = property.GetCustomAttribute<InjectProperty>();
                List<string> injectList = new List<string>();
                foreach (var arg in (CustomAttributeArgument[])(injectAttribute.ConstructorArguments[0].Value))
                {
                    injectList.Add(arg.Value as string);
                }
                if (injectList.Count == 0) continue;

                foreach (string injectKey in injectList)
                {
                    GetMethodInject<BasePropertyInjector>(injectKey).DoInjectProperty(assembly, property, type);
                }
            }
        }

        /// <summary>
        /// 获取注入器
        /// </summary>
        /// <param name="injectKey"></param>
        /// <returns></returns>
        private T GetMethodInject<T>(string injectKey) where T : BaseInjector
        {
            if (!injectorPool.ContainsKey(injectKey))
            {
                Type injectorType = AssemblyManager.GetAssemblyType("Assembly-CSharp-Editor", "ResetCore.ReAssembly." + injectKey);
                injectorPool.Put(injectKey,
                    injectorType.GetConstructor(new Type[0]).Invoke(new object[0]) as T);
            }
            return injectorPool.Get(injectKey) as T;
        }

    }

}
