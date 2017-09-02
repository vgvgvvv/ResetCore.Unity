﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using ResetCore.Util;
using UnityEngine;
using UnityEngine.Networking;

namespace ResetCore.Data
{
    public class SerializedAttribute : Attribute
    {
        public readonly string name;
        public readonly string defaultValue;
        public readonly int count = 0;
        public readonly bool forceExpend = false;

        public SerializedAttribute(string name, string defaultValue = null, int count = 0, bool forceExpend = false)
        {
            this.name = name;
            this.defaultValue = defaultValue;
            this.count = count;
            this.forceExpend = forceExpend;
        }
    }

    public class XmlUtil
    {

        /// <summary>
        /// 读取字符串，默认为子节点
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="name"></param>
        /// <param name="isAttr"></param>
        /// <returns></returns>
        public static string ReadString(XmlElement xml, string name, bool isAttr = false)
        {
            if (string.IsNullOrEmpty(name))
                return xml.InnerText;

            if (!isAttr)
            {
                var node = xml.SelectSingleNode(name);
                if (node == null)
                    throw new Exception("Attribute " + name + " not exist!");
                return node.InnerText;
            }

            var attribute = xml.Attributes[name];
            if (attribute == null)
                throw new Exception("Attribute " + name + " not exist!");

            return attribute.Value;
        }

        public static string ReadString(XmlElement xml, string name, string defaultValue)
        {
            try { return ReadString(xml, name); }
            catch { return defaultValue; }
        }

        #region 基本读取
        public static sbyte ReadSByte(XmlElement xml, string name)
        {
            return sbyte.Parse(ReadString(xml, name));
        }

        public static sbyte ReadSByte(XmlElement xml, string name, sbyte defaultValue)
        {
            try { return ReadSByte(xml, name); }
            catch { return defaultValue; }
        }

        public static byte ReadByte(XmlElement xml, string name)
        {
            return byte.Parse(ReadString(xml, name));
        }

        public static byte ReadByte(XmlElement xml, string name, byte defaultValue)
        {
            try { return ReadByte(xml, name); }
            catch { return defaultValue; }
        }

        public static short ReadInt16(XmlElement xml, string name)
        {
            return short.Parse(ReadString(xml, name));
        }

        public static short ReadInt16(XmlElement xml, string name, short defaultValue)
        {
            try { return ReadInt16(xml, name); }
            catch { return defaultValue; }
        }

        public static ushort ReadUInt16(XmlElement xml, string name)
        {
            return ushort.Parse(ReadString(xml, name));
        }

        public static ushort ReadUInt16(XmlElement xml, string name, ushort defaultValue)
        {
            try { return ReadUInt16(xml, name); }
            catch { return defaultValue; }
        }

        public static int ReadInt32(XmlElement xml, string name)
        {
            return int.Parse(ReadString(xml, name));
        }

        public static int ReadInt32(XmlElement xml, string name, int defaultValue)
        {
            try { return ReadInt32(xml, name); }
            catch { return defaultValue; }
        }

        public static uint ReadUInt32(XmlElement xml, string name)
        {
            return uint.Parse(ReadString(xml, name));
        }

        public static uint ReadUInt32(XmlElement xml, string name, uint defaultValue)
        {
            try { return ReadUInt32(xml, name); }
            catch { return defaultValue; }
        }

        public static long ReadInt64(XmlElement xml, string name)
        {
            return long.Parse(ReadString(xml, name));
        }

        public static long ReadInt64(XmlElement xml, string name, long defaultValue)
        {
            try { return ReadInt64(xml, name); }
            catch { return defaultValue; }
        }

        public static ulong ReadUInt64(XmlElement xml, string name)
        {
            return ulong.Parse(ReadString(xml, name));
        }

        public static ulong ReadUInt64(XmlElement xml, string name, ulong defaultValue)
        {
            try { return ReadUInt64(xml, name); }
            catch { return defaultValue; }
        }

        public static char ReadChar(XmlElement xml, string name)
        {
            return char.Parse(ReadString(xml, name));
        }

