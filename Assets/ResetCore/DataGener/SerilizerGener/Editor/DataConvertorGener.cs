using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ResetCore.CodeDom;

namespace ResetCore.Data
{

    public class DataConvertorGener
    {
        public void GenDataSerialize(Type type, string outPath)
        {
            CodeGener gener = new CodeGener("ResetCore.Data.Serializer", type.Name + "Serializer");
            gener.AddImport("ResetCore.Data")
                .AddBaseType("IDataSerialize");
            
            

            gener.GenCSharp(outPath);

        }

       
    }
}


