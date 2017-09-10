using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResetCore.ReAssembly
{
    public abstract class BaseClassInjector : BaseInjector
    {

        /// <summary>
        /// 注入类
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="type"></param>
        public abstract void DoInjectClass(AssemblyDefinition assembly, TypeDefinition type);

    }

}