        public static char ReadChar(XmlElement xml, string name, char defaultValue)
        {
            try { return ReadChar(xml, name); }
            catch { return defaultValue; }
        }

        public static bool ReadBoolean(XmlElement xml, string name)
        {
            return bool.Parse(ReadString(xml, name));
        }

        public static bool ReadBoolean(XmlElement xml, string name, bool defaultValue)
        {
            try { return ReadBoolean(xml, name); }
            catch { return defaultValue; }
        }

        public static float ReadSingle(XmlElement xml, string name)
        {
            return float.Parse(ReadString(xml, name));
        }

        public static float ReadSingle(XmlElement xml, string name, float defaultValue)
        {
            try { return ReadSingle(xml, name); }
            catch { return defaultValue; }
        }

        public static double ReadDouble(XmlElement xml, string name)
        {
            return double.Parse(ReadString(xml, name));
        }

        public static double ReadDouble(XmlElement xml, string name, double defaultValue)
        {
            try { return ReadDouble(xml, name); }
            catch { return defaultValue; }
        }

        public static decimal ReadDecimal(XmlElement xml, string name)
        {
            return decimal.Parse(ReadString(xml, name));
        }

        public static decimal ReadDecimal(XmlElement xml, string name, decimal defaultValue)
        {
            try { return ReadDecimal(xml, name); }
            catch { return defaultValue; }
        }
        #endregion 基本读取

