using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using ResetCore.Util;
using ResetCore.ReAssembly;
using System.Reflection;

namespace ResetCore.Json
{
    public class JsonLoadable : Attribute
    {

    }

    public class JsonObject
    {
        /// <summary>
        /// 将组件转换为JsonObject
        /// </summary>
        /// <param name="comp"></param>
        /// <returns></returns>
        public static JsonData NewInstance(Component comp)
        {
            var fields = comp.GetType().GetFields();
            JsonData data = new JsonData();
            foreach (var field in fields)
            {
                object value = field.GetValue(comp);

                //if (field.GetCustomAttributes(typeof(JsonLoadable), true).Length == 0)
                //    continue;

                data[field.Name + "#Assembly"] = field.FieldType.Assembly.GetName().Name;
                data[field.Name + "#Type"] = field.FieldType.FullName;
                if (StringEx.IsConvertableType(field.FieldType))
                {
                    data[field.Name] = value.ConverToString();
                }
                else if(field.FieldType == typeof(GameObject))
                {
                    data[field.Name] = GameObjectConvertToPath(comp.gameObject, ((GameObject)value));
                }
                else if(typeof(Component).IsAssignableFrom(field.FieldType))
                {
                    data[field.Name] = GameObjectConvertToPath(comp.gameObject, ((Component)value).gameObject);
                }
                else
                {
                    Debug.LogError("不支持的序列化类型:" + field.FieldType.Name);
                }
            }
            return data;
        }

        /// <summary>
        /// 数据应用到组件
        /// </summary>
        /// <param name="comp"></param>
        /// <param name="data"></param>
        public static void Apply(Component comp, JsonData data)
        {
            var fields = comp.GetType().GetFields();
            foreach (var field in fields)
            {
                //if (field.GetCustomAttributes(typeof(JsonLoadable), true).Length == 0)
                //    continue;

                if (!data.Keys.Contains(field.Name) || !data.Keys.Contains(field.Name + "#Type")
                    || !data.Keys.Contains(field.Name + "#Assembly"))
                    return;

                Type type = AssemblyManager.GetAssemblyType(data[field.Name + "#Assembly"].ToString(), 
                    data[field.Name + "#Type"].ToString());
                if (type == null)
                    return;

                if (StringEx.IsConvertableType(field.FieldType))
                {
                    object value = StringEx.GetValue(data[field.Name].ToString(), type);
                    field.SetValue(comp, value);
                }
                else if(field.FieldType == typeof(GameObject))
                {
                    object value = PathConvertToGameObject(comp.gameObject, data[field.Name].ToString());
                    field.SetValue(comp, value);
                }
                else if (typeof(Component).IsAssignableFrom(field.FieldType))
                {
                    object value = PathConvertToGameObject(comp.gameObject, data[field.Name].ToString())
                        .GetComponent(field.FieldType);
                    field.SetValue(comp, value);
                }
                else
                {
                    Debug.LogError("不支持的序列化类型:" + type.Name);
                }
            }
        }

        /// <summary>
        /// 将组件转换为路径
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="child"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GameObjectConvertToPath(GameObject owner, GameObject child, string path = "")
        {
            string newPath;
            if (string.IsNullOrEmpty(path))
                newPath = child.name;
            else
                newPath = child.name + "/" + path;

            if (child.transform.parent == null)
                return "";

            if (child.transform.parent == owner.transform)
                return newPath;
            else
                return GameObjectConvertToPath(owner, child.transform.parent.gameObject, newPath);
        }

        /// <summary>
        /// 将路径转换为组件
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static GameObject PathConvertToGameObject(GameObject owner, string path)
        {
            string childName = String.Empty;
            string subPath = String.Empty;
            if (path.Contains("/"))
            {
                childName = path.Substring(0, path.IndexOf('/'));
                subPath = path.Substring(path.IndexOf('/')+1);
            }else
            {
                childName = path;
            }

            var child = owner.transform.Find(childName);
            if (child == null)
                return null;

            if (string.IsNullOrEmpty(subPath))
            {
                return child.gameObject;
            }
            else
            {
                return PathConvertToGameObject(child.gameObject, subPath);
            }

        }
       
    }

}
