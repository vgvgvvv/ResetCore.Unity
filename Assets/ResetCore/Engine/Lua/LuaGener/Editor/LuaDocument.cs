using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using MoonCommonLib;
using UnityEngine;

namespace ResetCore.Lua
{
    public enum LuaMemberType
    {
        Global,//全局
        Local,//局部
    }


    /// <summary>
    /// Lua文档对象
    /// </summary>
    public class LuaDocumentNode
    {

        public LuaDocumentNode() { }
        public LuaDocumentNode(string className)
        {
            this.ClassName = className;
        }

        public const string LUA_DECLEAR =
            "--this file is gen by script\n--you can edit this file in custom part\n\n\n";
        public const string LUA_REQUIRES_TAG = "--lua requires";
        public const string LUA_MODEL_TAG = "--lua model";
        public const string LUA_CLASSDEINE_TAG = "--lua class define";
        public const string LUA_FIELDS_TAG = "--lua fields";
        public const string LUA_PROPERTIES_TAG = "--lua properties";
        public const string LUA_FUNCTIONS_TAG = "--lua functions";
        public const string LUA_STATEMENTS_TAG = "--lua statements";
        public const string LUA_CUSTOM_TAG = "--lua custom scripts";
        public const string LUA_NEXT_TAG = "--next--";
        public const string Lua_FUNC_END = "--func end";

        #region Document基本结构

        /// <summary>
        /// 类名
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 定义类的方式
        /// </summary>
        public LuaBaseStatementNode ClassInitStatement { get; set; }

        /// <summary>
        /// Lua自定义模块名称
        /// </summary>
        public LuaModelNode ModelNode { get; set; }

        /// <summary>
        /// Lua请求的模块
        /// </summary>
        private readonly List<LuaRequireNode> RequireNodes = new List<LuaRequireNode>();

        public void AddRequire(LuaRequireNode requireNode)
        {
            for (int i = 0; i < RequireNodes.Count; i++)
            {
                if (RequireNodes[i].requireName == requireNode.requireName ||
                    string.IsNullOrEmpty(requireNode.requireName))
                    return;
            }
            RequireNodes.Add(requireNode);
        }

        public void RemoveRequire(string requireName)
        {
            for (int i = 0; i < RequireNodes.Count; i++)
            {
                if (RequireNodes[i].requireName == requireName)
                {
                    RequireNodes.RemoveAt(i);
                }
            }
        }

        public LuaRequireNode GetRequire(string requireName)
        {
            for (int i = 0; i < RequireNodes.Count; i++)
            {
                if (RequireNodes[i].requireName == requireName)
                {
                    return RequireNodes[i];
                }
            }
            return null;
        }

        public List<LuaRequireNode> GetRequireNodes()
        {
            return new List<LuaRequireNode>(RequireNodes);
        }

        /// <summary>
        /// Lua函数
        /// </summary>
        private readonly List<LuaFunctionNode> FunctionNodes = new List<LuaFunctionNode>();

        public void AddFunction(LuaFunctionNode functionNode)
        {
            for (int i = 0; i < FunctionNodes.Count; i++)
            {
                if (FunctionNodes[i].functionName == functionNode.functionName ||
                    string.IsNullOrEmpty(functionNode.functionName))
                    return;
            }
            FunctionNodes.Add(functionNode);
        }

        public void RemoveFunction(string Name)
        {
            for (int i = 0; i < FunctionNodes.Count; i++)
            {
                if (FunctionNodes[i].functionName == Name)
                {
                    FunctionNodes.RemoveAt(i);
                    return;
                }
            }
        }

        public LuaFunctionNode GetFunction(string name)
        {
            for (int i = 0; i < FunctionNodes.Count; i++)
            {
                if (FunctionNodes[i].functionName == name)
                {
                    return FunctionNodes[i];
                }
            }
            return null;
        }

