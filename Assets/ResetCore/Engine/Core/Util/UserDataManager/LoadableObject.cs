using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ResetCore.Util
{
    public class LoadableObject
    {
        /// <summary>
        /// 类型
        /// </summary>
        private Type type;

        /// <summary>
        /// 记录属性
        /// </summary>
        Dictionary<PropertyInfo, string[]> propertyDataNameDict = new Dictionary<PropertyInfo, string[]>();
        Dictionary<PropertyInfo, object> propertyDefaultValueDict = new Dictionary<PropertyInfo, object>();
        /// <summary>
        /// 记录域
        /// </summary>
        Dictionary<FieldInfo, string[]> fieldDataNameDict = new Dictionary<FieldInfo, string[]>();
        Dictionary<FieldInfo, object> fieldDefaultValueDict = new Dictionary<FieldInfo, object>();

        /// <summary>
        /// 记录方法
        /// </summary>
        Dictionary<MethodInfo, string[]> methodDataNameDict = new Dictionary<MethodInfo, string[]>();
        Dictionary<MethodInfo, object> methodDefaultValueDict = new Dictionary<MethodInfo, object>();

        /// <summary>
        /// 默认数据组名称
        /// </summary>
        public string groupName { get; private set; }

        public LoadableObject()
        {
            InitReflectInfo();
        }

        /// <summary>
        /// 加载属性、域和函数的数据
        /// </summary>
        public void Load()
        {
            foreach(var key in propertyDataNameDict.Keys)
            {
                var val = UserDataManager.GetDataOrDef(propertyDataNameDict[key][1], propertyDataNameDict[key][0], 
                    propertyDefaultValueDict[key], key.PropertyType);
                key.SetValue(this, val, new object[0]);
            }

            foreach (var key in fieldDataNameDict.Keys)
            {
                var val = UserDataManager.GetDataOrDef(fieldDataNameDict[key][1], fieldDataNameDict[key][0],
                    fieldDefaultValueDict[key], key.FieldType);
                key.SetValue(this, val);
            }

            foreach(var key in methodDataNameDict.Keys)
            {
                var val = UserDataManager.GetDataOrDef(methodDataNameDict[key][1], methodDataNameDict[key][0], 
                    methodDataNameDict[key], key.GetParameters()[0].ParameterType);
                key.Invoke(this, new object[] { val });
            }
        }

        /// <summary>
        /// 保存属性与域的数据
        /// </summary>
        public void Save()
        {
            foreach (var key in propertyDataNameDict.Keys)
            {
                UserDataManager.SetData(propertyDataNameDict[key][1], propertyDataNameDict[key][0], key.GetValue(this, new object[0]));
            }

            foreach (var key in fieldDataNameDict.Keys)
            {
                UserDataManager.SetData(fieldDataNameDict[key][1], fieldDataNameDict[key][0], key.GetValue(this));
            }
        }

        private void InitReflectInfo()
        {
            type = GetType();
            var propertys = type.GetProperties();
            var fields = type.GetFields();
            var methods = type.GetMethods();

            var classAttr = type.GetFirstAttribute<LoadableClassAttribute>(true);
            if(classAttr != null)
            {
                this.groupName = classAttr.groupName;
            }else
            {
                this.groupName = "default";
            }

            for (int i = 0; i < propertys.Length; i++)
            {
                var attr = propertys[i].GetFirstAttribute<LoadableAttribute>(true);
                if (attr == null)
                    continue;

                string attrGroupName = attr.groupName == "default" ? groupName : attr.groupName;
                string attrDataName = attr.dataName;
                object attrDefaultValue = attr.defaultValue;

                propertyDataNameDict.Add(propertys[i], new string[] { attrGroupName, attrDataName });
                propertyDefaultValueDict.Add(propertys[i], attrDefaultValue);

            }

            for (int i = 0; i < fields.Length; i++)
            {
                var attr = fields[i].GetFirstAttribute<LoadableAttribute>(true);
                if (attr == null)
                    continue;

                string attrGroupName = attr.groupName == "default" ? groupName : attr.groupName;
                string attrDataName = attr.dataName;
                object attrDefaultValue = attr.defaultValue;

                fieldDataNameDict.Add(fields[i], new string[] { attrGroupName, attrDataName });
                fieldDefaultValueDict.Add(fields[i], attrDefaultValue);
            }

            for (int i = 0; i < methods.Length; i++)
            {
                var attr = methods[i].GetFirstAttribute<LoadableAttribute>(true);
                if (attr == null)
                    continue;

                string attrGroupName = attr.groupName == "default" ? groupName : attr.groupName;
                string attrDataName = attr.dataName;
                object attrDefaultValue = attr.defaultValue;

                methodDataNameDict.Add(methods[i], new string[] { attrGroupName, attrDataName });
            }
        }
    }

}
