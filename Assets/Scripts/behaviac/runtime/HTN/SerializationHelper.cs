using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace behaviac
{
    /// <summary>
    /// Used to serialize and deserialize data to/from byte arrays.
    /// This class attempts to provide a lot of flexibility in
    /// what types of data can be stored, including arrays and
    /// complex types.
    /// </summary>
    public sealed class SerializationHelper : IDisposable
    {
        #region Static data

        private static Dictionary<string, System.Type> typeCache = new Dictionary<string, System.Type>();

        #endregion Static data

        #region Private data

        private const uint HEADER_FLAG = 0xC0DEFEED;
        private const uint CURRENT_DATA_VERSION = 0x00002;

        private uint dataVersion;

        private MemoryStream buffer;
        private BinaryWriter writer;
        private BinaryReader reader;

        private List<UnityEngine.Object> unityObjects;
        private List<object> objectReferences;
        private List<string> stringReferences;

        #endregion Private data

        #region Constructor

        static SerializationHelper()
        {
            foreach(Assembly assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach(System.Type type in assembly.GetTypes())
                {
                    var isMappable =
                        !type.IsSpecialName &&
                        !type.IsAbstract &&
                        !type.IsInterface;

                    if (isMappable)
                    {
                        typeCache[type.FullName] = type;
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new BinaryStore object for saving data to a byte array
        /// </summary>
        public SerializationHelper(List<UnityEngine.Object> unityObjects)
        {
            this.buffer = new MemoryStream();
            this.writer = new BinaryWriter(this.buffer);

            this.unityObjects = unityObjects;
            this.objectReferences = new List<object>();
            this.stringReferences = new List<string>();

            writeInfoHeader();
        }

        /// <summary>
        /// Initializes a new BinaryStore object for loading saved data
        /// </summary>
        /// <param name="data"></param>
        public SerializationHelper(byte[] data, List<UnityEngine.Object> unityObjects)
        {
            this.buffer = new MemoryStream(data);
            this.reader = new BinaryReader(this.buffer);

            this.unityObjects = unityObjects;
            this.objectReferences = new List<object>();
            this.stringReferences = new List<string>();

            readInfoHeader();
        }

        #endregion Constructor

        #region Public properties

        /// <summary>
        /// Returns TRUE if the read buffer is positioned at the end of the available data
        /// </summary>
        public bool EndOfFile
        {
            get
            {
                if (reader == null)
                {
                    return false;
                }

                else
                { return (this.buffer.Position >= this.buffer.Length); }
            }
        }

        #endregion Public properties

        #region Public methods

        /// <summary>
        /// Returns a byte array containing all data saved via the Write() method
        /// </summary>
        public byte[] GetBuffer()
        {
            var data = new byte[this.buffer.Length];
            Array.Copy(this.buffer.GetBuffer(), data, data.Length);
            return data;
        }

        /// <summary>
        /// Read the next stored value in the buffer
        /// </summary>
        public object Read()
        {
            var typeName = readTypeName();

            if (string.IsNullOrEmpty(typeName))
            {
                Debug.LogError("Failed to read type name");
                return null;
            }

            switch (typeName)
            {
                case TypeNames.NULL:
                    return null;

                case TypeNames.UnityObject:
                    var index = reader.ReadInt32();
                    return unityObjects[index];

                case TypeNames.SystemType:
                    var storedTypeName = reader.ReadString();
                    return GetType(storedTypeName);

                case TypeNames.Delegate:
                    return readDelegate();

                case TypeNames.MethodInfo:
                    return readMethodInfo();

                case TypeNames.Float:
                    return reader.ReadSingle();

                case TypeNames.Double:
                    return reader.ReadDouble();

                case TypeNames.Int:
                    return reader.ReadInt32();

                case TypeNames.Bool:
                    return reader.ReadBoolean();

                case TypeNames.Vector3:
                    var vector = new Vector3();
                    vector.x = reader.ReadSingle();
                    vector.y = reader.ReadSingle();
                    vector.z = reader.ReadSingle();
                    return vector;

                case TypeNames.Rect:
                    var rect = new Rect();
                    rect.xMin = reader.ReadSingle();
                    rect.yMin = reader.ReadSingle();
                    rect.xMax = reader.ReadSingle();
                    rect.yMax = reader.ReadSingle();
                    return rect;

                case TypeNames.Array:
                {
                    var elementTypeName = readTypeName();
                    var elementType = GetType(elementTypeName);
                    return readArray(elementType);
                }

                case TypeNames.TypedList:
                {
                    var elementTypeName = readTypeName();
                    var elementType = GetType(elementTypeName);
                    return readTypedList(elementType);
                }

                case TypeNames.ObjectRef:
                    // If an object reference was stored rather than the object's data, look up
                    // the object and return a reference.
                    var referenceIndex = reader.ReadInt32();
                    return objectReferences[referenceIndex];
            }

            var storedType = GetType(typeName);

            if (storedType == null)
            {
                // Notify the developer that a type which was previously serialized
                // cannot be deserialized
                Debug.LogError("Serialized type not found: " + typeName);

                // Naive attempt to allow deserialization to continue. This may not work,
                // but will not be worse than not attempting it.
                Read();

                // Could not properly deserialize the value, so return NULL to the caller.
                return null;
            }

            return readValueInternal(storedType);
        }

        /// <summary>
        /// Write the value to the buffer
        /// </summary>
        public void Write(object value)
        {
            if (value == null)
            {
                writeTypeName(TypeNames.NULL);
                return;
            }

            // Special handling for UnityEngine.Object
            if (value is UnityEngine.Object)
            {
                writeUnityObject(value);
                return;
            }

            // Special handling for System.Type
            if (value is System.Type)
            {
                writeTypeName(TypeNames.SystemType);
                writer.Write(((System.Type)value).FullName);
                return;
            }

            // Special handling for delegates
            if (value is Delegate)
            {
                writeDelegate(value);
                return;
            }

            // Special handling for System.Reflection.MethodInfo
            if (value is MethodInfo)
            {
                writeMethodInfo(value);
                return;
            }

            #region Special handling for most-common primitive types and structures

            if (value is float)
            {
                writeTypeName(TypeNames.Float);
                writer.Write((float)value);
                return;
            }
            else if (value is double)
            {
                writeTypeName(TypeNames.Double);
                writer.Write((double)value);
                return;
            }
            else if (value is int)
            {
                writeTypeName(TypeNames.Int);
                writer.Write((int)value);
                return;
            }
            else if (value is bool)
            {
                writeTypeName(TypeNames.Bool);
                writer.Write((bool)value);
                return;
            }
            else if (value is Vector3)
            {
                var vector = (Vector3)value;
                writeTypeName(TypeNames.Vector3);
                writer.Write(vector.x);
                writer.Write(vector.y);
                writer.Write(vector.z);
                return;
            }
            else if (value is Rect)
            {
                var rect = (Rect)value;
                writeTypeName(TypeNames.Rect);
                writer.Write(rect.xMin);
                writer.Write(rect.yMin);
                writer.Write(rect.xMax);
                writer.Write(rect.yMax);
                return;
            }
            else if (value is Array)
            {
                writeArray((Array)value);
                return;
            }
            else if (typeof(List<>).IsAssignableFrom(value.GetType()))
            {
                writeTypedList((IList)value);
                return;
            }

            #endregion Special handling for most-common primitive types and structures

            var valueType = value.GetType();

            // Keep track of object references to enable the above lookup
            if (valueType.IsClass)
            {
                var referenceIndex = objectReferences.IndexOf(value);

                if (referenceIndex != -1)
                {
                    writeTypeName(TypeNames.ObjectRef);
                    writer.Write((Int32)referenceIndex);

                    return;
                }
            }

            // Store the name of the type so that it can be properly deserialized later
            writeTypeName(valueType.FullName);

            // Allow custom serialization to run before storing object
            if (value is ISerializationCallbackReceiver)
            {
                ((ISerializationCallbackReceiver)value).OnBeforeSerialize();
            }

            if (valueType.IsEnum)
            {
                writeEnumeration(value);
                return;
            }

            if (valueType.IsPrimitive || valueType == typeof(string))
            {
                writer.Write(value.ToString());
                return;
            }

            if (value is IDictionary)
            {
                writeDictionary(value);
                return;
            }

            if (value is IList)
            {
                writeUntypedList(value);
                return;
            }

            writeObjectStructure(value, valueType);
        }

        #endregion Public methods

        #region Private utility methods

        private void writeInfoHeader()
        {
            writer.Write(HEADER_FLAG);
            writer.Write(CURRENT_DATA_VERSION);
        }

        private void readInfoHeader()
        {
            var flag = reader.ReadUInt32();

            if (flag != HEADER_FLAG)
            {
                reader.BaseStream.Position = 0;
                this.dataVersion = 0;

                return;
            }

            this.dataVersion = reader.ReadUInt32();
        }

        private void writeTypeName(string type)
        {
            int index = stringReferences.IndexOf(type);

            if (index == -1)
            {
                index = stringReferences.Count;
                stringReferences.Add(type);

                writer.Write(index);
                writer.Write(type);
            }
            else
            {
                writer.Write(index);
            }
        }

        private string readTypeName()
        {
            if (dataVersion == 0)
            {
                return reader.ReadString();
            }

            var index = reader.ReadInt32();

            if (index < stringReferences.Count)
            {
                return stringReferences[index];
            }

            var typeName = reader.ReadString();
            stringReferences.Add(typeName);

            return typeName;
        }

        private object readValueInternal(Type valueType)
        {
            try
            {
                if (valueType.IsEnum)
                {
                    return readEnumeration(valueType);
                }

                if (valueType.IsPrimitive || valueType == typeof(string))
                {
                    var valueAsString = reader.ReadString();
                    return Convert.ChangeType(valueAsString, valueType);
                }

                if (typeof(IDictionary).IsAssignableFrom(valueType))
                {
                    return readDictionary(valueType);
                }

                if (valueType.IsArray || typeof(IList).IsAssignableFrom(valueType))
                {
                    return readList(valueType);
                }

                return readObjectStructure(valueType);
            }
            catch (Exception err)
            {
                Debug.LogError(err);

                if (valueType.IsPrimitive)
                {
                    return Activator.CreateInstance(valueType);
                }

                else
                {
                    return null;
                }
            }
        }

        private object readObjectStructure(Type valueType)
        {
            var objValue = Activator.CreateInstance(valueType);

            // Keep track of all deserialized objects so that the list
            // is in sync with the indices stored by the Write() function.
            objectReferences.Add(objValue);

            #region Read object fields

            var fieldCount = reader.ReadInt32();

            for (int i = 0; i < fieldCount; i++)
            {
                var fieldName = reader.ReadString();
                var fieldValue = Read();

                var field = valueType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (field == null)
                {
                    UnityEngine.Debug.LogError(string.Format("Field not defined: {0}.{1}", valueType, fieldName));
                    continue;
                }

                field.SetValue(objValue, fieldValue);
            }

            #endregion Read object fields

            #region Read object properties

            var propertyCount = reader.ReadInt32();

            for (int i = 0; i < propertyCount; i++)
            {
                var propertyName = reader.ReadString();
                var propertyValue = Read();

                var property = valueType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

                if (property == null)
                {
                    UnityEngine.Debug.LogError(string.Format("Property not defined: {0}.{1}", valueType, propertyName));
                    continue;
                }

                property.SetValue(objValue, propertyValue, new object[] { });
            }

            #endregion Read object properties

            // Allow custom deserialization code to run
            if (objValue is ISerializationCallbackReceiver)
            {
                ((ISerializationCallbackReceiver)objValue).OnAfterDeserialize();
            }

            return objValue;
        }

        private void writeObjectStructure(object instance, Type instanceType)
        {
            objectReferences.Add(instance);

            var fields = instanceType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                         .Where(f =>
                                !f.IsLiteral &&
                                !f.IsNotSerialized &&
                                !f.IsSpecialName &&
                                !f.IsFamily &&
                                !f.IsDefined(typeof(System.NonSerializedAttribute), true) &&
                                (f.IsPublic || f.IsDefined(typeof(SerializeField), false))
                               )
                         .ToList();

            writer.Write(fields.Count);
            foreach(var field in fields)
            {
                writer.Write(field.Name);
                Write(field.GetValue(instance));
            }

            var properties = instanceType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                             .Where(p =>
                                    p.CanRead &&
                                    p.CanWrite &&
                                    !p.IsSpecialName &&
                                    p.GetIndexParameters().Length == 0
                                   )
                             .ToList();

            var indexParam = new object[] { };

            writer.Write(properties.Count);
            foreach(var property in properties)
            {
                writer.Write(property.Name);
                Write(property.GetValue(instance, indexParam));
            }
        }

        private void writeUntypedList(object value)
        {
            var list = ((IEnumerable)value).Cast<object>().ToList();

            writer.Write(list.Count);
            foreach(var element in list)
            {
                Write(element);
            }
        }

        private void writeTypedList(IList list)
        {
            var genericArguments = list.GetType().GetGenericArguments();

            if (genericArguments == null || genericArguments.Length == 0)
            {
                writeUntypedList(list);
                return;
            }

            var elementType = genericArguments[0];

            writeTypeName(TypeNames.TypedList);
            writeTypeName(elementType.FullName);
            writer.Write(list.Count);

            for (int i = 0; i < list.Count; i++)
            {
                Write(list[i]);
            }
        }

        private void writeArray(Array array)
        {
            writeTypeName(TypeNames.Array);
            writeTypeName(array.GetType().GetElementType().FullName);

            writer.Write(array.Length);

            for (int i = 0; i < array.Length; i++)
            {
                Write(array.GetValue(i));
            }
        }

        private void writeDictionary(object value)
        {
            var dict = (IDictionary)value;

            writer.Write(dict.Count);
            foreach(var key in dict.Keys)
            {
                Write(key);
                Write(dict[key]);
            }
        }

        private void writeEnumeration(object value)
        {
            writer.Write((int)value);
        }

        private void writeMethodInfo(object value)
        {
            var method = (MethodInfo)value;

            writeTypeName(TypeNames.MethodInfo);
            writer.Write(method.DeclaringType.FullName);
            writer.Write(method.Name);
        }

        private void writeDelegate(object value)
        {
            var func = (Delegate)value;
            var method = func.Method;

            writeTypeName(TypeNames.Delegate);
            writer.Write(value.GetType().FullName);
            writer.Write(method.DeclaringType.FullName);
            writer.Write(method.Name);
        }

        private void writeUnityObject(object value)
        {
            writeTypeName(TypeNames.UnityObject);

            var unityObj = (UnityEngine.Object)value;
            var index = unityObjects.IndexOf(unityObj);

            if (index != -1)
            {
                writer.Write(index);
                return;
            }

            unityObjects.Add(unityObj);
            writer.Write(unityObjects.Count - 1);
        }

        private object readTypedList(Type elementType)
        {
            var listType = typeof(List<>).MakeGenericType(elementType);
            var list = Activator.CreateInstance(listType) as IList;

            var length = reader.ReadInt32();

            for (int i = 0; i < length; i++)
            {
                var item = Read();
                list.Add(item);
            }

            return list;
        }

        private object readArray(Type elementType)
        {
            var length = reader.ReadInt32();
            var array = Array.CreateInstance(elementType, length);

            for (int i = 0; i < length; i++)
            {
                var elementValue = Read();
                array.SetValue(elementValue, i);
            }

            return array;
        }

        private object readList(Type listType)
        {
            var length = reader.ReadInt32();
            var array = (IList)Activator.CreateInstance(listType, length);

            for (int i = 0; i < length; i++)
            {
                var elementValue = Read();

                if (listType.IsArray)
                {
                    array[i] = elementValue;
                }

                else
                {
                    array.Add(elementValue);
                }
            }

            if (array is ISerializationCallbackReceiver)
            {
                ((ISerializationCallbackReceiver)array).OnAfterDeserialize();
            }

            return array;
        }

        private object readDictionary(Type valueType)
        {
            var count = reader.ReadInt32();
            var dict = (IDictionary)Activator.CreateInstance(valueType);

            for (int i = 0; i < count; i++)
            {
                var key = Read();
                dict[key] = Read();
            }

            if (dict is ISerializationCallbackReceiver)
            {
                ((ISerializationCallbackReceiver)dict).OnAfterDeserialize();
            }

            return dict;
        }

        private object readEnumeration(Type valueType)
        {
            try
            {
                if (dataVersion == CURRENT_DATA_VERSION)
                {
                    return Enum.ToObject(valueType, (object)reader.ReadInt32());
                }

                else
                {
                    return Enum.Parse(valueType, reader.ReadString());
                }
            }
            catch
            {
                return Enum.ToObject(valueType, (object)0);
            }
        }

        private object readMethodInfo()
        {
            var declaringType = reader.ReadString();
            var methodName = reader.ReadString();

            var type = GetType(declaringType);

            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' could not be found", declaringType));
            }

            return type.GetMethod(methodName);
        }

        private object readDelegate()
        {
            var delegateType = GetType(reader.ReadString());
            var declaringType = GetType(reader.ReadString());
            var method = declaringType.GetMethod(reader.ReadString());

            return Delegate.CreateDelegate(delegateType, method);
        }

        private static Type GetType(string typeName)
        {
            // Ensure that an empty type name was not passed
            if (string.IsNullOrEmpty(typeName))
            {
                throw new ArgumentException("TypeName cannot be empty or null");
            }

            // Check the type map first
            Type type = null;

            if (typeCache.TryGetValue(typeName, out type))
            {
                return type;
            }

            // Try Type.GetType() first. This will work with types defined
            // by the Mono runtime, in the same assembly as the caller, etc.
            type = Type.GetType(typeName);

            // If it worked, then we're done here
            if (type != null)
            {
                return typeCache[typeName] = type;
            }

            // If the TypeName is a full name, then we can try loading the defining assembly directly
            if (typeName.Contains("."))
            {
                try
                {
                    // Get the name of the assembly (Assumption is that we are using
                    // fully-qualified type names)
                    var assemblyName = typeName.Substring(0, typeName.IndexOf('.'));

                    // Attempt to load the indicated Assembly
                    var assembly = Assembly.Load(assemblyName);

                    if (assembly == null)
                    {
                        return null;
                    }

                    // Ask that assembly to return the proper Type
                    type = assembly.GetType(typeName);

                    if (type != null)
                    {
                        return typeCache[typeName] = type;
                    }
                }
                catch (Exception err)
                {
                    Debug.LogError(err);
                }
            }

            // If we still haven't found the proper type, we can enumerate all of the
            // loaded assemblies and see if any of them define the type
            var currentAssembly = Assembly.GetExecutingAssembly();
            var referencedAssemblies = currentAssembly.GetReferencedAssemblies();
            foreach(var assemblyName in referencedAssemblies)
            {
                // Load the referenced assembly
                var assembly = Assembly.Load(assemblyName);

                if (assembly != null)
                {
                    // See if that assembly defines the named type
                    type = assembly.GetType(typeName);

                    if (type != null)
                    {
                        return typeCache[typeName] = type;
                    }
                }
            }

            // The type just couldn't be found...
            return null;
        }

        #endregion Private utility methods

        #region IDisposable Members

        public void Dispose()
        {
            // NOTE: For whatever reason, the Mono implementation of the BinaryReader and
            // BinaryWriter classes has chosen to keep the access to the Dispose() method
            // hidden unless the object is explicitly cast to IDisposable first.

            if (this.writer != null)
            {
                ((IDisposable)this.writer).Dispose();
                writer = null;
            }

            if (this.reader != null)
            {
                ((IDisposable)this.reader).Dispose();
                this.reader = null;
            }

            if (buffer != null)
            {
                this.buffer.Dispose();
                this.buffer = null;
            }
        }

        #endregion IDisposable Members

        #region Nested types

        private class TypeNames
        {
            public const string NULL = "null";
            public const string UnityObject = "UnityEngine.Object";
            public const string SystemType = "System.Type";
            public const string Delegate = "Delegate";
            public const string MethodInfo = "MethodInfo";
            public const string Float = "float";
            public const string Double = "double";
            public const string Int = "int";
            public const string Bool = "bool";
            public const string ObjectRef = "ObjectReference";
            public const string Vector3 = "vector3";
            public const string Rect = "rect";
            public const string Array = "array";
            public const string TypedList = "list";
        }

        #endregion Nested types
    }
}
