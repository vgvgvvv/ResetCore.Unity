using UnityEngine;
using System.Collections;
using System;
using ResetCore.Util;
using UnityEngine.UI;
using System.Text;
using System.Collections.Generic;
using System.Reflection;

namespace ResetCore.UGUI
{
    public static class UGUIUtil
    {
        /// <summary>
        /// 是否为UI组件
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public static bool IsUIComponent(this Component component)
        {
            return component.GetType().Assembly.GetName().Name != "UnityEngine.UI";
        }

        
    }

}