        public static object ReadAny(XmlElement xml, string name, Type type, object defaultValue)
        {
            var node = string.IsNullOrEmpty(name) ? xml : xml.SelectSingleNode(name);
            if (node.ChildNodes.Count != 0 && node.FirstChild is XmlText)
            {
                return node.InnerText.GetValue(type);
            }
            else if (type.IsArray)
            {
                return ReadArray(xml, name, type);
            }
            else if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>)))
            {
                return ReadList(xml, name, type);
            }
            else if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Dictionary<,>)))
            {
                return ReadDictionary(xml, name, type);
            }
            return null;
        }
        
        /// <summary>
        /// 读取列表
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object ReadList(XmlElement xml, string name, Type type)
        {
            var node = xml.SelectSingleNode(name);
            var addMethod = type.GetMethod("Add");
            System.Type genericArgType = type.GetGenericArguments()[0];

            var instance = Activator.CreateInstance(type);

            foreach (XmlElement nodeChildNode in node.ChildNodes)
            {
                var item = ReadAny(nodeChildNode, null, genericArgType, null);
                addMethod.Invoke(instance, new[] {item});
            }
            return instance;
        }

        /// <summary>
        /// 读取数组
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object ReadArray(XmlElement xml, string name, Type type)
        {
            var node = xml.SelectSingleNode(name);
            Type elementType = Type.GetType(
                type.FullName.Replace("[]", string.Empty));
            
            Array array = Array.CreateInstance(elementType, node.ChildNodes.Count);

            int i = 0;
            foreach (XmlElement nodeChildNode in node.ChildNodes)
            {
                var item = ReadAny(nodeChildNode, null, elementType, null);
                array.SetValue(item, i);
                i++;
            }
            return array;
        }

        /// <summary>
        /// 读取词典
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object ReadDictionary(XmlElement xml, string name, Type type)
        {
            var node = xml.SelectSingleNode(name);
            System.Type keyType = type.GetGenericArguments()[0];
            System.Type valueType = type.GetGenericArguments()[1];
            var addMethod = type.GetMethod("Add");
            var instance = type.GetConstructor(System.Type.EmptyTypes).Invoke(null);

            foreach (XmlElement nodeChildNode in node.ChildNodes)
            {
                var key = ReadAny(nodeChildNode, "Key", keyType, null);
                var value = ReadAny(nodeChildNode, "Value", valueType, null);
                if(key == null || value == null)
                    continue;
                
                addMethod.Invoke(instance, new[] {key, value});
            }

            return instance;
        }


        public static void Write(XmlElement xml, string name, string value, bool isAttr = false)
        {
            if (string.IsNullOrEmpty(name))
                xml.InnerText = value;
            else if (!isAttr)
            {
                var node = xml.OwnerDocument.CreateElement(name);
                xml.AppendChild(node);
                node.InnerText = value;
            }
            else
                xml.SetAttribute(name, value);
        }


        #region 基本类型写入

        public static void Write(XmlElement xml, string name, sbyte value)
        {
            Write(xml, name, value.ToString());
        }

        public static void Write(XmlElement xml, string name, byte value)
        {
            Write(xml, name, value.ToString());
        }

        public static void Write(XmlElement xml, string name, short value)
        {
            Write(xml, name, value.ToString());
        }

        public static void Write(XmlElement xml, string name, ushort value)
        {
            Write(xml, name, value.ToString());
        }

        public static void Write(XmlElement xml, string name, int value)
        {
            Write(xml, name, value.ToString());
        }

        public static void Write(XmlElement xml, string name, uint value)
        {
            Write(xml, name, value.ToString());
        }

        public static void Write(XmlElement xml, string name, long value)
        {
            Write(xml, name, value.ToString());
        }

        public static void Write(XmlElement xml, string name, ulong value)
        {
            Write(xml, name, value.ToString());
        }

        public static void Write(XmlElement xml, string name, char value)
        {
            Write(xml, name, value.ToString());
        }

        public static void Write(XmlElement xml, string name, bool value)
        {
            Write(xml, name, value.ToString());
        }

        public static void Write(XmlElement xml, string name, float value)
        {
            Write(xml, name, value.ToString());
        }

        public static void Write(XmlElement xml, string name, double value)
        {
            Write(xml, name, value.ToString());
        }

        public static void Write(XmlElement xml, string name, decimal value)
        {
            Write(xml, name, value.ToString());
        }

        #endregion 基本类型写入

        /// <summary>
        /// 通用写入
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="name"></param>
        /// <param name="val"></param>
        public static void WriteAny(XmlElement xml, string name, object val)
        {
            Type type = val.GetType();
            if (IsPrimitive(type))
            {
                Write(xml, name, val.ToString());
            }else if (type.IsArray)
            {
                WriteArray(xml, name, val);
            }else if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>)))
            {
                WriteList(xml, name, val);
            }else if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Dictionary<,>)))
            {
                WriteDictionary(xml, name, val);
            }
        }
        
        /// <summary>
        /// 写入数组
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="name"></param>
        /// <param name="array"></param>
        public static void WriteArray(XmlElement xml, string name, object array)
        {
            var node = xml.OwnerDocument.CreateElement(name);
            xml.AppendChild(node);

            Type type = array.GetType();
            Type elementType = Type.GetType(
                type.FullName.Replace("[]", string.Empty));

            node.SetAttribute("Type", elementType.FullName);

            var realArray = array as Array;

            for (int i = 0; i < realArray.Length; i++)
            {
                var item = xml.OwnerDocument.CreateElement("item");
                WriteAny(item, null, realArray.GetValue(i));
                node.AppendChild(item);
            }
        }

        /// <summary>
        /// 写入列表
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="name"></param>
        /// <param name="array"></param>
        public static void WriteList(XmlElement xml, string name, object array)
        {
            var node = xml.OwnerDocument.CreateElement(name);
            xml.AppendChild(node);

            Type type = array.GetType();
            Type elementType = type.GenericTypeArguments[0];

            node.SetAttribute("Type", elementType.FullName);

            int Count = (int)type.GetProperty("Count").GetValue(array, null);
            MethodInfo mget = type.GetMethod("get_Item", BindingFlags.Instance | BindingFlags.Public);


            object item;
            for (int i = 0; i < Count; i++)
            {
                var itemNode = xml.OwnerDocument.CreateElement("item");
                item = mget.Invoke(array, new object[] { i });
                WriteAny(itemNode, null, item);
                node.AppendChild(itemNode);
            }
        }

        /// <summary>
        /// 写入词典
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="name"></param>
        /// <param name="dictionry"></param>
        public static void WriteDictionary(XmlElement xml, string name, object dictionry)
        {
            var node = xml.OwnerDocument.CreateElement(name);
            xml.AppendChild(node);

            Type type = dictionry.GetType();
            Type keyType = type.GenericTypeArguments[0];
            Type valueType = type.GenericTypeArguments[1];

            node.SetAttribute("KeyType", keyType.FullName);
            node.SetAttribute("ValueType", valueType.FullName);

            MethodInfo getIe = type.GetMethod("GetEnumerator");
            object enumerator = getIe.Invoke(dictionry, new object[0]);
            System.Type enumeratorType = enumerator.GetType();
            MethodInfo moveToNextMethod = enumeratorType.GetMethod("MoveNext");
            PropertyInfo current = enumeratorType.GetProperty("Current");

            Type kvpType = null;
            PropertyInfo keyPropertyInfo = null;
            PropertyInfo valuePropertyInfo = null;
            while (enumerator != null && (bool)moveToNextMethod.Invoke(enumerator, new object[0]))
            {
                var itemNode = xml.OwnerDocument.CreateElement("item");
                node.AppendChild(itemNode);

                var kvp = current.GetValue(enumerator, null);
                if (kvpType == null)
                {
                    kvpType = kvp.GetType();
                    keyPropertyInfo = kvpType.GetProperty("Key");
                    valuePropertyInfo = kvpType.GetProperty("Value");
                }

                WriteAny(itemNode, "Key", keyPropertyInfo.GetValue(kvp, null));
                WriteAny(itemNode, "Value", valuePropertyInfo.GetValue(kvp, null));
            }

        }





        static bool IsPrimitive(Type type)
        {
            return type.IsPrimitive || type == typeof(string) || type.IsEnum;
        }

        //static bool IsExpend(Type type)
        //{
        //    if (IsPrimitive(type))
        //        return false;

        //    if (type.IsArray)
        //    {
        //        var elementType = type.GetElementType();
        //        return !IsPrimitive(elementType);
        //    }

        //    if (type.GetInterface("IList") != null)
        //    {
        //        var elementType = type.GetGenericArguments()[0];
        //        return !IsPrimitive(elementType);
        //    }

        //    if (StringUtil.GetParser(type) != null)
        //        return false;

        //    return true;
        //}

        //public static object FromXml(XmlElement xmlElement, Type type, string name = null, string defaultValue = null, bool forceExpend = false)
        //{
        //    if (!forceExpend && !IsExpend(type))
        //    {
        //        if (defaultValue == null)
        //            return Read(type, xmlElement, name);
        //        else
        //            return Read(type, xmlElement, name, defaultValue);
        //    }
        //    else if (IsPrimitive(type))
        //    {
        //        var xmlItem = xmlElement.SelectSingleNode(name) as XmlElement;
        //        if (defaultValue == null)
        //            return Read(type, xmlItem, name);
        //        else
        //            return Read(type, xmlItem, name, defaultValue);
        //    }
        //    else if (type.IsArray)
        //    {
        //        var itemType = type.GetElementType();
        //        var xmlItems = xmlElement.SelectNodes(name);
        //        Array array = Array.CreateInstance(itemType, xmlItems.Count);
        //        int i = 0;
        //        foreach (XmlElement xmlItem in xmlItems)
        //        {
        //            array.SetValue(FromXml(xmlItem, itemType), i++);
        //        }
        //        return array;
        //    }
        //    else if (type.GetInterface("IList") != null)
        //    {
        //        var itemType = type.GetGenericArguments()[0];
        //        var xmlItems = xmlElement.SelectNodes(name);
        //        var list = Activator.CreateInstance(type, xmlItems.Count) as IList;
        //        foreach (XmlElement xmlItem in xmlItems)
        //        {
        //            list.Add(FromXml(xmlItem, itemType));
        //        }
        //        return list;
        //    }
        //    else
        //    {
        //        var xmlItem = xmlElement;
        //        if (!string.IsNullOrEmpty(name))
        //        {
        //            xmlItem = xmlElement.SelectSingleNode(name) as XmlElement;
        //            if (xmlItem == null)
        //                return GetDefault(type);
        //        }

        //        var obj = Activator.CreateInstance(type);
        //        var fields = type.GetFields();
        //        foreach (var field in fields)
        //        {
        //            if (field.IsStatic)
        //                continue;

        //            bool serializable = true;
        //            if (field.IsPublic)
        //            {
        //                object[] nonSerializeds = field.GetCustomAttributes(typeof(NonSerializedAttribute), false);
        //                serializable = nonSerializeds == null || nonSerializeds.Length == 0;
        //            }
        //            else
        //            {
        //                object[] serializeFields = field.GetCustomAttributes(typeof(UnityEngine.SerializeField), false);
        //                serializable = serializeFields != null && serializeFields.Length > 0;
        //            }

        //            if (serializable)
        //            {
        //                string fieldName = field.Name;
        //                string fieldDefault = null;
        //                bool fieldForceExpend = false;

        //                object[] serializeds = field.GetCustomAttributes(typeof(SerializedAttribute), false);
        //                if (serializeds != null && serializeds.Length > 0)
        //                {
        //                    SerializedAttribute serialized = serializeds[0] as SerializedAttribute;
        //                    fieldName = serialized.name;
        //                    fieldDefault = serialized.defaultValue;
        //                    fieldForceExpend = serialized.forceExpend;
        //                }

        //                field.SetValue(obj, FromXml(xmlItem, field.FieldType, fieldName, fieldDefault, fieldForceExpend));
        //            }
        //        }
        //        return obj;
        //    }
        //}

        //public static object GetDefault(Type type)
        //{
        //    if (type.IsValueType)
        //        return Activator.CreateInstance(type);
        //    else
        //        return null;
        //}

        //public static void ToXml(XmlElement xmlElement, object obj, string name = null, bool forceExpend = false)
        //{
        //    var type = obj.GetType();
        //    if (!forceExpend && !IsExpend(type))
        //        Write(xmlElement, name, obj.ConverToString());
        //    else if (IsPrimitive(type))
        //    {
        //        var xmlItem = xmlElement.OwnerDocument.CreateElement(name);
        //        xmlElement.AppendChild(xmlItem);
        //        Write(xmlItem, null, obj.ConverToString());
        //    }
        //    else if (type.IsArray)
        //    {
        //        var array = obj as Array;
        //        foreach (var item in array)
        //        {
        //            var xmlItem = xmlElement.OwnerDocument.CreateElement(name);
        //            xmlElement.AppendChild(xmlItem);

        //            ToXml(xmlItem, item, null);
        //        }
        //    }
        //    else if (type.GetInterface("IList") != null)
        //    {
        //        var list = obj as IList;
        //        foreach (var item in list)
        //        {
        //            var xmlItem = xmlElement.OwnerDocument.CreateElement(name);
        //            xmlElement.AppendChild(xmlItem);

        //            ToXml(xmlItem, item, null);
        //        }
        //    }
        //    else
        //    {
        //        var xmlItem = xmlElement;
        //        if (!string.IsNullOrEmpty(name))
        //        {
        //            xmlItem = xmlElement.OwnerDocument.CreateElement(name);
        //            xmlElement.AppendChild(xmlItem);
        //        }

        //        var fields = type.GetFields();
        //        foreach (var field in fields)
        //        {
        //            if (field.IsStatic)
        //                continue;

        //            bool serializable = true;
        //            if (field.IsPublic)
        //            {
        //                object[] nonSerializeds = field.GetCustomAttributes(typeof(NonSerializedAttribute), false);
        //                serializable = nonSerializeds == null || nonSerializeds.Length == 0;
        //            }
        //            else
        //            {
        //                object[] serializeFields = field.GetCustomAttributes(typeof(UnityEngine.SerializeField), false);
        //                serializable = serializeFields != null && serializeFields.Length > 0;
        //            }

        //            if (serializable)
        //            {
        //                string fieldName = field.Name;

        //                object[] serializeds = field.GetCustomAttributes(typeof(SerializedAttribute), false);
        //                if (serializeds != null && serializeds.Length > 0)
        //                {
        //                    SerializedAttribute serialized = serializeds[0] as SerializedAttribute;
        //                    fieldName = serialized.name;
        //                }

        //                ToXml(xmlItem, field.GetValue(obj), fieldName);
        //            }
        //        }
        //    }
        //}
    }

}