        public List<LuaFunctionNode> GetFunctionNodes()
        {
            return new List<LuaFunctionNode>(FunctionNodes);
        }

        /// <summary>
        /// Lua值域
        /// </summary>
        private readonly List<LuaFieldNode> FieldNodes = new List<LuaFieldNode>();

        public void AddField(LuaFieldNode fieldNode)
        {
            for (int i = 0; i < FieldNodes.Count; i++)
            {
                if (FieldNodes[i].fieldName == fieldNode.fieldName ||
                    string.IsNullOrEmpty(fieldNode.fieldName))
                    return;
            }
            FieldNodes.Add(fieldNode);
        }

        public void RemoveField(string Name)
        {
            for (int i = 0; i < FieldNodes.Count; i++)
            {
                if (FieldNodes[i].fieldName == Name)
                {
                    FieldNodes.RemoveAt(i);
                    return;
                }
            }
        }

        public LuaFieldNode GetField(string name)
        {
            for (int i = 0; i < FieldNodes.Count; i++)
            {
                if (FieldNodes[i].fieldName == name)
                {
                    return FieldNodes[i];
                }
            }
            return null;
        }

        public List<LuaFieldNode> GetFieldNodes()
        {
            return new List<LuaFieldNode>(FieldNodes);
        }

        /// <summary>
        /// Lua属性
        /// </summary>
        private readonly List<LuaPropertyNode> PropertyNodes = new List<LuaPropertyNode>();

        public void AddProperty(LuaPropertyNode propertyNode)
        {
            for (int i = 0; i < PropertyNodes.Count; i++)
            {
                if (PropertyNodes[i].propertyName == propertyNode.propertyName ||
                    string.IsNullOrEmpty(propertyNode.propertyName))
                {
                    PropertyNodes.RemoveAt(i);
                }
            }
            PropertyNodes.Add(propertyNode);
        }

        public void RemoveProperty(string Name)
        {
            for (int i = 0; i < PropertyNodes.Count; i++)
            {
                if (PropertyNodes[i].propertyName == Name)
                {
                    PropertyNodes.RemoveAt(i);
                    return;
                }
            }
        }

        public LuaPropertyNode GetProperty(string name)
        {
            for (int i = 0; i < PropertyNodes.Count; i++)
            {
                if (PropertyNodes[i].propertyName == name)
                {
                    return PropertyNodes[i];
                }
            }
            return null;
        }

        public List<LuaPropertyNode> GetPropertyNodes()
        {
            return new List<LuaPropertyNode>(PropertyNodes);
        }

        /// <summary>
        /// Lua代码语句
        /// </summary>
        private readonly List<LuaBaseStatementNode> StatementNodes = new List<LuaBaseStatementNode>();

        /// <summary>
        /// 自定义代码
        /// </summary>
        public string CustomScript { get; set; }

        #endregion

        /// <summary>
        /// 转成String
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var documentNode = this;
            StringBuilder builder = new StringBuilder();

            builder.Append(LUA_DECLEAR);

            //Requires
            if (documentNode.RequireNodes.Count > 0)
            {
                builder.Append("--lua requires\n");
                documentNode.RequireNodes.ForEach((node) =>
                {
                    builder.Append(node.ToString(documentNode))
                        .Append(node.nextLine);
                });
                builder.Append("--lua requires end\n");
                builder.Append("\n");
            }

            //model
            if (documentNode.ModelNode != null)
            {
                builder.Append($"{LUA_MODEL_TAG}\n");
                builder.Append(documentNode.ModelNode.ToString(documentNode))
                    .Append(documentNode.ModelNode.nextLine);
                builder.Append($"{LUA_MODEL_TAG} end\n");
                builder.Append("\n");
            }

