using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResetCore.ReAssembly
{
    public abstract class BasePropertyInjector : BaseInjector
    {

        /// <summary>
        /// 注入属性
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="property"></param>
        /// <param name="type"></param>
        public abstract void DoInjectProperty(AssemblyDefinition assembly, PropertyDefinition property, TypeDefinition type);

        /// <summary>
        /// 获取隐藏的值域名
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        protected string GetHiddenFieldName(PropertyDefinition property)
        {
            return "<" + property.Name + ">k__BackingField";
        }
    }

}
