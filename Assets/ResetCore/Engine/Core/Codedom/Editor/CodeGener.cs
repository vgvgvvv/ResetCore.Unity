using UnityEngine;
using System.Collections;
using System.CodeDom;
using System.Reflection;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEditor;

namespace ResetCore.CodeDom
{
    public class CodeGener
    {

        public CodeCompileUnit unit { get; private set; }
        public CodeTypeDeclaration newClass { get; private set; }
        public CodeNamespace codeNameSpace { get; private set; }

        public CodeGener(string namespaceName, string name, TypeAttributes typeAttr = TypeAttributes.Public, bool isClass = true)
        {
            unit = new CodeCompileUnit();
            newClass = new CodeTypeDeclaration(name);
            newClass.TypeAttributes = typeAttr;
            newClass.IsClass = true;
            codeNameSpace = new CodeNamespace(namespaceName);

            codeNameSpace.Types.Add(newClass);
            unit.Namespaces.Add(codeNameSpace);

        }

        /// <summary>
        /// 添加命名空间
        /// </summary>
        /// <param name="importNames">命名空间</param>
        /// <returns></returns>
        public CodeGener AddImport(params string[] importNames)
        {
            importNames.Foreach<string>((str) =>
            {
                CodeNamespaceImport import = new CodeNamespaceImport(str);
                codeNameSpace.Imports.Add(import);
            });
            return this;
        }

        /// <summary>
        /// 添加基类
        /// </summary>
        /// <param name="baseTypeNames">基类名</param>
        /// <returns></returns>
        public CodeGener AddBaseType(params string[] baseTypeNames)
        {
            baseTypeNames.Foreach<string>((str) =>
            {
                newClass.BaseTypes.Add(str);
            });
            return this;
        }

        /// <summary>
        /// 添加成员域
        /// </summary>
        /// <param name="type">域类型</param>
        /// <param name="fieldName">域名称</param>
        /// <param name="initAct">初始化行为</param>
        /// <param name="memberAttr">域特性</param>
        /// <returns></returns>
        public CodeGener AddMemberField(Type type, string fieldName, Action<CodeMemberField> initAct = null, MemberAttributes memberAttr = MemberAttributes.Public)
        {
            CodeMemberField newField = new CodeMemberField(type, fieldName);
            newField.Attributes = memberAttr;
            newClass.Members.Add(newField);
            if (initAct != null)
            {
                initAct(newField);
            }
            return this;
        }

        /// <summary>
        /// 添加成员域
        /// </summary>
        /// <param name="typeName">域类型</param>
        /// <param name="fieldName">域名称</param>
        /// <param name="initAct">初始化行为</param>
        /// <param name="memberAttr">域特性</param>
        /// <returns></returns>
        public CodeGener AddMemberField(string typeName, string fieldName, Action<CodeMemberField> initAct = null, MemberAttributes memberAttr = MemberAttributes.Public)
        {
            CodeMemberField newField = new CodeMemberField(typeName, fieldName);
            newField.Attributes = memberAttr;
            newClass.Members.Add(newField);
            if (initAct != null)
            {
                initAct(newField);
            }
            return this;
        }

