using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using ResetCore.Util;
using UnityEngine.UI;
using System.Text;
using ResetCore.CodeDom;
using System.IO;
using UnityEditor;
using System.CodeDom;

namespace ResetCore.UGUI
{


    public class UIScriptGener
    {
        private string[] splitGoNameAndProperty = new string[] { "@" };
        private string[] splitPropertyAndType = new string[] { ":" };
        private string[] splitComponentAndProperty = new string[] { "->" };

        private GameObject rootGo;
        private Type uiType;
        //生成界面代码
        public void GenViewScript(BaseUI ui)
        {
            rootGo = ui.gameObject;
            uiType = ui.GetType();
            string viewClassName = uiType.Name + "View";

            if(File.Exists(PathEx.Combine(Application.dataPath, UIView.uiViewScriptPath, viewClassName + ".cs")))
            {
                if (!EditorUtility.DisplayDialog("提示","已存在" + viewClassName + ",是否要覆盖？", "继续", "取消"))
                    return;
            }

            CodeGener gener = new CodeGener(UIView.uiViewNameSpace, viewClassName);
            gener.AddBaseType(typeof(UIView).Name)
                .AddImport("ResetCore.UGUI");

            rootGo.transform.DoToSelfAndAllChildren((tran) =>
            {
                GameObject go = tran.gameObject;
                if (!go.name.StartsWith(UIView.genableSign)) return;

                string goName = go.name.Replace(UIView.genableSign, string.Empty);

                if (goName.Contains(splitGoNameAndProperty[0]))
                {
                    goName = goName.Split(splitGoNameAndProperty, StringSplitOptions.RemoveEmptyEntries)[0];
                }

                gener.AddMemberProperty(typeof(GameObject), UIView.goName + goName);
                var coms = go.GetComponents<Component>();
                foreach(var com in coms)
                {
                    Type comType = com.GetType();
                    if (!UIView.uiCompTypeList.Contains(comType)) continue;

                    gener.AddMemberProperty(comType, UIView.comNameDict[comType] + goName);
                }

            });
            gener.GenCSharp(PathEx.Combine(Application.dataPath, UIView.uiViewScriptPath));
        }

        //生成数据代码
        public void GenModelScript(BaseUI ui)
        {
            rootGo = ui.gameObject;
            uiType = ui.GetType();
            string modelClassName = uiType.Name + "Model";

            if (File.Exists(PathEx.Combine(Application.dataPath, UIModel.uiModelScriptPath, modelClassName + ".cs")))
            {
                if (!EditorUtility.DisplayDialog("提示", "已存在" + modelClassName + ",是否要覆盖？", "继续", "取消"))
                    return;
            }

            CodeGener gener = new CodeGener(UIModel.uiModelNameSpace, modelClassName);
            gener.AddBaseType(typeof(UIModel).Name)
                .AddImport("ResetCore.UGUI")
                .AddImport("ResetCore.Event");

            List<string> hasAddedProperty = new List<string>();

            CodeConstructor constructor = new CodeConstructor();
            constructor.Attributes = MemberAttributes.Public;

            rootGo.transform.DoToSelfAndAllChildren((tran) =>
            {
                GameObject go = tran.gameObject;
                if (!go.name.StartsWith(UIModel.genableSign)) return;

                string goName = go.name.Replace(UIModel.genableSign, string.Empty);
                string propertyName;
                string typeName;

                if (!goName.Contains(splitGoNameAndProperty[0]))
                    return;

                string[] tempstrs = goName.Split(splitGoNameAndProperty, StringSplitOptions.RemoveEmptyEntries);
                goName = tempstrs[0];

                if (!tempstrs[1].Contains(splitComponentAndProperty[0]))
                {
                    EditorUtility.WarnPrefab(go, "命名错误", go.name + "的格式不正确，标准格式为“g-View变量名@组件名->Model属性名:变量类型”", "好的");
                    return;
                }
                
                propertyName = tempstrs[1].Split(splitComponentAndProperty, StringSplitOptions.RemoveEmptyEntries)[1];

                if (!propertyName.Contains(splitPropertyAndType.ToString()))
                    typeName = "EventProperty<string>";

                tempstrs = propertyName.Split(splitPropertyAndType, StringSplitOptions.RemoveEmptyEntries);
                propertyName = tempstrs[0];
                typeName = "EventProperty<" + tempstrs[1] + ">";

                if (!hasAddedProperty.Contains(propertyName))
                {
                    gener.AddMemberProperty(typeName, propertyName);
                    hasAddedProperty.Add(propertyName);

                    constructor.Statements.AddRange(new CodeStatement[]
                    {
                        new CodeCommentStatement("From " + go.name),
                        new CodeSnippetStatement(propertyName + " = new " + typeName + "();")
                    });
                }
            });
            gener.newClass.Members.Add(constructor);
            gener.GenCSharp(PathEx.Combine(Application.dataPath, UIModel.uiModelScriptPath));
        }

