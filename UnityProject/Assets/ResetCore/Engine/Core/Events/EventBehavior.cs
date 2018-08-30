using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace ResetCore.Event
{
    /// <summary>
    /// 代表了能够生成事件的函数
    /// </summary>
    [AttributeUsage(AttributeTargets.Method|AttributeTargets.Field|AttributeTargets.Property)]
    public sealed class GenEventable : Attribute
    {
        public string[] eventName { get; private set; }

        public GenEventable(params string[] eventName)
        {
            this.eventName = eventName;
        }
    }

    public static class EventBehavior
    {

        /// <summary>
        /// 为所有绑定了GenEventable的函数、属性、值域绑定事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mono"></param>
        public static void GenEvent<T>(T mono)
        {
            Type monoType = mono.GetType();

            //绑定函数
            MethodInfo[] methodInfos = monoType.GetMethods();
            HandleMethods<T>(mono, methodInfos);

            //绑定属性
            PropertyInfo[] propInfos = monoType.GetProperties();
            HandleProperties(mono, propInfos);

            //绑定值域
            FieldInfo[] fieldInfos = monoType.GetFields();
            HandleFields(mono, fieldInfos);
        }

        /// <summary>
        /// 清除事件
        /// </summary>
        /// <param name="mono"></param>
        public static void ClearEvent<T>(T mono)
        {
            MonoEventDispatcher.monoEventControllerDict.Remove(mono);
        }

        /// <summary>
        /// 绑定数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mono"></param>
        /// <param name="eventName"></param>
        /// <param name="setter"></param>
        public static void BindData<T>(MonoBehaviour mono, string eventName, Action<T> setter)
        {
            EventDispatcher.AddEventListener<T>(eventName, setter, mono);
        }

        /// <summary>
        /// 设定数值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventName"></param>
        /// <param name="value"></param>
        public static void SetData<T>(string eventName, T value)
        {
            EventDispatcher.TriggerEvent<T>(eventName, value);
        }

        #region 私有函数
        private static void HandleMethods<T>(T mono, MethodInfo[] methodInfos)
        {
            for(int i = 0; i < methodInfos.Length; i++)
            {
                HandleMethod(mono, methodInfos[i]);
            }
        }

        private static void HandleMethod<T>(T mono, MethodInfo method)
        {
            object[] attrs = method.GetCustomAttributes(typeof(GenEventable), true);
            if (attrs.Length == 0) return;
            GenEventable genEventAttr = attrs[0] as GenEventable;


            ParameterInfo[] paras = method.GetParameters();
            var eventNames = genEventAttr.eventName;

            if(eventNames == null || eventNames.Length == 0)
            {
                eventNames = new string[] { method.Name };
            }

            for (int i = 0; i < eventNames.Length; i++)
            {
                switch (paras.Length)
                {
                    case 0:
                        {
                            EventDispatcher.AddEventListener(eventNames[i],
                                () => { method.Invoke(mono, new object[0]); }, mono);
                        }
                        break;
                    case 1:
                        {
                            EventDispatcher.AddEventListener<object>(eventNames[i],
                                (arg1) => { method.Invoke(mono, new object[] { arg1 }); }, mono);
                        }
                        break;
                    case 2:
                        {
                            EventDispatcher.AddEventListener<object, object>(eventNames[i],
                                (arg1, arg2) => { method.Invoke(mono, new object[] { arg1, arg2 }); }, mono);
                        }
                        break;
                    case 3:
                        {
                            EventDispatcher.AddEventListener<object, object, object>(eventNames[i],
                                (arg1, arg2, arg3) => { method.Invoke(mono, new object[] { arg1, arg2, arg3 }); }, mono);
                        }
                        break;
                    case 4:
                        {
                            EventDispatcher.AddEventListener<object, object, object, object>(eventNames[i],
                                (arg1, arg2, arg3, arg4) => { method.Invoke(mono, new object[] { arg1, arg2, arg3, arg4 }); }, mono);
                        }
                        break;
                    default:
                        {
                            Debug.unityLogger.LogError("Event Gen Error", "The method " + method.Name + " has too much para");
                        }
                        break;
                }
            }
        }

        private static void HandleProperties<T>(T mono, PropertyInfo[] propertyInfos)
        {
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                HandleProperty(mono, propertyInfos[i]);
            }
        }

        private static void HandleProperty<T>(T mono, PropertyInfo propertyInfo)
        {
            object[] attrs = propertyInfo.GetCustomAttributes(typeof(GenEventable), true);
            if (attrs.Length == 0) return;
            GenEventable genEventAttr = attrs[0] as GenEventable;

            var eventNames = genEventAttr.eventName;

            if (eventNames == null || eventNames.Length == 0)
            {
                eventNames = new string[] { propertyInfo.Name };
            }

            for (int i = 0; i < eventNames.Length; i++)
            {
                EventDispatcher.AddEventListener<object>(eventNames[i],
                                (arg1) => { propertyInfo.SetValue(mono, arg1, new object[0]); }, mono);
            }
                
        }

        private static void HandleFields<T>(T mono, FieldInfo[] fieldInfos)
        {
            for (int i = 0; i < fieldInfos.Length; i++)
            {
                HandleField(mono, fieldInfos[i]);
            }
        }

        private static void HandleField<T>(T mono, FieldInfo fieldInfo)
        {
            object[] attrs = fieldInfo.GetCustomAttributes(typeof(GenEventable), true);
            if (attrs.Length == 0) return;
            GenEventable genEventAttr = attrs[0] as GenEventable;

            var eventNames = genEventAttr.eventName;

            if (eventNames == null || eventNames.Length == 0)
            {
                eventNames = new string[] { fieldInfo.Name };
            }

            for (int i = 0; i < eventNames.Length; i++)
            {
                EventDispatcher.AddEventListener<object>(eventNames[i],
                                (arg1) => { fieldInfo.SetValue(mono, arg1); }, mono);
            }
        }
        #endregion 私有函数

    }
}
