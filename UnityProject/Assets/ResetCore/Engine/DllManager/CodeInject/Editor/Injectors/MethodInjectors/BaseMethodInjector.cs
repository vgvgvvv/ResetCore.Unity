using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace ResetCore.ReAssembly
{
    /// <summary>
    /// 基于属性的注入器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseMethodInjector : BaseInjector
    {
        /// <summary>
        /// 执行注入操作
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="method"></param>
        /// <param name="type"></param>
        public abstract void DoInjectMethod(AssemblyDefinition assembly, MethodDefinition method, TypeDefinition type);

    }

    
}
