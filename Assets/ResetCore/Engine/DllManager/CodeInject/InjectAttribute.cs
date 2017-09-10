using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResetCore.ReAssembly
{
    //需要注入的类
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class InjectClass : Attribute
    {
        public List<string> injectNameList { get; private set; }
        public InjectClass(params string[] injectNames)
        {
            injectNameList = new List<string>(injectNames);
        }
    }

    //需要注入的函数或者类
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Method)]
    public sealed class InjectMethod : Attribute
    {
        public List<string> injectNameList { get; private set; }
        public InjectMethod(params string[] injectNames)
        {
            injectNameList = new List<string>(injectNames);
        }
    }

    //需要忽略注入的类或者函数
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class IgnoreInjectMethod : Attribute
    {
        public List<string> ignoreinjectNameList { get; private set; }
        public IgnoreInjectMethod(params string[] ignoreNames)
        {
            ignoreinjectNameList = new List<string>(ignoreNames);
        }
    }



    //需要注入的属性
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class InjectProperty : Attribute
    {
        public List<string> injectNameList { get; private set; }
        public InjectProperty(params string[] injectNames)
        {
            injectNameList = new List<string>(injectNames);
        }
    }

}
