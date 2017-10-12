using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
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
	public class LuaDocumentNode {

		/// <summary>
		/// 类名
		/// </summary>
		public string ClassName { get; private set; }
		
		/// <summary>
		/// 定义类的方式
		/// </summary>
		public LuaBaseStatementNode ClassInitStatement { get; private set; }
		
		/// <summary>
		/// Lua自定义模块名称
		/// </summary>
		public LuaModelNode ModelNode { get; private set; }
		
		/// <summary>
		/// Lua请求的模块
		/// </summary>
		public readonly List<LuaRequireNode> RequireNodes = new List<LuaRequireNode>();
		
		/// <summary>
		/// Lua函数
		/// </summary>
		public readonly List<LuaFunctionNode> FunctionNodes = new List<LuaFunctionNode>();
		
		/// <summary>
		/// Lua值域
		/// </summary>
		public readonly List<LuaFieldNode> FieldNodes = new List<LuaFieldNode>();
		
		/// <summary>
		/// Lua属性
		/// </summary>
		public readonly List<LuaPropertyNode> PropertyNodes = new List<LuaPropertyNode>();
		
		/// <summary>
		/// Lua代码语句
		/// </summary>
		public readonly List<LuaBaseStatementNode> StatementNodes = new List<LuaBaseStatementNode>();

		/// <summary>
		/// 转成String
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			var documentNode = this;
			StringBuilder builder = new StringBuilder();
			
			//Requires
			builder.Append("--lua requires");
			documentNode.RequireNodes.ForEach((node) =>
			{
				builder.Append(node.ToString(documentNode))
					.Append(node.nextLine);
			});
			builder.Append("\n");

			//model
			builder.Append("--lua model");
			builder.Append(documentNode.ModelNode.ToString(documentNode))
				.Append(documentNode.ModelNode.nextLine);
			builder.Append("\n");

			//class define
			builder.Append("--lua class define");
			builder.Append(documentNode.ClassName).Append(" = ");
			builder.Append(documentNode.ClassInitStatement != null
					? documentNode.ClassInitStatement.ToString(documentNode)
					: "{}")
				.Append(documentNode.ModelNode.nextLine);
			builder.Append("\n");

			//lua fields
			builder.Append("--lua fields");
			documentNode.FieldNodes.ForEach((node) =>
			{
				builder.Append(node.ToString(documentNode))
					.Append(node.nextLine);
			});
			builder.Append("\n");
			
			//lua properties
			builder.Append("--lua properties");
			documentNode.PropertyNodes.ForEach((node) =>
			{
				builder.Append(node.ToString(documentNode))
					.Append(node.nextLine);
			});
			builder.Append("\n");
			
			//statements
			builder.Append("--lua statements");
			documentNode.StatementNodes.ForEach((node)=>
			{
				builder.Append(node.ToString(documentNode))
					.Append(node.nextLine);
			});
			builder.Append("\n");
			return builder.ToString();
		}
	}
	
	/// <summary>
	/// 基础的Lua语句
	/// </summary>
	public abstract class LuaBaseStatementNode
	{
		public int stateLayer { get; set; } = 0;
		public abstract string ToString(LuaDocumentNode documentNode);

		public string nextLine
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
		public LuaRequireNode(string requireName)
		{
			this.requireName = requireName;
		}

		public string requireName { get; private set; }
		
		public override string ToString(LuaDocumentNode documentNode)
		{
			return $"require \"{requireName}\"";
		}
	}

	/// <summary>
	/// Lua Model语句
	/// </summary>
	public class LuaModelNode : LuaBaseStatementNode
	{
		public LuaModelNode(string modelName)
		{
			this.modelName = modelName;
		}

		public string modelName { get; private set; }
		public override string ToString(LuaDocumentNode documentNode)
		{
			return $"module(\"{modelName}\", package.seeall)";
		}
	}

	/// <summary>
	/// Lua Function 定义
	/// </summary>
	public class LuaFunctionNode : LuaBaseStatementNode
	{
		public LuaFunctionNode(string functionName, LuaMemberType memberType, List<string> functionArg = null, 
			List<LuaBaseStatementNode> statementNodes = null)
		{
			this.functionName = functionName;
			this.memberType = memberType;
			this.statementNodes = statementNodes;
		}

		public string functionName { get; private set; }
		public List<string> functionArg { get; private set; }
		public LuaMemberType memberType { get; private set; }
		public List<LuaBaseStatementNode> statementNodes { get; private set; } = new List<LuaBaseStatementNode>();

		public override string ToString(LuaDocumentNode documentNode)
		{
			for (int i = 0; i < statementNodes.Count; i++)
			{
				statementNodes[i].stateLayer = stateLayer + 1;
			}
			
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
			builder.Append($"){nextLine}");
			if (functionArg != null && functionArg.Count > 0)
			{
				for (int i = 0; i < statementNodes.Count; i++)
				{
					builder.Append(statementNodes[i]).Append(nextLine);
				}
			}
			builder.Append("end");
			return builder.ToString();
		}
	}

	/// <summary>
	/// Lua Field 定义
	/// </summary>
	public class LuaFieldNode : LuaBaseStatementNode
	{
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
				builder.Append("= ").Append(initStatementNode.ToString(documentNode));
			}
			return builder.ToString();
		}
	}

	/// <summary>
	/// Lua Property 定义
	/// </summary>
	public class LuaPropertyNode : LuaBaseStatementNode
	{
		public string propertyName { get; private set; }
		public LuaBaseStatementNode initStatementNode { get; private set; }
		public LuaMemberType memberType { get; private set; }
		public List<LuaBaseStatementNode> getStatementNodes { get; private set; } = new List<LuaBaseStatementNode>();
		public List<LuaBaseStatementNode> setStatementNodes { get; private set; } = new List<LuaBaseStatementNode>();

		public override string ToString(LuaDocumentNode documentNode)
		{
			StringBuilder builder = new StringBuilder();
			
			//field
			LuaFieldNode field = new LuaFieldNode($"_{propertyName}", memberType, initStatementNode);
			builder.Append(field.ToString(documentNode)).Append(nextLine);
			
			//get function
			LuaFunctionNode getFunctionNode = new LuaFunctionNode($"get{propertyName}", memberType, null, 
				new List<LuaBaseStatementNode>());
			
			StringBuilder getStatementBuilder = new StringBuilder();
			getStatementBuilder.Append("return ");
			if (memberType == LuaMemberType.Local)
			{
				getStatementBuilder.Append("self.");
			}
			getStatementBuilder.Append($"_{propertyName}");
			var getStatementNode = new LuaScriptStatementNode(getStatementBuilder.ToString());
			getFunctionNode.statementNodes.Add(getStatementNode);
			
			builder.Append(getFunctionNode.ToString(documentNode)).Append(nextLine);
			
			//set function
			LuaFunctionNode setFunctionNode = new LuaFunctionNode($"set{propertyName}", memberType, new List<string>(), 
				new List<LuaBaseStatementNode>());

			setFunctionNode.functionArg.Add(propertyName);
			
			StringBuilder setStatementBuilder = new StringBuilder();
			if (memberType == LuaMemberType.Local)
			{
				getStatementBuilder.Append("self.");
			}
			setStatementBuilder.Append($"_{propertyName} = {propertyName}");
			var setStatementNode = new LuaScriptStatementNode(setStatementBuilder.ToString());
			setStatementNodes.Add(setStatementNode);
			
			builder.Append(setStatementNode.ToString(documentNode)).Append(nextLine);
			
			return builder.ToString();
		}
	}

	

	/// <summary>
	/// 最基础的代码语句
	/// </summary>
	public class LuaScriptStatementNode : LuaBaseStatementNode
	{
		public LuaScriptStatementNode(string script)
		{
			this.script = script;
		}

		public string script { get; private set; }

		public override string ToString(LuaDocumentNode documentNode)
		{
			return script.ToString();
		}
	}

	public class LuaCommentNode : LuaBaseStatementNode
	{
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
			return $"--[[{comment}]]";
		}
	}
}
