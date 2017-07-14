using UnityEngine;
using System.Collections;
using System.CodeDom.Compiler;
using System.CodeDom;
using System.Reflection;
using System.Xml.Linq;
using ResetCore.Util;
using System.IO;
using System.Collections.Generic;
using System;
using ResetCore.Data;
using UnityEditor;
using ResetCore.Data.GameDatas.Xml;
using ResetCore.Data.GameDatas.Obj;
using ResetCore.Data.GameDatas.Json;

public class XmlDataClassGener
{

    //
    private static string className;
    private static string baseClassName;
    private static string nameSpace;
    private static string[] importNameSpaces;
    private static string outputFile;


    /// <summary>
    /// 创建代码
    /// </summary>
    /// <param name="className">类名</param>
    /// <param name="baseType">基础类型</param>
    /// <param name="fieldDict">属性表</param>
    /// <param name="path">自定义路径</param>
    public static void CreateNewClass(string className, Dictionary<string, Type> fieldDict
        , Dictionary<string, List<string>> attributeDict, string path = null)
    {
        GetPropString(className, typeof(XmlData), path);

        CodeCompileUnit unit;
        CodeTypeDeclaration NewClass;
        CreateNewClass(out unit, out NewClass);

        AddProp(fieldDict, attributeDict, NewClass);

        GenCSharp(outputFile, unit);

    }

    private static void GetPropString(string className, Type baseType, string path)
    {
        importNameSpaces = new string[]{
                "System","System.Collections.Generic", "UnityEngine"
            };
        XmlDataClassGener.className = className;
        baseClassName = baseType.Name + "<" + className + ">";

        nameSpace = XmlData.nameSpace;
        string classPath = PathConfig.GetLoaclGameDataClassPath(PathConfig.DataType.Xml);
        if (!Directory.Exists(classPath))
        {
            Directory.CreateDirectory(classPath);
        }
        outputFile = classPath + className + ".cs";


        //自定义Path
        if (path != null)
        {
            outputFile = path;
        }
    }
    private static void CreateNewClass(out CodeCompileUnit unit, out CodeTypeDeclaration NewClass)
    {
        unit = new CodeCompileUnit();
        CodeNamespace theNamespace = new CodeNamespace(nameSpace);
        unit.Namespaces.Add(theNamespace);


        NewClass = new CodeTypeDeclaration(className);
        theNamespace.Types.Add(NewClass);

        foreach (string ns in importNameSpaces)
        {
            CodeNamespaceImport import = new CodeNamespaceImport(ns);
            theNamespace.Imports.Add(import);
        }


        NewClass.TypeAttributes = TypeAttributes.Public;
        NewClass.BaseTypes.Add(baseClassName);
        NewClass.IsClass = true;

        CodeMemberField fileNameField = new CodeMemberField("String", "fileName");
        fileNameField.Attributes = MemberAttributes.Static | MemberAttributes.Public;
        fileNameField.InitExpression = new CodeSnippetExpression("\"" + className + "\"");
        NewClass.Members.Add(fileNameField);
    }

    private static void AddProp(Dictionary<string, Type> fieldDic
        , Dictionary<string, List<string>> attributeDict, CodeTypeDeclaration NewClass)
    {
        foreach (KeyValuePair<string, Type> pair in fieldDic)
        {
            string propName = pair.Key;
            Type propType = pair.Value;

            //添加字段

            CodeMemberField field = new CodeMemberField(pair.Value, "_" + propName);
            field.Attributes = MemberAttributes.Private;

            NewClass.Members.Add(field);

            //添加属性
            CodeMemberProperty property = new CodeMemberProperty();
            property.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            property.Name = propName;
            property.HasGet = true;
            property.HasSet = true;
            property.Type = new CodeTypeReference(propType);
            property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_" + propName)));
            property.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_" + propName), new CodePropertySetValueReferenceExpression()));

            foreach (var attr in attributeDict[propName])
            {
                Type attrType;
                if (DataAttributes.attributes.TryGetValue(attr, out attrType))
                {
                    property.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(attrType)));
                }
            }

            NewClass.Members.Add(property);
        }
    }

    private static void GenCSharp(string outputFile, CodeCompileUnit unit)
    {
        //生成代码
        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");

        //缩进样式
        CodeGeneratorOptions options = new CodeGeneratorOptions();
        options.BracingStyle = "C";
        //空行
        options.BlankLinesBetweenMembers = true;

        PathEx.MakeDirectoryExist(outputFile);
        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outputFile))
        {
            Debug.Log("生成代码" + outputFile);
            provider.GenerateCodeFromCompileUnit(unit, sw, options);
        }
    }
}
