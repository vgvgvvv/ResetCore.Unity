using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResetCore.Data
{

    [AttributeUsage(AttributeTargets.Field|AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SerializeIgnore : Attribute { }

}
