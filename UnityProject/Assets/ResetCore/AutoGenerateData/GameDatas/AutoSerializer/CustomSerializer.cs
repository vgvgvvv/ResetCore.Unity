using System;
using System.Collections;
using System.Collections.Generic;
using ResetCore.Data.AutoSerializer;
using UnityEngine;

namespace ResetCore.Data
{
    public class CustomSerializer
    {
        private static readonly Dictionary<Type, object> SerializerDict = new Dictionary<Type, object>()
        {
            {typeof(TestSerializeData), new TestSerializeData_Serializer() }
        };

        public static IXmlSerializer<T> GetSerializer<T>()
        {
            return SerializerDict[typeof(T)] as IXmlSerializer<T>;
        }

        
    }
}
