using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ResetCore.CodeDom;
using ResetCore.Util;
using ResetCore.Data;
using System;
using System.Reflection;
using System.Linq;
using System.IO;
using ResetCore.Data.GameDatas.Protobuf;

namespace ResetCore.Data
{
    public class Source2Protobuf : ISource2
    {
        public DataType dataType
        {
            get
            {
                return DataType.Normal;
            }
        }

        public void GenData(IDataReadable reader, string outputPath = null)
        {
            string className = reader.currentDataTypeName;
            Type protobufDataType = Type.GetType(ProtobufData.nameSpace + "." + className + ",Assembly-CSharp");
            //Debug.Log(ProtobufData.nameSpace + "." + className + ",Assembly-CSharp" + " 是否存在" + protobufDataType.ToString());

            if (protobufDataType == null)
            {
                GenCS(reader);
                protobufDataType = Type.GetType(ProtobufData.nameSpace + "." + className + ",Assembly-CSharp");
                Debug.unityLogger.Log("Gen the CS File Please");
            }

            List<Dictionary<string, object>> rowObjs = reader.GetRowObjs();

            List<object> result = new List<object>();
            for (int i = 0; i < rowObjs.Count; i++)
            {
                object item = Activator.CreateInstance(protobufDataType);
                PropertyInfo[] propertys = protobufDataType.GetProperties();
                foreach (KeyValuePair<string, object> pair in rowObjs[i])
                {
                    PropertyInfo prop = propertys.First((pro) => { return pro.Name == pair.Key; });
                    prop.SetValue(item, pair.Value, null);
                    Debug.unityLogger.Log(pair.ConverToString());
                }

                result.Add(item);
            }

            string protoPath = PathConfig.GetLocalGameDataPath(PathConfig.DataType.Protobuf);
            PathEx.MakeFileDirectoryExist(protoPath);

            if (ProtoBuf.Serializer.NonGeneric.CanSerialize(protobufDataType))
            {
                string resPath = protoPath + className + ProtobufData.ex;
                if(outputPath != null)
                {
                    resPath = outputPath;
                }
                string root = Path.GetDirectoryName(resPath);
                PathEx.MakeFileDirectoryExist(root);
                using (var file = System.IO.File.Create(resPath))
                {
                    ProtoBuf.Serializer.NonGeneric.Serialize(file, result);
                    Debug.unityLogger.Log(resPath + "导出成功");
                }

            }
            else
            {
                Debug.unityLogger.LogError("序列化", protobufDataType.FullName + "不可序列化！");
            }



        }

        public void GenCS(IDataReadable reader)
        {


            string className = reader.currentDataTypeName;

            CodeGener protobufBaseGener = new CodeGener(ProtobufData.nameSpace, className);
            protobufBaseGener.newClass.AddMemberCostomAttribute("ProtoBuf.ProtoContract");
            protobufBaseGener.AddImport("System", "ProtoBuf");

            List<string> comment = reader.GetComment();

            protobufBaseGener
                .AddBaseType("ProtobufData<" + className + ">")
                .AddMemberField(typeof(string), "fileName", (member) =>
                {
                    member.AddFieldMemberInit("\"" + className + "\"");
                }, System.CodeDom.MemberAttributes.Static | System.CodeDom.MemberAttributes.Final | System.CodeDom.MemberAttributes.Public);

            reader.GetTitle().ForEach((i, title) =>
            {
                string[] titleSplit = title.Split('|');
                string varName = titleSplit[0];
                string typeName = titleSplit[1];
                protobufBaseGener.AddMemberProperty(typeName.GetTypeByString(), varName, (member) =>
                {
                    member.AddComment(comment[i], true);
                    member.AddMemberCostomAttribute("ProtoBuf.ProtoMember", (i+1).ToString());
                });
            });
            PathEx.MakeFileDirectoryExist(PathConfig.GetLoaclGameDataClassPath(PathConfig.DataType.Protobuf));
            protobufBaseGener.GenCSharp(PathConfig.GetLoaclGameDataClassPath(PathConfig.DataType.Protobuf));

            
        }

    }

}
