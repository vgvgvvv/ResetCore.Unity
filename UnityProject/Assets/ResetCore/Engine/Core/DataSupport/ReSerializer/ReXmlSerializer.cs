using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using ResetCore.Util;
using UnityEngine;
using UnityEngine.Networking;

namespace ResetCore.Data
{
    public class ReXmlSerializer
    {

        /// <summary>
        /// 从XmlDocument中读取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xDoc"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T ReadFromXmlDocument<T>(XmlDocument xDoc, string name)
        {
            var Root = xDoc["Root"];
            var res = (T)ReXmlSerializer.ReadAny(Root, "TestClass", typeof(T), null);
            return res;
        }

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

        /// <summary>
        /// 读取任何
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static object ReadAny(XmlElement xml, string name, Type type, object defaultValue)
        {
            var node = string.IsNullOrEmpty(name) ? xml : xml.SelectSingleNode(name);
            if (IsPrimitive(type))
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
            else
            {
                return ReadObject(xml, name, type);
            }
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
            var node = name == null? xml : xml.SelectSingleNode(name);
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
            var node = name == null ? xml : xml.SelectSingleNode(name);
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
            var node = name == null ? xml : xml.SelectSingleNode(name);
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

        /// <summary>
        /// 读取对象
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object ReadObject(XmlElement xml, string name, Type type)
        {
            var node = name == null ? xml : xml.SelectSingleNode(name);

            var obj = Activator.CreateInstance(type);

            foreach (XmlElement nodeChildNode in node.ChildNodes)
            {
                var field = type.GetField(nodeChildNode.LocalName);
                if (field != null && field.GetCustomAttribute<SerializeIgnore>() == null)
                {
                    var value = ReadAny(nodeChildNode, null, field.FieldType, null);
                    field.SetValue(obj, value);
                }

                var property = type.GetProperty(nodeChildNode.LocalName);
                if (property != null && property.GetCustomAttribute<SerializeIgnore>() == null)
                {
                    var value = ReadAny(nodeChildNode, null, property.PropertyType, null);
                    property.SetValue(obj, value);
                }
            }
            return obj;
        }


       /// <summary>
       /// 写入文档
       /// </summary>
       /// <param name="name"></param>
       /// <param name="value"></param>
       /// <returns></returns>
        public static XmlDocument WriteToXmlDocument(string name, object value)
        {
            XmlDocument xDoc = new XmlDocument();
            var Root = xDoc.CreateElement("Root");
            xDoc.AppendChild(Root);
            ReXmlSerializer.WriteAny(Root, name, value);
            return xDoc;
        }

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="isAttr"></param>
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

        public static void Write(XmlElement xml, string name, Vector2 value)
        {
            Write(xml, name, StringEx.Vector2ToString(value));
        }

        public static void Write(XmlElement xml, string name, Vector3 value)
        {
            Write(xml, name, StringEx.Vector3ToString(value));
        }

        public static void Write(XmlElement xml, string name, Vector4 value)
        {
            Write(xml, name, StringEx.Vector4ToString(value));
        }

        public static void Write(XmlElement xml, string name, Quaternion value)
        {
            Write(xml, name, StringEx.QuaternionToString(value));
        }

        public static void Write(XmlElement xml, string name, Color value)
        {
            Write(xml, name, StringEx.ColorToString(value));
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
            if (val == null)
            {
                return;
            }
            Type type = val.GetType();
            if (IsPrimitive(type))
            {
                Write(xml, name, val.ConverToString());
            }
            else if (type.IsArray)
            {
                WriteArray(xml, name, val);
            }else if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>)))
            {
                WriteList(xml, name, val);
            }else if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Dictionary<,>)))
            {
                WriteDictionary(xml, name, val);
            }
            else
            {
                WriteObejct(xml, name, val);
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
                WriteAny(node, "item", realArray.GetValue(i));
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
                item = mget.Invoke(array, new object[] { i });
                WriteAny(node, "item", item);
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

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        public static void WriteObejct(XmlElement xml, string name, object obj)
        {
            var node = xml.OwnerDocument.CreateElement(name);
            xml.AppendChild(node);

            Type type = obj.GetType();

            var fields = type.GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                var currentField = fields[i];
                if (currentField.GetCustomAttribute<SerializeIgnore>() != null)
                    continue;
                WriteAny(node, currentField.Name, currentField.GetValue(obj));
            }

            var properties = type.GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                var currentProperty = properties[i];
                if (currentProperty.GetCustomAttribute<SerializeIgnore>() != null)
                    continue;
                WriteAny(node, currentProperty.Name, currentProperty.GetValue(obj, null));
            }

        }

        /// <summary>
        /// 是否为Xml对象中的基本类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsPrimitive(Type type)
        {
            return type.IsPrimitive || type == typeof(string) || type.IsEnum
                || type == typeof(Vector2) || type == typeof(Vector3) || type == typeof(Vector4)
                   || type == typeof(Quaternion) || type == typeof(Color);
        }

    }

}
