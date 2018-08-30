using NUnit.Framework;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using ResetCore.CodeDom;

namespace ResetCore.Data
{
    public class SerializerGener
    {
        //代码存放目录
        private static string serializerGenerPath;
        private static string SerializerGenerPath
        {
            get
            {
                return serializerGenerPath ?? (serializerGenerPath =
                           PathConfig.localDataResourcesPath + "GameDatas/AutoSerializer");
            }
        }

        private static readonly string serializerNamespace = "ResetCore.Data.AutoSerializer";

        /// <summary>
        /// 生成序列化
        /// </summary>
        /// <param name="type"></param>
        public static void GenSerializer(Type type)
        {
            if(type == null)
                return;

            var typeName = type.FullName;
            var finalName = GetSerializerName(type);
            CodeGener g = new CodeGener(serializerNamespace, finalName);
            g.AddBaseType("IXmlSerializer<" + type.FullName + ">");
            g.AddImport("System.Xml");

            //ToXElement方法
            CodeMemberMethod toXElementMethod = new CodeMemberMethod();
            toXElementMethod.Name = "Write";
            toXElementMethod.Attributes = MemberAttributes.Public;
            toXElementMethod.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(XmlElement)), "xElement"));
            toXElementMethod.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "name"));
            toXElementMethod.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeName), "obj"));
            //函数体
            toXElementMethod.Statements.AddRange(GetToXElementCodeStatements(type));
            g.newClass.Members.Add(toXElementMethod);

            //Parse方法
            CodeMemberMethod parseMethod = new CodeMemberMethod();
            parseMethod.Name = "Read";
            parseMethod.Attributes = MemberAttributes.Public;
            parseMethod.ReturnType = new CodeTypeReference(type);
            parseMethod.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(XmlElement)), "xElement"));
            parseMethod.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "name"));
            //函数体
            parseMethod.Statements.AddRange(GetParseXElementCodeStatements(type));
            g.newClass.Members.Add(parseMethod);


            //生成代码
            g.GenCSharp(SerializerGenerPath);
        }

        /// <summary>
        /// 获取转化为Element的代码
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static CodeStatement[] GetToXElementCodeStatements(Type type)
        {
            var statementList = new List<CodeStatement>();
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

            statementList.Add(new CodeSnippetStatement("\t\t\tvar node = xElement.OwnerDocument.CreateElement(name);"));
            statementList.Add(new CodeSnippetStatement("\t\t\txElement.AppendChild(node);"));
            for (int i = 0; i < fields.Length; i++)
            {
                var currentField = fields[i];
                var statement = GetToXElementCodeStatement(currentField);
                if(statement != null)
                    statementList.Add(statement);
            }
            return statementList.ToArray();
        }

        private static CodeStatement GetToXElementCodeStatement(FieldInfo info)
        {
            var type = info.FieldType;
            if (ReXmlSerializer.IsPrimitive(type))
            {
                return new CodeSnippetStatement(
                    string.Format("\t\t\tReXmlSerializer.Write(xElement, \"{0}\", obj.{0});", info.Name));
            }
            else
            {
                return new CodeSnippetStatement(
                    string.Format("\t\t\tCustomSerializer.GetSerializer<{0}>().Write(xElement, \"{1}\", obj. {1});",
                        type.FullName, info.Name));
            }
        }

        /// <summary>
        /// 获取解析Element的代码
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static CodeStatement[] GetParseXElementCodeStatements(Type type)
        {
            var statementList = new List<CodeStatement>();
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < fields.Length; i++)
            {
                var currentField = fields[i];
                var statement = GetParseStatement(currentField);
                if(statement != null)
                    statementList.Add(statement);
            }
            return statementList.ToArray();
        }

        private static CodeStatement GetParseStatement(FieldInfo info)
        {
            //TODO 解析的代码生成
            var type = info.FieldType;
            if (ReXmlSerializer.IsPrimitive(type))
            {
                return new CodeSnippetStatement(
                    string.Format("\t\t\tReXmlSerializer.Write(xElement, \"{0}\", obj.{0});", info.Name));
            }
            else
            {
                return new CodeSnippetStatement(
                    string.Format("\t\t\tCustomSerializer.GetSerializer<{0}>().Write(xElement, \"{1}\", obj. {1});",
                        type.FullName, info.Name));
            }
            return null;
        }


        private static string GetSerializerName(Type type)
        {
            return type.FullName.Replace('.', '_') + "_Serializer";
        }

        [Test]
        public void Test()
        {
            GenSerializer(typeof(TestSerializeData));
        }


    }


}
