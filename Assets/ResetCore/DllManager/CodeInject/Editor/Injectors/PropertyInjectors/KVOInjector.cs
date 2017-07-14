using System;
using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;
using ResetCore.ReAssembly;
using Mono.Cecil.Cil;
using ResetCore.Event;
using System.Linq;
using System.Reflection;

namespace ResetCore.ReAssembly
{
    public class KVOInjector : BasePropertyInjector
    {

        public override void DoInjectProperty(AssemblyDefinition assembly, PropertyDefinition property, TypeDefinition type)
        {
            var methods = typeof(EventDispatcher).GetMethods();
            MethodInfo triggerMethod = null;
            foreach (var method in methods)
            {
                if(method.Name == "TriggerEvent" && method.GetGenericArguments().Count() == 1)
                {
                    triggerMethod = method;
                    break;
                }
            }

            var triggerRef = assembly.MainModule.Import(triggerMethod);
            //var objectRef = assembly.MainModule.Import(typeof(object));
            triggerRef = triggerRef.MakeGeneric(property.PropertyType);
            var typeRef = assembly.MainModule.Import(typeof(int));
            var fieldRef = type.Fields.Single(field => field.Name == GetHiddenFieldName(property));

            InjectEmitHelper.InsertBefore(property.SetMethod.GetILProcessor(), property.SetMethod.GetFirstInstriction(),
                new Dictionary<OpCode, object>()
                {
                    { OpCodes.Ldstr, type.Name + "." + property.Name},
                    { OpCodes.Ldarg_1, null},
                    //{ OpCodes.Castclass, objectRef},
                    { OpCodes.Ldnull, null},
                    { OpCodes.Call, triggerRef},
                });



            InjectEmitHelper.ComputeOffsets(property.SetMethod.Body);
        }


    }

}