            //class define
            if (!string.IsNullOrEmpty(ClassName))
            {
                builder.Append($"{LUA_CLASSDEINE_TAG}\n");
                builder.Append(documentNode.ClassName).Append(" = ");
                builder.Append(documentNode.ClassInitStatement != null
                        ? documentNode.ClassInitStatement.ToString(documentNode)
                        : "{}")
                    .Append(documentNode.ModelNode.nextLine);
                builder.Append($"{LUA_CLASSDEINE_TAG} end\n");
                builder.Append("\n");
            }

            //lua fields
            if (documentNode.FieldNodes.Count > 0)
            {
                builder.Append($"{LUA_FIELDS_TAG}\n");
                documentNode.FieldNodes.ForEach((node) =>
                {
                    builder.Append(node.ToString(documentNode))
                        .Append(node.nextLine)
                        .Append($"{LUA_NEXT_TAG}")
                        .Append(node.nextLine);
                });
                builder.Append($"{LUA_FIELDS_TAG} end\n");
                builder.Append("\n");
            }

            //lua properties
            if (documentNode.PropertyNodes.Count > 0)
            {
                builder.Append($"{LUA_PROPERTIES_TAG}\n");
                documentNode.PropertyNodes.ForEach((node) =>
                {
                    builder.Append(node.ToString(documentNode))
                        .Append(node.nextLine)
                        .Append($"{LUA_NEXT_TAG}")
                        .Append(node.nextLine);
                });
                builder.Append($"{LUA_PROPERTIES_TAG} end\n");
                builder.Append("\n");
            }

            //lua functions
            if (documentNode.FunctionNodes.Count > 0)
            {
                builder.Append($"{LUA_FUNCTIONS_TAG}\n");
                documentNode.FunctionNodes.ForEach((node) =>
                {
                    builder.Append(node.ToString(documentNode))
                        .Append(node.nextLine)
                        .Append($"{LUA_NEXT_TAG}")
                        .Append(node.nextLine);
                });
                builder.Append($"{LUA_FUNCTIONS_TAG} end\n");
                builder.Append("\n");
            }

            //statements
            if (documentNode.StatementNodes.Count > 0)
            {
                builder.Append($"{LUA_STATEMENTS_TAG}\n");
                documentNode.StatementNodes.ForEach((node) =>
                {
                    builder.Append(node.ToString(documentNode))
                        .Append(node.nextLine)
                        .Append($"{LUA_NEXT_TAG}")
                        .Append(node.nextLine);
                });
                builder.Append($"{LUA_STATEMENTS_TAG} end\n");
                builder.Append("\n");
            }

            builder.Append($"{LUA_CUSTOM_TAG}\n");
            //自定义部分
            builder.Append(CustomScript).Append("\n");
            builder.Append($"{LUA_CUSTOM_TAG} end\n");

