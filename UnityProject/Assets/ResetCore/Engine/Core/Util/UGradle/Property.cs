using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResetCore.UGradle
{
    public abstract class Property
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 应用属性 
        /// </summary>
        public abstract void Apply(WorkFlow workFlow);

    }
}