        //生成自动绑定代码
        public void GenBindScript(BaseUI ui)
        {
            rootGo = ui.gameObject;
            uiType = ui.GetType();
            string binderClassName = uiType.Name + "Binder";

            if (File.Exists(PathEx.Combine(Application.dataPath, UIBinder.uiBinderScriptPath, binderClassName + ".cs")))
            {
                if (!EditorUtility.DisplayDialog("提示", "已存在" + binderClassName + ",是否要覆盖？", "继续", "取消"))
                    return;
            }

            CodeGener gener = new CodeGener(UIBinder.uiBinderNameSpace, binderClassName);
            gener.AddBaseType(typeof(UIBinder).Name)
                .AddImport("ResetCore.UGUI")
                .AddImport("ResetCore.Event")
                .AddImport(UIView.uiViewNameSpace)
                .AddImport(UIModel.uiModelNameSpace);


            CodeMemberMethod initMethod = new CodeMemberMethod();
            initMethod.AddComment("Used to bind view and model", true);
            initMethod.Name = "Bind";
            string viewClassName = uiType.Name + "View";
            string modelClassName = uiType.Name + "Model";

            initMethod.Parameters.AddRange(new CodeParameterDeclarationExpression[] {
                    new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(UIView).Name), "view"),
                    new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(UIModel).Name), "model"),
                });
            initMethod.Attributes = MemberAttributes.Public | MemberAttributes.Override;

            List<string> hasAddedProperty = new List<string>();
            rootGo.transform.DoToSelfAndAllChildren((tran) =>
            {
                GameObject go = tran.gameObject;
                if (!go.name.StartsWith(UIBinder.genableSign)) return;

                string goName = go.name.Replace(UIBinder.genableSign, string.Empty);
                string comName;
                string propertyName;
                string typeName;

                if (!goName.Contains(splitGoNameAndProperty[0]))
                    return;

                string[] tempstrs = goName.Split(splitGoNameAndProperty, StringSplitOptions.RemoveEmptyEntries);
                goName = tempstrs[0];

                if (!tempstrs[1].Contains(splitComponentAndProperty[0]))
                {
                    EditorUtility.WarnPrefab(go, "命名错误", go.name + "的格式不正确，标准格式为“g-View变量名@组件名->Model属性名:变量类型”", "好的");
                    return;
                }

                tempstrs = tempstrs[1].Split(splitComponentAndProperty, StringSplitOptions.RemoveEmptyEntries);
                comName = tempstrs[0];
                propertyName = tempstrs[1];

                if (!propertyName.Contains(splitPropertyAndType.ToString()))
                    typeName = "EventProperty<string>";
                
                tempstrs = propertyName.Split(splitPropertyAndType, StringSplitOptions.RemoveEmptyEntries);
                propertyName = tempstrs[0];
                typeName = "EventProperty<" + tempstrs[1] + ">";

                initMethod.Statements.AddRange(new CodeStatement[] {
                    new CodeCommentStatement("From " + go.name),
                    new CodeSnippetStatement("((" + viewClassName + ")view)." + 
                        comName + goName + ".Bind(((" + modelClassName + ")model)." + propertyName + ");"),
                });

            });

            gener.newClass.Members.Add(initMethod);

            gener.GenCSharp(PathEx.Combine(Application.dataPath, UIBinder.uiBinderScriptPath));
        }
    }

}