            return builder.ToString();
        }


        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="context"></param>
        public void Parse(string context)
        {
            //requires
            RequireNodes.Clear();
            if (context.Contains(LUA_REQUIRES_TAG))
            {
                var requireContext = context.FindBetween($"{LUA_REQUIRES_TAG}", $"{LUA_REQUIRES_TAG} end");
                var lines = from line in requireContext.Split('\n') where !string.IsNullOrEmpty(line.Trim()) select line;
                lines.Foreach((line) =>
                {
                    var requireNode = new LuaRequireNode();
                    requireNode.Parse(line);
                    RequireNodes.Add(requireNode);
                });
            }

            //model
            ModelNode = null;
            if (context.Contains(LUA_MODEL_TAG))
            {
                var modelContext = context.FindBetween($"{LUA_MODEL_TAG}", $"{LUA_MODEL_TAG} end");
                ModelNode = new LuaModelNode();
                ModelNode.Parse(modelContext);
            }
            //class
            ClassName = null;
            ClassInitStatement = null;
            if (context.Contains(LUA_CLASSDEINE_TAG))
            {
                var classDefineContext = context.FindBetween($"{LUA_CLASSDEINE_TAG}", $"{LUA_CLASSDEINE_TAG} end");
                var splits = classDefineContext.Split('=');
                ClassName = splits[0].Trim();
                ClassInitStatement = splits[1].Trim() == "{}" ? null : new LuaScriptStatementNode(splits[1].Trim());
            }
            //fields
            FieldNodes.Clear();
            if (context.Contains(LUA_FIELDS_TAG))
            {
                var fieldsContext = context.FindBetween($"{LUA_FIELDS_TAG}", $"{LUA_FIELDS_TAG} end");
                while (true)
                {
                    if (string.IsNullOrEmpty(fieldsContext) || !fieldsContext.Contains(LUA_NEXT_TAG))
                        break;
                    var endPosition = fieldsContext.IndexOf(LUA_NEXT_TAG);
                    var fieldContext = fieldsContext.Substring(0, endPosition).Trim();
                    var fieldNode = new LuaFieldNode();
                    FieldNodes.Add(fieldNode);
                    fieldNode.Parse(fieldContext);
                    fieldsContext = fieldsContext.FindAfter(LUA_NEXT_TAG).Trim();
                }
            }
            //properties
            PropertyNodes.Clear();
            if (context.Contains(LUA_PROPERTIES_TAG))
            {
                var propertiesContext = context.FindBetween($"{LUA_PROPERTIES_TAG}", $"{LUA_PROPERTIES_TAG} end");
                while (true)
                {
                    if (string.IsNullOrEmpty(propertiesContext) || !propertiesContext.Contains(LUA_NEXT_TAG))
                        break;
                    var endPosition = propertiesContext.IndexOf(LUA_NEXT_TAG);
                    var propertyContext = propertiesContext.Substring(0, endPosition).Trim();
                    var propertyNode = new LuaPropertyNode();
                    PropertyNodes.Add(propertyNode);
                    propertyNode.Parse(propertyContext);
                    propertiesContext = propertiesContext.FindAfter(LUA_NEXT_TAG).Trim();
                }
            }
            //functions
            FunctionNodes.Clear();
            if (context.Contains(LUA_FUNCTIONS_TAG))
            {
                var functionsContext = context.FindBetween($"{LUA_FUNCTIONS_TAG}", $"{LUA_FUNCTIONS_TAG} end");
                int i = 0;
                while (true)
                {
                    i++;
                    if (i > 100)
                    {
                        break;
                    }
                    if (string.IsNullOrEmpty(functionsContext) || !functionsContext.Contains(LUA_NEXT_TAG) || i > 100)
                        break;
                    var endPosition = functionsContext.IndexOf(LUA_NEXT_TAG);
                    var functionContext = functionsContext.Substring(0, endPosition).Trim();
                    var functionNode = new LuaFunctionNode();
                    FunctionNodes.Add(functionNode);
                    functionNode.Parse(functionContext);
                    functionsContext = functionsContext.FindAfter(LUA_NEXT_TAG).Trim();
                }
            }
            //statements
            StatementNodes.Clear();
            if (context.Contains(LUA_STATEMENTS_TAG))
            {
                var statementsContext = context.FindBetween($"{LUA_STATEMENTS_TAG}", $"{LUA_STATEMENTS_TAG} end");
                var lines = from line in statementsContext.SplitAndTrim('\n')
                            where string.IsNullOrEmpty(line.Trim())
                            select new LuaScriptStatementNode(line);
                StatementNodes.AddRange(lines);
            }
            //custom
            CustomScript = null;
            if (context.Contains(LUA_CUSTOM_TAG))
            {
                var customContext = context.FindBetween($"{LUA_CUSTOM_TAG}", $"{LUA_CUSTOM_TAG} end");
                CustomScript = customContext.Trim();
            }
        }

        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="path"></param>
        public void Load(string path)
        {
            if (File.Exists(path))
            {
                FileInfo file = new FileInfo(path);
                Parse(file.ReadText());
            }
        }
    }

    /// <summary>
    /// 基础的Lua语句
    /// </summary>
    public abstract class LuaBaseStatementNode
    {
        /// <summary>
        /// 缩进层级
        /// </summary>
        public int stateLayer { get; set; } = 0;

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <param name="documentNode"></param>
        /// <returns></returns>
        public abstract string ToString(LuaDocumentNode documentNode);

        /// <summary>
        /// 解析字符串
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract void Parse(string context);

        public string nextLine
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("\n");
                for (int i = 0; i < stateLayer; i++)
                {
                    builder.Append("\t");
                }
                return builder.ToString();
            }
        }

        public string childNextLine
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("\n");
                for (int i = 0; i <= stateLayer; i++)
                {
                    builder.Append("\t");
                }
                return builder.ToString();
            }
        }
    }

    /// <summary>
    /// Lua Require语句
    /// </summary>
    public class LuaRequireNode : LuaBaseStatementNode
    {
        public LuaRequireNode() { }
        public LuaRequireNode(string requireName)
        {
            this.requireName = requireName;
        }

        public string requireName { get; private set; }

        public override string ToString(LuaDocumentNode documentNode)
        {
            return $"require \"{requireName}\"";
        }

        public override void Parse(string context)
        {
            context = context.Trim();
            requireName = context.Replace("require", string.Empty).Trim().Replace("\"", string.Empty);
        }
    }

    /// <summary>
    /// Lua Model语句
    /// </summary>
    public class LuaModelNode : LuaBaseStatementNode
    {
        public LuaModelNode() { }
        public LuaModelNode(string modelName)
        {
            this.modelName = modelName;
        }

        public string modelName { get; private set; }
        public override string ToString(LuaDocumentNode documentNode)
        {
            return $"module(\"{modelName}\", package.seeall)";
        }

        public override void Parse(string context)
        {
            modelName = context.Trim().RemoveString("module(\"").RemoveString("\", package.seeall)");
        }
    }

    /// <summary>
    /// Lua Function 定义
    /// </summary>
    public class LuaFunctionNode : LuaBaseStatementNode
    {
        public LuaFunctionNode() { }
        public LuaFunctionNode(string functionName, LuaMemberType memberType, List<string> functionArg = null,
            List<LuaBaseStatementNode> statementNodes = null)
        {
            this.functionName = functionName;
            this.memberType = memberType;
            this.functionArg = functionArg ?? new List<string>();
            this.statementNodes = statementNodes ?? new List<LuaBaseStatementNode>();
        }

        public string functionName { get; private set; }
        public List<string> functionArg { get; private set; }
        public LuaMemberType memberType { get; private set; }
        public List<LuaBaseStatementNode> statementNodes { get; private set; } = new List<LuaBaseStatementNode>();

        public override string ToString(LuaDocumentNode documentNode)
        {

            StringBuilder builder = new StringBuilder();
            builder.Append("function ");
            if (memberType == LuaMemberType.Local)
            {
                builder.Append(documentNode.ClassName).Append(":");
            }
            builder.Append(functionName).Append("(");
            if (functionArg != null && functionArg.Count > 0)
            {
                for (int i = 0; i < functionArg.Count; i++)
                {
                    if (i != 0)
                    {
                        builder.Append(",");
                    }
                    builder.Append(functionArg[i]);
                }
            }
            builder.Append($"){childNextLine}");
            builder.Append(childNextLine);
            if (statementNodes != null && statementNodes.Count > 0)
            {
                for (int i = 0; i < statementNodes.Count; i++)
                {
                    statementNodes[i].stateLayer = stateLayer + 1;
                }
                for (int i = 0; i < statementNodes.Count; i++)
                {
                    builder.Append(statementNodes[i].ToString(documentNode))
                        .Append(statementNodes[i].nextLine);
                }
            }
            builder.Append(nextLine);
            builder.Append($"end {LuaDocumentNode.Lua_FUNC_END}");


            return builder.ToString();
        }

        public override void Parse(string context)
        {
            context = context.Trim();
            string[] lines = context.Split('\n');
            var firstline = lines[0];
            var functionStart = firstline.IndexOf("function ") + "function ".Length;
            var argStart = firstline.IndexOf("(");
            var argEnd = firstline.IndexOf(")");

            functionName = firstline.Substring(functionStart, argStart - functionStart);
            memberType = LuaMemberType.Global;
            if (functionName.Contains(":"))
            {
                memberType = LuaMemberType.Local;
                functionName = functionName.Split(':')[1];
            }
            functionArg = new List<string>(firstline.Substring(argStart).RemoveString("(", ")").SplitAndTrim(','));
            statementNodes = new List<LuaBaseStatementNode>();
            for (int i = 1; i < lines.Length - 1; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;
                statementNodes.Add(new LuaScriptStatementNode(lines[i].Trim()));
            }

        }
    }

    /// <summary>
    /// Lua Field 定义
    /// </summary>
    public class LuaFieldNode : LuaBaseStatementNode
    {

        public LuaFieldNode() { }
        public LuaFieldNode(string fieldName, LuaMemberType memberType, LuaBaseStatementNode initStatementNode = null)
        {
            this.fieldName = fieldName;
            this.memberType = memberType;
            this.initStatementNode = initStatementNode;
        }

        public string fieldName { get; private set; }
        public LuaMemberType memberType { get; private set; }
        public LuaBaseStatementNode initStatementNode { get; private set; }

        public override string ToString(LuaDocumentNode documentNode)
        {
            StringBuilder builder = new StringBuilder();
            if (memberType == LuaMemberType.Local)
                builder.Append("local ");
            builder.Append(fieldName);
            if (initStatementNode != null)
            {
                builder.Append(" = ").Append(initStatementNode.ToString(documentNode));
            }
            return builder.ToString();
        }

        public override void Parse(string context)
        {
            context = context.Trim();
            memberType = context.StartsWith("local") ? LuaMemberType.Local : LuaMemberType.Global;
            context = context.RemoveString("local");
            if (!context.Contains("="))
            {
                fieldName = context.Trim();
            }
            else
            {
                fieldName = context.Split('=')[0].Trim();
                initStatementNode = new LuaScriptStatementNode(context.Split('=')[1].Trim());
            }

        }
    }

    /// <summary>
    /// Lua Property 定义
    /// </summary>
    public class LuaPropertyNode : LuaBaseStatementNode
    {
        public LuaPropertyNode() { }


        public LuaPropertyNode(string propertyName, LuaMemberType memberType,
            LuaBaseStatementNode initStatementNode = null)
        {
            this.propertyName = propertyName;
            this.initStatementNode = initStatementNode;
            this.memberType = memberType;
        }

        public string propertyName { get; private set; }
        public LuaBaseStatementNode initStatementNode { get; private set; }
        public LuaMemberType memberType { get; private set; }
        public List<LuaBaseStatementNode> getStatementNodes { get; private set; } = new List<LuaBaseStatementNode>();
        public List<LuaBaseStatementNode> setStatementNodes { get; private set; } = new List<LuaBaseStatementNode>();

        public string defualtGetStatement
        {
            get
            {
                StringBuilder getStatementBuilder = new StringBuilder();
                getStatementBuilder.Append("return ");
                if (memberType == LuaMemberType.Local)
                {
                    getStatementBuilder.Append("self.");
                }
                getStatementBuilder.Append($"_{propertyName}");
                return getStatementBuilder.ToString();
            }
        }

        public string defualtSetStatement
        {
            get
            {
                StringBuilder setStatementBuilder = new StringBuilder();
                if (memberType == LuaMemberType.Local)
                {
                    setStatementBuilder.Append("self.");
                }
                setStatementBuilder.Append($"_{propertyName} = {propertyName}");
                return setStatementBuilder.ToString();
            }
        }

        public override string ToString(LuaDocumentNode documentNode)
        {
            StringBuilder builder = new StringBuilder();

            //field
            LuaFieldNode field = new LuaFieldNode($"_{propertyName}", memberType, initStatementNode);
            builder.Append(field.ToString(documentNode)).Append(nextLine);

            //get function
            LuaFunctionNode getFunctionNode = new LuaFunctionNode($"get{propertyName}", memberType, null,
                new List<LuaBaseStatementNode>());



            if (getStatementNodes == null || getStatementNodes.Count == 0)
            {
                var getStatementNode = new LuaScriptStatementNode(defualtGetStatement);
                getFunctionNode.statementNodes.Add(getStatementNode);
            }
            for (int i = 0; i < getStatementNodes.Count; i++)
            {
                getFunctionNode.statementNodes.Add(getStatementNodes[i]);
            }


            builder.Append(getFunctionNode.ToString(documentNode)).Append(nextLine);

            //set function
            LuaFunctionNode setFunctionNode = new LuaFunctionNode($"set{propertyName}", memberType, new List<string>(),
                new List<LuaBaseStatementNode>());

            setFunctionNode.functionArg.Add(propertyName);

            if (setStatementNodes == null || setStatementNodes.Count == 0)
            {
                var setStatementNode = new LuaScriptStatementNode(defualtSetStatement);
                setFunctionNode.statementNodes.Add(setStatementNode);
            }

            for (int i = 0; i < setStatementNodes.Count; i++)
            {
                setFunctionNode.statementNodes.Add(setStatementNodes[i]);
            }


            builder.Append(setFunctionNode.ToString(documentNode));

            return builder.ToString();
        }

        public override void Parse(string context)
        {
            var lines = context.SplitAndTrim('\n');
            var firstLine = lines[0];

            //解析定义
            var field = new LuaFieldNode();
            field.Parse(firstLine);
            propertyName = field.fieldName.Substring(1);
            memberType = field.memberType;
            initStatementNode = field.initStatementNode;

            //解析get与set
            var getFuncBody = context.FindBetween($"get{propertyName}()", $"end {LuaDocumentNode.Lua_FUNC_END}");
            var getFuncLines = from line in getFuncBody.SplitAndTrim('\n')
                               where !string.IsNullOrEmpty(line.Trim())
                               select new LuaScriptStatementNode(line);
            getStatementNodes.AddRange(getFuncLines);

            context = context.FindAfter($"end {LuaDocumentNode.Lua_FUNC_END}");

            var setFuncBody = context.FindBetween($"set{propertyName}({propertyName})", $"end {LuaDocumentNode.Lua_FUNC_END}");
            var setFuncLines = (from line in setFuncBody.Split('\n')
                                where !string.IsNullOrEmpty(line.Trim())
                                select new LuaScriptStatementNode(line)).ToArray();
            setStatementNodes.AddRange(setFuncLines);
        }
    }



    /// <summary>
    /// 最基础的代码语句
    /// </summary>
    public class LuaScriptStatementNode : LuaBaseStatementNode
    {
        public LuaScriptStatementNode() { }
        public LuaScriptStatementNode(string script)
        {
            this.script = script;
        }

        public string script { get; private set; }

        public override string ToString(LuaDocumentNode documentNode)
        {
            return script.ToString();
        }

        public override void Parse(string context)
        {
            script = context.Trim();
        }
    }

    public class LuaCommentNode : LuaBaseStatementNode
    {
        public LuaCommentNode() { }
        public LuaCommentNode(string comment)
        {
            this.comment = comment;
        }

        public string comment { get; private set; }

        public override string ToString(LuaDocumentNode documentNode)
        {
            if (!comment.Contains("\n"))
            {
                return $"--{comment}";
            }
            return $"--[[{comment}]]--";
        }

        public override void Parse(string context)
        {
            if (context.Contains("\n"))
            {
                comment = context.Trim().Substring(2);
            }
            else
            {
                comment = context.Trim().Substring(3, context.Length - 6);
            }
        }
    }
}
