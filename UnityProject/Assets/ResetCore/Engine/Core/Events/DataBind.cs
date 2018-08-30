using UnityEngine.UI;
using ResetCore.Event;
using ResetCore.Util;
using System.Reflection;
using UnityEngine;

namespace ResetCore.Event
{
   
    public static class DataBind
    {
        /// <summary>
        /// 绑定数据到Text
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="text"></param>
        /// <param name="dataId"></param>
        /// <param name="func"></param>
        public static void BindData<T>(this Text text, string dataId, System.Func<T, string> func = null)
        {
            EventDispatcher.AddEventListener<T>(dataId, (data) =>
            {
                if (func == null)
                {
                    text.text = data.ConverToString();
                }
                else
                {
                    text.text = func(data);
                }
                
            }, text);
        }

        /// <summary>
        /// 绑定事件到任意对象域
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <typeparam name="V">域类型</typeparam>
        /// <param name="obj">绑定的对象</param>
        /// <param name="dataId">绑定的事件</param>
        /// <param name="fieldName">域名</param>
        /// <param name="act">处理传递值的函数</param>
        public static void BindField<T, V>(this T obj, string dataId, string fieldName, System.Func<V, V> act = null)
        {
            System.Type fieldType = obj.GetType();
            if(fieldType != typeof(V))
            {
                Debug.unityLogger.LogError("数据绑定错误", "域类型为" + fieldType.Name + "函数返回类型为" + typeof(V).Name);
            }
            EventDispatcher.AddEventListener<V>(dataId, (data) =>
            {
                FieldInfo field = fieldType.GetField(fieldName);
                if(act != null)
                {
                    field.SetValue(obj, act(data));
                }
                else
                {
                    field.SetValue(obj, data);
                }
            }, obj);
        }

        
        /// <summary>
        /// 绑定任意对象中的属性
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <typeparam name="V">属性类型</typeparam>
        /// <param name="obj">绑定的对象</param>
        /// <param name="dataId">绑定的事件Id</param>
        /// <param name="propertyName">绑定的属性名</param>
        /// <param name="act">处理属性的函数</param>
        public static void BindProperty<T, V>(this T obj, string dataId, string propertyName, System.Func<V, V> act = null)
        {
            System.Type propertyType = obj.GetType();
            if (propertyType != typeof(V))
            {
                Debug.unityLogger.LogError("数据绑定错误", "属性类型为" + propertyType.Name + "函数返回类型为" + typeof(V).Name);
            }
            EventDispatcher.AddEventListener<V>(dataId, (data) =>
            {
                PropertyInfo field = propertyType.GetProperty(propertyName);
                if (act != null)
                {
                    field.SetValue(obj, act(data), null);
                }
                else
                {
                    field.SetValue(obj, data, null);
                }
            }, obj);
        }
        /// <summary>
        /// 改变数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataId"></param>
        /// <param name="value"></param>
        public static void ChangeData<T>(string dataId, T value)
        {
            EventDispatcher.TriggerEvent<T>(dataId, value);
        }

        public static void Bind<T>(string eventName, System.Func<T> getter, System.Action<T> setter)
        {

        }
    }

}