        /// <summary>
        /// 添加成员属性
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="propertyName">字段名</param>
        /// <param name="initAct">初始化行为</param>
        /// <param name="memberAttr">成员特性</param>
        /// <returns></returns>
        public CodeGener AddMemberProperty(Type type, string propertyName, Action<CodeMemberProperty> initAct = null, MemberAttributes memberAttr = MemberAttributes.Public)
        {
            string fieldName = "_" + propertyName;
            AddMemberField(type, fieldName, null, MemberAttributes.Private);

            CodeMemberProperty newProperty = new CodeMemberProperty();
            newProperty.Type = new CodeTypeReference(type);
            newProperty.Name = propertyName;
            newProperty.Attributes = memberAttr;
            newClass.Members.Add(newProperty);

            newProperty.HasGet = true;
            newProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName)));
            newProperty.HasSet = true;
            newProperty.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName), new CodePropertySetValueReferenceExpression()));

            if (initAct != null)
            {
                initAct(newProperty);
            }

            return this;
        }

        /// <summary>
        /// 添加成员属性
        /// </summary>
        /// <param name="typeName">类型名</param>
        /// <param name="propertyName">字段名</param>
        /// <param name="initAct">初始化行为</param>
        /// <param name="memberAttr">成员特性</param>
        /// <returns></returns>
        public CodeGener AddMemberProperty(string typeName, string propertyName, Action<CodeMemberProperty> initAct = null, MemberAttributes memberAttr = MemberAttributes.Public)
        {
            string fieldName = "_" + propertyName;
            AddMemberField(typeName, fieldName, null, MemberAttributes.Private);

            CodeMemberProperty newProperty = new CodeMemberProperty();
            newProperty.Type = new CodeTypeReference(typeName);
            newProperty.Name = propertyName;
            newProperty.Attributes = memberAttr;
            newClass.Members.Add(newProperty);

            newProperty.HasGet = true;
            newProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName)));
            newProperty.HasSet = true;
            newProperty.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName), new CodePropertySetValueReferenceExpression()));

            if (initAct != null)
            {
                initAct(newProperty);
            }

            return this;
        }

        /// <summary>
        /// 添加成员函数
        /// </summary>
        /// <param name="returnType"></param>
        /// <param name="paramDict"></param>
        /// <param name="statements"></param>
        /// <returns></returns>
        public CodeGener AddMemberMethod(Type returnType, Dictionary<string, Type> paramDict, params CodeStatement[] statements)
        {
            CodeMemberMethod newMethod = new CodeMemberMethod();
            newMethod.ReturnType = new CodeTypeReference(returnType);
            foreach (var keyValuePair in paramDict)
            {
                newMethod.Parameters.Add(new CodeParameterDeclarationExpression(keyValuePair.Value, keyValuePair.Key));
            }

            newMethod.Statements.AddRange(statements);

            newClass.Members.Add(newMethod);

            return this;
        }

        /// <summary>
        /// 添加成员函数
        /// </summary>
        /// <param name="returnType"></param>
        /// <param name="paramDict"></param>
        /// <param name="statements"></param>
        /// <returns></returns>
        public CodeGener AddMemberMethod(string returnType, Dictionary<string, Type> paramDict, params CodeStatement[] statements)
        {
            CodeMemberMethod newMethod = new CodeMemberMethod();
            newMethod.ReturnType = new CodeTypeReference(returnType);
            foreach (var keyValuePair in paramDict)
            {
                newMethod.Parameters.Add(new CodeParameterDeclarationExpression(keyValuePair.Value, keyValuePair.Key));
            }

            newMethod.Statements.AddRange(statements);

            newClass.Members.Add(newMethod);

            return this;
        }

        /// <summary>
        /// 生成代码
        /// </summary>
        /// <param name="outputPath">生成目录</param>
        public void GenCSharp(string outputPath)
        {
            if(!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
            string outputFilePath = Path.Combine(outputPath, newClass.Name) + ".cs";
            //生成代码
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");

            //缩进样式
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            //空行
            options.BlankLinesBetweenMembers = true;

            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outputFilePath))
            {
                Debug.Log("生成代码" + outputFilePath);
                provider.GenerateCodeFromCompileUnit(unit, sw, options);
            }

            AssetDatabase.Refresh();
        }
    }

    public static class CodeEx
    {
        /// <summary>
        /// 添加注释
        /// </summary>
        /// <param name="member"></param>
        /// <param name="comment"></param>
        /// <param name="isDoc"></param>
        public static void AddComment(this CodeTypeMember member, string comment, bool isDoc = false)
        {
            member.Comments.Add(new CodeCommentStatement(new CodeComment(comment, isDoc)));
        }

    }

    public static class CodeTypeMemberEx
    {
        /// <summary>
        /// 添加不带参数的属性
        /// </summary>
        /// <param name="member"></param>
        /// <param name="AttributeType"></param>
        public static void AddMemberCostomAttribute<T>(this CodeTypeMember member)
        {
            Type AttributeType = typeof(T);
            member.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(AttributeType)));
        }

        public static void AddMemberCostomAttribute(this CodeTypeMember member, string attr)
        {
            member.CustomAttributes.Add(new CodeAttributeDeclaration(attr));
        }

        /// <summary>
        /// 添加带有参数的属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="member"></param>
        /// <param name="argExpressions"></param>
        public static void AddMemberCostomAttribute<T>(this CodeTypeMember member, params string[] argExpressions) where T : Attribute
        {
            Type AttributeType = typeof(T);
            CodeAttributeArgument[] args = new CodeAttributeArgument[argExpressions.Length];
            argExpressions.Foreach<string>((i, expression) =>
            {
                args[i] = new CodeAttributeArgument(new CodeSnippetExpression(expression));
            });

            member.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(AttributeType), args));
        }

        public static void AddMemberCostomAttribute(this CodeTypeMember member, string attr, params string[] argExpressions)
        {
            CodeAttributeArgument[] args = new CodeAttributeArgument[argExpressions.Length];
            argExpressions.Foreach<string>((i, expression) =>
            {
                args[i] = new CodeAttributeArgument(new CodeSnippetExpression(expression));
            });
            member.CustomAttributes.Add(new CodeAttributeDeclaration(attr, args));
        }

        /// <summary>
        /// 域的初始化表达式
        /// </summary>
        /// <param name="field"></param>
        /// <param name="express">表达式</param>
        public static void AddFieldMemberInit(this CodeMemberField field, string express)
        {
            field.InitExpression = new CodeSnippetExpression(express);
        }
    }

}

