using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResetCore.Util
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property)]
    public class LoadableAttribute : Attribute
    {

        /// <summary>
        /// 数据名
        /// </summary>
        public string dataName { get; private set; }
        /// <summary>
        /// 数据组名
        /// </summary>
        public string groupName { get; private set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public object defaultValue { get; private set; }

        public LoadableAttribute(string dataName, object defaultValue, string groupName = "default")
        {
            this.dataName = dataName;
            this.defaultValue = defaultValue;
            this.groupName = groupName;
        }

    }
    [AttributeUsage(AttributeTargets.Class)]
    public class LoadableClassAttribute : Attribute
    {
        /// <summary>
        /// 数据组名
        /// </summary>
        public string groupName { get; private set; }
        public LoadableClassAttribute(string groupName = "default")
        {
            this.groupName = groupName;
        }
    }

}
