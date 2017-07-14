/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Tencent is pleased to support the open source community by making behaviac available.
//
// Copyright (C) 2015 THL A29 Limited, a Tencent company. All rights reserved.
//
// Licensed under the BSD 3-Clause License (the "License"); you may not use this file except in
// compliance with the License. You may obtain a copy of the License at http://opensource.org/licenses/BSD-3-Clause
//
// Unless required by applicable law or agreed to in writing, software distributed under the License
// is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
// or implied. See the License for the specific language governing permissions and limitations under
// the License.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#if !BEHAVIAC_RELEASE
#define BEHAVIAC_DEBUG
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace behaviac
{
    public struct CStringID
    {
        private uint m_id;

        public CStringID(string str)
        {
            //SetId(str);
            m_id = CRC32.CalcCRC(str);
        }

        public void SetId(string str)
        {
            m_id = CRC32.CalcCRC(str);
        }

        public uint GetId()
        {
            return m_id;
        }

        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to CStringID return false.
            if (obj is CStringID)
            {
                CStringID p = (CStringID)obj;

                // Return true if the fields match:
                return (m_id == p.m_id);
            }
            else if (obj is string)
            {
                CStringID p = new CStringID((string)obj);

                // Return true if the fields match:
                return (m_id == p.m_id);
            }

            return false;
        }

        public bool Equals(CStringID p)
        {
            // If parameter is null return false:
            if (((object)p) == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (m_id == p.m_id);
        }

        public override int GetHashCode()
        {
            return (int)m_id;
        }

        public static bool operator ==(CStringID a, CStringID b)
        {
            return a.m_id == b.m_id;
        }

        public static bool operator !=(CStringID a, CStringID b)
        {
            return a.m_id != b.m_id;
        }
    }

    public struct CSerializationID
    {
        public CSerializationID(string id)
        {
        }
    }

    public interface ISerializableNode
    {
        ISerializableNode newChild(CSerializationID chidlId);

        void setAttr(CSerializationID attrId, string value);

        void setAttr<VariableType>(CSerializationID attrId, VariableType value);
    }

    public struct CPropertyNode
    {
        public CPropertyNode(Agent agent, string className)
        {
        }
    }

    public class CMethodBase
    {
        private behaviac.MethodMetaInfoAttribute descAttrbute_;
        private System.Reflection.MethodBase method_;

        public CMethodBase(System.Reflection.MethodBase m, behaviac.MethodMetaInfoAttribute a, string methodNameOverride)
        {
            method_ = m;
            descAttrbute_ = a;
            this.m_variableName = !string.IsNullOrEmpty(methodNameOverride) ? methodNameOverride : method_.Name;
            m_id.SetId(this.m_variableName);
        }

        protected CMethodBase(CMethodBase copy)
        {
            this.m_variableName = copy.m_variableName;
            this.m_instanceName = copy.m_instanceName;
            this.method_ = copy.method_;
            this.descAttrbute_ = copy.descAttrbute_;
            this.m_id = copy.m_id;
            Debug.Check(copy.m_params == null);
        }

        protected string m_instanceName;
        protected string m_variableName;

        public string Name
        {
            get
            {
                return this.m_variableName;
            }
        }

        private CStringID m_id = new CStringID();

        public CStringID GetId()
        {
            return m_id;
        }

        public string InstanceName
        {
            get
            {
                return this.m_instanceName;
            }
            set
            {
                this.m_instanceName = value;
            }
        }

        public virtual int ParamsCount
        {
            get
            {
                return this.method_ != null ? this.method_.GetParameters().Length : 0;
            }
        }

        public virtual List<Type> ParamTypes
        {
            get
            {
                List<Type> paramTypes = null;

                if (this.method_ != null)
                {
                    System.Reflection.ParameterInfo[] parameters = this.method_.GetParameters();
                    int paramsCount = parameters.Length;
                    paramTypes = new List<Type>();

                    for (int i = 0; i < paramsCount; ++i)
                    {
                        paramTypes.Add(parameters[i].ParameterType);
                    }
                }
                else
                {
                    int paramsCount = this.m_params.Length;

                    for (int i = 0; i < paramsCount; ++i)
                    {
                        paramTypes.Add(this.m_params[i].paramProperty.PropertyType);
                    }
                }

                return paramTypes;
            }
        }

        public Agent GetParentAgent(Agent pAgent)
        {
            return Utils.GetParentAgent(pAgent, this.m_instanceName);
        }

        public static Param_t[] LoadParams(List<string> paramsToken, List<Type> parameters, ref object[] _params_value)
        {
            int paramCount = parameters != null ? parameters.Count : paramsToken.Count;
            Param_t[] _params = null;

            if (paramCount > 0)
            {
                _params = new Param_t[paramCount];
                _params_value = new object[paramCount];

                for (int i = 0; i < paramsToken.Count; ++i)
                {
                    if (parameters != null)
                    {
                        Type paramType = parameters[i];

                        bool isStruct = paramsToken[i][0] == '{';

                        if (isStruct)
                        {
                            Dictionary<string, Property> props = new Dictionary<string, Property>();
                            string strT = "";

                            if (StringUtils.ParseForStruct(paramType, paramsToken[i], ref strT, props))
                            {
                                object paramValue = StringUtils.FromString(paramType, strT, false);

                                _params_value[i] = paramValue;
                                _params[i].paramStructMembers = props;
                            }
                        }
                        else
                        {
                            //paramsToken[i][0] = '"', then it is a string
                            bool isString = paramsToken[i][0] == '"';

                            if (isString || paramsToken[i].IndexOf(' ') == -1)
                            {
                                //" is paired
                                Debug.Check(!isString || paramsToken[i][paramsToken[i].Length - 1] == '"');
                                string paramStr = isString ? paramsToken[i].Substring(1, paramsToken[i].Length - 2) : paramsToken[i];

                                object paramValue = StringUtils.FromString(paramType, paramStr, false);

                                _params_value[i] = paramValue;
                            }
                            else
                            {
                                ParseMethodParamProperty(paramsToken[i], _params, i);
                            }
                        }

                        _params[i].isRefOut = paramType.IsByRef;
                    }
                    else
                    {
                        ParseMethodParamProperty(paramsToken[i], _params, i);
                    }
                }//end of for
            }

            return _params;
        }

        private static void ParseMethodParamProperty(string paramToken, Param_t[] _params, int i)
        {
            string typeName = null;
            Property paramProperty = Condition.ParseProperty(paramToken, ref typeName);
            _params[i].paramProperty = paramProperty;
        }

        public void Load(List<string> paramsToken)
        {
            int paramsCount = this.ParamsCount;

            if (paramsCount > 0)
            {
                if (paramsToken.Count == paramsCount)
                {
                    this.m_params = LoadParams(paramsToken, this.ParamTypes, ref this.m_params_value);
                }
                else
                {
                    Debug.Check(false);
                }
            }
        }

        public bool IsStatic()
        {
            return false;
        }

        public virtual string GetClassNameString()
        {
            return this.method_.DeclaringType.FullName;
        }

        public virtual bool IsNamedEvent()
        {
            return false;
        }

        public virtual CMethodBase clone()
        {
            return new CMethodBase(this);
        }

        public struct Param_t
        {
            public bool isRefOut;
            public Property paramProperty;
            public Dictionary<string, Property> paramStructMembers;
        };

        protected Param_t[] m_params = null;
        protected object[] m_params_value = null;

        public void GetParamsValue(Agent pAgent, bool bAllowNullValue)
        {
            Agent pParent = this.GetParentAgent(pAgent);

            this.GetParamsValue(pParent, pAgent, bAllowNullValue);
        }

        private object run(Agent parent, Agent pSelf)
        {
            this.GetParamsValue(parent, pSelf, true);

            object returnValue = this.method_.Invoke(parent, this.m_params_value);

            this.UpdateAgentValue(pSelf);

            return returnValue;
        }

        private void UpdateAgentValue(Agent pSelf)
        {
            if (this.m_params != null)
            {
                for (int i = 0; i < this.m_params.Length; ++i)
                {
                    if (this.m_params[i].isRefOut)
                    {
                        Property property = this.m_params[i].paramProperty;

                        if (property != null)
                        {
                            Agent pAgent = property.GetParentAgent(pSelf);
                            property.SetValue(pAgent, this.m_params_value[i]);
                        }

                        if (this.m_params[i].paramStructMembers != null)
                        {
                            Type structType = this.m_params_value[i].GetType();
                            Agent.CTagObjectDescriptor objectDesc = Agent.GetDescriptorByName(structType.FullName);

                            var e = this.m_params[i].paramStructMembers.GetEnumerator();
                            while (e.MoveNext())
                            {
                                CMemberBase member = objectDesc.GetMember(e.Current.Key);
                                object v = member.Get(this.m_params_value[i]);

                                Agent pAgent = e.Current.Value.GetParentAgent(pSelf);
                                e.Current.Value.SetValue(pAgent, v);
                            }
                        }
                    }//end of if isRefOut
                }
            }
        }

        private void GetParamsValue(Agent parent, Agent pSelf, bool bAllowNullValue)
        {
            if (this.m_params != null)
            {
                Debug.Check(this.m_params_value != null && this.m_params_value.Length == this.m_params.Length);

                for (int i = 0; i < this.m_params.Length; ++i)
                {
                    Property property = this.m_params[i].paramProperty;

                    if (property != null)
                    {
                        object v = property.GetValue(parent, pSelf);

                        if (this.m_params_value[i] == null || !this.m_params_value[i].GetType().IsValueType)
                        {
                            Utils.Clone(ref this.m_params_value[i], v);
                        }
                        else
                        {
                            this.m_params_value[i] = v;
                        }

                        Debug.Check(bAllowNullValue || this.m_params_value[i] != null);
                    }

                    //Debug.Check(this.m_this.m_params[i].value != null);

                    if (this.m_params[i].paramStructMembers != null)
                    {
                        Type structType = this.m_params_value[i].GetType();
                        Agent.CTagObjectDescriptor objectDesc = Agent.GetDescriptorByName(structType.FullName);

                        var e = this.m_params[i].paramStructMembers.GetEnumerator();
                        while (e.MoveNext())
                        {
                            CMemberBase member = objectDesc.GetMember(e.Current.Key);

                            object v = e.Current.Value.GetValue(parent, pSelf);

                            member.Set(this.m_params_value[i], v);
                        }
                    }
                }
            }
        }

        public object Invoke(Agent pParent, Agent pAgent)
        {
            object returnValue = this.run(pParent, pAgent);

            return returnValue;
        }

        public object Invoke(Agent pAgent)
        {
            Agent pParent = this.GetParentAgent(pAgent);

            object returnValue = this.run(pParent, pAgent);

            return returnValue;
        }

        public object Invoke(Agent pSelf, object param)
        {
            Agent pParent = this.GetParentAgent(pSelf);

            Debug.Check(this.m_params_value.Length == 1);

            this.m_params_value[0] = param;

            object returnValue = this.method_.Invoke(pParent, this.m_params_value);
            //this.UpdateAgentValue(pSelf);

            return returnValue;
        }

        public object Invoke(Agent pSelf, object[] paramsValue)
        {
            Agent pParent = this.GetParentAgent(pSelf);

            for (int i = 0; i < paramsValue.Length; ++i)
            {
                this.m_params_value[i] = paramsValue[i];
            }

            object returnValue = this.method_.Invoke(pParent, this.m_params_value);

            //this.UpdateAgentValue(pSelf);

            return returnValue;
        }
    }

    public class CMemberBase
    {
        //behaviac.MemberMetaInfoAttribute descAttrbute_;
        protected string m_name;

        public CMemberBase(string name, behaviac.MemberMetaInfoAttribute a)
        {
            m_name = name;
            m_id.SetId(name);

            if (a != null)
            {
                m_range = a.Range;
            }
            else
            {
                m_range = 1.0f;
            }
        }

        public virtual Type MemberType
        {
            get
            {
                return null;
            }
        }

        public virtual bool ISSTATIC()
        {
            return false;
        }

        private float m_range = 1.0f;

        public float GetRange()
        {
            return this.m_range;
        }

        private CStringID m_id = new CStringID();

        public CStringID GetId()
        {
            return m_id;
        }

        public string Name
        {
            get
            {
                return this.m_name;
            }
        }

        public virtual string GetClassNameString()
        {
            return null;
        }

        private string m_instaceName;

        public string InstanceName
        {
            get
            {
                return m_instaceName;
            }
            set
            {
                m_instaceName = value;
            }
        }

        public Property CreateProperty(string defaultValue, bool bConst)
        {
            Property p = new Property(this, bConst);

            if (!string.IsNullOrEmpty(defaultValue))
            {
                p.SetDefaultValue(defaultValue);
            }

            return p;
        }

        public virtual int GetTypeId()
        {
            return 0;
        }

        public virtual void Load(Agent parent, ISerializableNode node)
        {
        }

        public virtual void Save(Agent parent, ISerializableNode node)
        {
        }

        public virtual object Get(object agentFrom)
        {
            return null;
        }

        public virtual void Set(object objectFrom, object v)
        {
        }
    }

    public class CFieldInfo : CMemberBase
    {
        protected System.Reflection.FieldInfo field_;

        public CFieldInfo(System.Reflection.FieldInfo f, behaviac.MemberMetaInfoAttribute a)
            : base(f.Name, a)
        {
            this.field_ = f;
        }

        public override Type MemberType
        {
            get
            {
                return field_.FieldType;
            }
        }

        public override bool ISSTATIC()
        {
            return this.field_.IsStatic;
        }

        public override string GetClassNameString()
        {
            return this.field_.DeclaringType.FullName;
        }

        public override object Get(object agentFrom)
        {
            if (this.ISSTATIC())
            {
                return this.field_.GetValue(null);
            }
            else
            {
                return this.field_.GetValue(agentFrom);
            }
        }

        public override void Set(object objectFrom, object v)
        {
            this.field_.SetValue(objectFrom, v);
        }
    }

    public class CProperyInfo : CMemberBase
    {
        private PropertyInfo property_;

        private static MethodInfo getGetMethod(PropertyInfo property)
        {
#if ( !UNITY_EDITOR && UNITY_METRO )
            return property.GetMethod;
#else
            return property.GetGetMethod();
#endif
        }

        private static MethodInfo getSetMethod(PropertyInfo property)
        {
#if ( !UNITY_EDITOR && UNITY_METRO )
            return property.SetMethod;
#else
            return property.GetSetMethod();
#endif
        }

        public CProperyInfo(System.Reflection.PropertyInfo p, behaviac.MemberMetaInfoAttribute a)
            : base(p.Name, a)
        {
            this.property_ = p;
        }

        public override Type MemberType
        {
            get
            {
                return this.property_.PropertyType;
            }
        }

        public override string GetClassNameString()
        {
            return this.property_.DeclaringType.FullName;
        }

        public override object Get(object agentFrom)
        {
            if (this.ISSTATIC())
            {
                var getMethod = getGetMethod(this.property_);
                return getMethod.Invoke(null, null);
            }
            else
            {
                var getMethod = getGetMethod(this.property_);
                return getMethod.Invoke(agentFrom, null);
            }
        }

        public override void Set(object objectFrom, object v)
        {
            var setMethod = getSetMethod(this.property_);

            if (setMethod == null)
            {
            }
            else
            {
                var paramArray = new object[1];
                paramArray[0] = v;
                setMethod.Invoke(objectFrom, paramArray);
            }
        }
    }

    public class CCustomMethod : CMethodBase
    {
        private string m_className;
        protected List<Type> m_paramTypes = null;

        public CCustomMethod(string className, string eventName)
            : base(null, null, eventName)
        {
            m_className = className;
        }

        protected CCustomMethod(CCustomMethod copy)
            : base(copy)
        {
            m_className = copy.m_className;
            m_paramTypes = copy.m_paramTypes;
        }

        public void AddParamType(string typeName)
        {
            Type type = Utils.GetTypeFromName(typeName);

            if (this.m_paramTypes == null)
            {
                this.m_paramTypes = new List<Type>();
            }

            this.m_paramTypes.Add(type);
        }

        public override int ParamsCount
        {
            get
            {
                if (this.m_paramTypes != null)
                {
                    return this.m_paramTypes.Count;
                }

                return 0;
            }
        }

        public override List<Type> ParamTypes
        {
            get
            {
                return this.m_paramTypes;
            }
        }

        public override string GetClassNameString()
        {
            return m_className;
        }
    }

    public class CNamedEvent : CCustomMethod
    {
        public CNamedEvent(string className, string eventName)
            : base(className, eventName)
        {
        }

        protected CNamedEvent(CNamedEvent copy)
            : base(copy)
        {
        }

        public override CMethodBase clone()
        {
            return new CNamedEvent(this);
        }

        public override bool IsNamedEvent()
        {
            return true;
        }

        private bool m_bFired;

        public bool IsFired()
        {
            return this.m_bFired;
        }

        public void SetFired(Agent pAgent, bool bFired)
        {
            this.m_bFired = bFired;

            if (bFired)
            {
                pAgent.btonevent(this.Name);
            }
        }

        public void SetParam<ParamType>(Agent pAgent, ParamType param)
        {
            Debug.Check(this.m_paramTypes != null && m_paramTypes.Count == 1);
            Debug.Check(this.m_paramTypes[0] == typeof(ParamType) || typeof(ParamType).IsSubclassOf(this.m_paramTypes[0]), "SetParam's Param is not compatible");

            //string eventName = string.Format("{0}_param0", this.Name);
            AgentState currentState = pAgent.Variables.Push(false);
            Debug.Check(currentState != null);

            string eventName = string.Format("{0}{1}", Task.LOCAL_TASK_PARAM_PRE, 0);

            //AgentProperties agentT = AgentProperties.Get(pAgent.GetClassTypeName());
            pAgent.SetVariable(eventName, param);
        }

        public void SetParam<ParamType1, ParamType2>(Agent pAgent, ParamType1 param1, ParamType2 param2)
        {
            Debug.Check(this.m_paramTypes != null && m_paramTypes.Count == 2);
            Debug.Check(this.m_paramTypes[0] == typeof(ParamType1) || typeof(ParamType1).IsSubclassOf(this.m_paramTypes[0]), "SetParam's Param1 is not compatible");
            Debug.Check(this.m_paramTypes[1] == typeof(ParamType2) || typeof(ParamType2).IsSubclassOf(this.m_paramTypes[1]), "SetParam's Param2 is not compatible");

            AgentState currentState = pAgent.Variables.Push(false);
            Debug.Check(currentState != null);

            //AgentProperties agentT = AgentProperties.Get(pAgent.GetClassTypeName());

            string eventName1 = string.Format("{0}{1}", Task.LOCAL_TASK_PARAM_PRE, 0);
            pAgent.SetVariable(eventName1, param1);
            string eventName2 = string.Format("{0}{1}", Task.LOCAL_TASK_PARAM_PRE, 1);
            pAgent.SetVariable(eventName2, param2);
        }

        public void SetParam<ParamType1, ParamType2, ParamType3>(Agent pAgent, ParamType1 param1, ParamType2 param2, ParamType3 param3)
        {
            Debug.Check(this.m_paramTypes != null && m_paramTypes.Count == 3);
            Debug.Check(this.m_paramTypes[0] == typeof(ParamType1) || typeof(ParamType1).IsSubclassOf(this.m_paramTypes[0]), "SetParam's Param1 is not compatible");
            Debug.Check(this.m_paramTypes[1] == typeof(ParamType2) || typeof(ParamType2).IsSubclassOf(this.m_paramTypes[1]), "SetParam's Param2 is not compatible");
            Debug.Check(this.m_paramTypes[2] == typeof(ParamType3) || typeof(ParamType3).IsSubclassOf(this.m_paramTypes[2]), "SetParam's Param3 is not compatible");

            AgentState currentState = pAgent.Variables.Push(false);
            Debug.Check(currentState != null);

            //AgentProperties agentT = AgentProperties.Get(pAgent.GetClassTypeName());

            string eventName1 = string.Format("{0}{1}", Task.LOCAL_TASK_PARAM_PRE, 0);
            pAgent.SetVariable(eventName1, param1);
            string eventName2 = string.Format("{0}{1}", Task.LOCAL_TASK_PARAM_PRE, 1);
            pAgent.SetVariable(eventName2, param2);
            string eventName3 = string.Format("{0}{1}", Task.LOCAL_TASK_PARAM_PRE, 2);
            pAgent.SetVariable(eventName3, param3);
        }
    }

    public class CTaskMethod : CNamedEvent
    {
        public CTaskMethod(string className, string eventName)
            : base(className, eventName)
        {
        }

        private CTaskMethod(CTaskMethod copy)
            : base(copy)
        {
        }

        public override CMethodBase clone()
        {
            return new CTaskMethod(this);
        }

        public void SetTaskParams(Agent pAgent)
        {
            if (this.ParamsCount > 0)
            {
                this.GetParamsValue(pAgent, false);

                string agentType = pAgent.GetClassTypeName();
                Debug.Check(agentType != null);
                AgentProperties agentT = AgentProperties.Get(agentType);

                for (int i = 0; i < this.m_params_value.Length; ++i)
                {
                    string paramName = string.Format("{0}{1}", Task.LOCAL_TASK_PARAM_PRE, i);
                    CTaskMethod.SetTaskParam(pAgent, agentT, paramName, this.m_params_value[i]);
                }
            }
        }

        private static void SetTaskParam(Agent pAgent, AgentProperties agentT, string paramName, object paramValu)
        {
            //pAgent.SetVariable(paramName, parasValue[i]);
            Property localProperty = agentT.GetLocal(paramName);
            localProperty.SetValue(pAgent, paramValu);
        }
    }

    public enum E_VariableComparisonType
    {
        VariableComparisonType_Assignment,      //( "Assignment (=)" )
        VariableComparisonType_Equal,           //( "Equal (==)" )
        VariableComparisonType_NotEqual,        //( "Not Equal (!=)" )
        VariableComparisonType_Greater,         //( "Greater (>)"  )
        VariableComparisonType_GreaterEqual,    //( "Greater Or Equal (>=)" )
        VariableComparisonType_Less,            //( "Lower (<)"  )
        VariableComparisonType_LessEqual,       //( "Lower Or Equal (<=)" )
        VariableComparisonType_And,             //( "Lower Or Equal (&&)" )
        VariableComparisonType_Or               //( "Lower Or Equal (||)" )
    }

    public class VariableComparator
    {
        public static E_VariableComparisonType ParseComparisonType(string comparionOperator)
    {
        if (comparionOperator == "Assignment")
        {
            return E_VariableComparisonType.VariableComparisonType_Assignment;
        }
        else if (comparionOperator == "Equal")
        {
            return E_VariableComparisonType.VariableComparisonType_Equal;
        }
        else if (comparionOperator == "NotEqual")
        {
            return E_VariableComparisonType.VariableComparisonType_NotEqual;
        }
        else if (comparionOperator == "Greater")
        {
            return E_VariableComparisonType.VariableComparisonType_Greater;
        }
        else if (comparionOperator == "GreaterEqual")
        {
            return E_VariableComparisonType.VariableComparisonType_GreaterEqual;
        }
        else if (comparionOperator == "Less")
        {
            return E_VariableComparisonType.VariableComparisonType_Less;
        }
        else if (comparionOperator == "LessEqual")
        {
            return E_VariableComparisonType.VariableComparisonType_LessEqual;
        }
        else
        {
            Debug.Check(false);
        }

        return E_VariableComparisonType.VariableComparisonType_Equal;
    }

    public VariableComparator(Property lhs, CMethodBase lhs_m, Property rhs, CMethodBase rhs_m)
    {
        m_lhs = lhs;
        m_lhs_m = lhs_m;
        m_rhs = rhs;
        m_rhs_m = rhs_m;
    }

    private VariableComparator(VariableComparator copy)
    {
        m_lhs = copy.m_lhs;
        m_lhs_m = copy.m_lhs_m;
        m_rhs = copy.m_rhs;
        m_rhs_m = copy.m_rhs_m;
    }

    ~VariableComparator()
    {
        m_lhs = null;
        m_lhs_m = null;
        m_rhs = null;
        m_rhs_m = null;
    }

    public VariableComparator clone()
    {
        return new VariableComparator(this);
    }

    public static VariableComparator Create(Property lhs, CMethodBase lhs_m, Property rhs, CMethodBase rhs_m)
    {
        return new VariableComparator(lhs, lhs_m, rhs, rhs_m);
    }

    public bool Execute(Agent agent)
    {
        object lhs = null;

        Agent agentL = agent;

        if (this.m_lhs != null)
        {
            agentL = this.m_lhs.GetParentAgent(agentL);
            lhs = this.m_lhs.GetValue(agentL);
        }
        else
        {
            Debug.Check(this.m_lhs_m != null);
            lhs = this.m_lhs_m.Invoke(agentL);
        }

        object rhs = null;

        Agent agentR = agent;

        if (this.m_rhs != null)
        {
            agentR = this.m_rhs.GetParentAgent(agentR);
            rhs = this.m_rhs.GetValue(agentR);
        }
        else
        {
            Debug.Check(this.m_rhs_m != null);
            rhs = this.m_rhs_m.Invoke(agentR);
        }

        switch (this.m_comparisonType)
        {
            case E_VariableComparisonType.VariableComparisonType_Assignment:
                Debug.Check(this.m_lhs != null);
                //this.m_lhs.SetFrom(agentR, this.m_rhs, agentL);
                this.m_lhs.SetValue(agentL, rhs);

                return true;

            case E_VariableComparisonType.VariableComparisonType_Equal:
                return Details.Equals_(lhs, rhs);

            case E_VariableComparisonType.VariableComparisonType_NotEqual:
                return !Details.Equals_(lhs, rhs);

            case E_VariableComparisonType.VariableComparisonType_Greater:
                return Details.Greater(lhs, rhs);

            case E_VariableComparisonType.VariableComparisonType_GreaterEqual:
                return Details.GreaterEqual(lhs, rhs);

            case E_VariableComparisonType.VariableComparisonType_Less:
                return Details.Less(lhs, rhs);

            case E_VariableComparisonType.VariableComparisonType_LessEqual:
                return Details.LessEqual(lhs, rhs);

            default:
                Debug.Check(false, "Unsupported comparison type");
                break;
        }

        return false;
    }

    private void SetProperty(Property lhs, Property rhs)
    {
        m_lhs = lhs;
        m_rhs = rhs;
    }

    public void SetComparisonType(E_VariableComparisonType type)
    {
        m_comparisonType = type;
    }

    private E_VariableComparisonType GetComparisonType()
    {
        return m_comparisonType;
    }

    protected Property m_lhs;
    protected CMethodBase m_lhs_m;
    protected Property m_rhs;
    protected CMethodBase m_rhs_m;

    //The operator used in the comparison
    protected E_VariableComparisonType m_comparisonType;
    }

    public class CTextNode
    {
        public CTextNode(string name)
        {
        }

        public void setAttr(string attrName, string attrValue)
        {
        }
    }

    static public class Utils
    {
        public static bool IsNull(System.Object aObj)
        {
            return aObj == null;
        }

        public static bool IsStaticType(Type type)
        {
            return type != null && type.IsAbstract && type.IsSealed;
        }

        private static Dictionary<string, bool> ms_staticClasses;

        private static Dictionary<string, bool> StaticClasses
        {
            get
            {
                if (ms_staticClasses == null)
                {
                    ms_staticClasses = new Dictionary<string, bool>();
                }

                return ms_staticClasses;
            }
        }

        public static void AddStaticClass(Type type)
        {
            if (Utils.IsStaticType(type))
            {
                Utils.StaticClasses[type.FullName] = true;
            }
        }

        public static bool IsStaticClass(string className)
        {
            return Utils.StaticClasses.ContainsKey(className);
        }

        public static Agent GetParentAgent(Agent pAgent, string instanceName)
        {
            Agent pParent = pAgent;

            if (!string.IsNullOrEmpty(instanceName) && instanceName != "Self")
            {
                pParent = Agent.GetInstance(instanceName, (pParent != null) ? pParent.GetContextId() : 0);

                if (pAgent != null && pParent == null && !Utils.IsStaticClass(instanceName))
                {
                    pParent = pAgent.GetVariable<Agent>(instanceName);
                    Debug.Check(pParent != null);
                }
            }

            return pParent;
        }

        public static bool IsDefault<T>(T obj)
        {
            return EqualityComparer<T>.Default.Equals(obj, default(T));
        }

        //public static bool IsNull(UnityEngine.Object p)
        //{
        //    bool isNull = !p;
        //    return isNull;
        //}

        public static uint MakeVariableId(string idstring)
        {
            return CRC32.CalcCRC(idstring);
        }

        public static int GetClassTypeNumberId<T>()
        {
            return 0;
        }

        public static void ConvertFromInteger<T>(int v, ref T ret)
        {
        }

        public static uint ConvertToInteger<T>(T v)
        {
            return 0;
        }

        public static Type GetType(string typeName)
        {
            // search base class
            Type type = Type.GetType(typeName);

            if (type != null)
            {
                return type;
            }

            // search loaded plugins
            for (int i = 0; i < AppDomain.CurrentDomain.GetAssemblies().Length; ++i)
            {
                Assembly a = AppDomain.CurrentDomain.GetAssemblies()[i];
                type = a.GetType(typeName);

                if (type != null)
                {
                    return type;
                }
            }

            // it could be a List<> type
            if (typeName.StartsWith("System.Collections.Generic.List"))
            {
                int startIndex = typeName.IndexOf("[[");

                if (startIndex > -1)
                {
                    int endIndex = typeName.IndexOf(",");

                    if (endIndex < 0)
                    {
                        endIndex = typeName.IndexOf("]]");
                    }

                    if (endIndex > startIndex)
                    {
                        string item = typeName.Substring(startIndex + 2, endIndex - startIndex - 2);
                        type = Utils.GetType(item);

                        if (type != null)
                        {
                            type = typeof(List<>).MakeGenericType(type);
                            return type;
                        }
                    }
                }
            }

            return null;
        }

        private static Dictionary<string, string> ms_type_mapping = new Dictionary<string, string>()
        {
            {"Boolean"          , "bool"},
            {"System.Boolean"   , "bool"},
            {"Int32"            , "int"},
            {"System.Int32"     , "int"},
            {"UInt32"           , "uint"},
            {"System.UInt32"    , "uint"},
            {"Int16"            , "short"},
            {"System.Int16"     , "short"},
            {"UInt16"           , "ushort"},
            {"System.UInt16"    , "ushort"},
            {"Int8"             , "sbyte"},
            {"System.Int8"      , "sbyte"},
            {"SByte"            , "sbyte"},
            {"System.SByte"     , "sbyte"},
            {"UInt8"            , "ubyte"},
            {"System.UInt8"     , "ubyte"},
            {"Byte"             , "ubyte"},
            {"System.Byte"      , "ubyte"},
            {"Char"      		, "char"},
            {"System.Char"      , "char"},
            {"Int64"            , "long"},
            {"System.Int64"     , "long"},
            {"UInt64"           , "ulong"},
            {"System.UInt64"    , "ulong"},
            {"Single"           , "float"},
            {"System.Single"    , "float"},
            {"Double"           , "double"},
            {"System.Double"    , "double"},
            {"String"           , "string"},
            {"System.String"    , "string"},
            {"Void"             , "void"},
            {"System.Void"      , "void"}
        };

        private static object GetDefaultValue(Type t)
        {
            if (t.IsValueType)
            {
                return Activator.CreateInstance(t);
            }

            return null;
        }

        public static object FromStringPrimitive(Type type, string valueStr)
        {
            if (valueStr != null)
            {
                if (type == typeof(string) && !string.IsNullOrEmpty(valueStr) &&
                    valueStr.Length > 1 && valueStr[0] == '\"' && valueStr[valueStr.Length - 1] == '\"')
                {
                    valueStr = valueStr.Substring(1, valueStr.Length - 2);
                }

                try
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(type);
                    return converter.ConvertFromString(valueStr);
                }
                catch
                {
                    if (type == typeof(bool))
                    {
                        bool b;

                        if (bool.TryParse(valueStr, out b))
                        {
                            return b;
                        }
                    }
                    else if (type == typeof(int))
                    {
                        int i;

                        if (int.TryParse(valueStr, out i))
                        {
                            return i;
                        }
                    }
                    else if (type == typeof(uint))
                    {
                        uint ui;

                        if (uint.TryParse(valueStr, out ui))
                        {
                            return ui;
                        }
                    }
                    else if (type == typeof(short))
                    {
                        short s;

                        if (short.TryParse(valueStr, out s))
                        {
                            return s;
                        }
                    }
                    else if (type == typeof(ushort))
                    {
                        ushort us;

                        if (ushort.TryParse(valueStr, out us))
                        {
                            return us;
                        }
                    }
                    else if (type == typeof(char))
                    {
                        char c;

                        if (char.TryParse(valueStr, out c))
                        {
                            return c;
                        }
                    }
                    else if (type == typeof(sbyte))
                    {
                        sbyte sb;

                        if (sbyte.TryParse(valueStr, out sb))
                        {
                            return sb;
                        }
                    }
                    else if (type == typeof(byte))
                    {
                        byte b;

                        if (byte.TryParse(valueStr, out b))
                        {
                            return b;
                        }
                    }
                    else if (type == typeof(long))
                    {
                        long l;

                        if (long.TryParse(valueStr, out l))
                        {
                            return l;
                        }
                    }
                    else if (type == typeof(ulong))
                    {
                        ulong ul;

                        if (ulong.TryParse(valueStr, out ul))
                        {
                            return ul;
                        }
                    }
                    else if (type == typeof(float))
                    {
                        float f;

                        if (float.TryParse(valueStr, out f))
                        {
                            return f;
                        }
                    }
                    else if (type == typeof(double))
                    {
                        double d;

                        if (double.TryParse(valueStr, out d))
                        {
                            return d;
                        }
                    }
                    else if (type == typeof(string))
                    {
                        return valueStr;
                    }
                    else if (type.IsEnum)
                    {
                        object ret = Enum.Parse(type, valueStr, true);

                        return ret;
                    }
                    else
                    {
                        Debug.Check(false);
                    }
                }
            }

            return GetDefaultValue(type);
        }

        public static Type GetPrimitiveTypeFromName(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return null;
            }

            switch (typeName)
            {
                case "bool":
                    return typeof(bool);

                case "int":
                    return typeof(int);

                case "uint":
                    return typeof(uint);

                case "short":
                    return typeof(short);

                case "ushort":
                    return typeof(ushort);

                case "char":
                    return typeof(char);

                case "sbyte":
                    return typeof(sbyte);

                case "ubyte":
                case "byte":
                    return typeof(byte);

                case "long":
                case "llong":
                    return typeof(long);

                case "ulong":
                case "ullong":
                    return typeof(ulong);

                case "float":
                    return typeof(float);

                case "double":
                    return typeof(double);

                case "string":
                    return typeof(string);
            }

            return Utils.GetType(typeName);
        }

        private static Type GetElementTypeFromName(string typeName)
        {
            bool bArrayType = false;

            //array type
            if (typeName.StartsWith("vector<"))
            {
                bArrayType = true;
            }

            if (bArrayType)
            {
                int bracket0 = typeName.IndexOf('<');
                int bracket1 = typeName.IndexOf('>');
                int len = bracket1 - bracket0 - 1;

                string elementTypeName = typeName.Substring(bracket0 + 1, len);
                Type elementType = Utils.GetTypeFromName(elementTypeName);

                return elementType;
            }

            return null;
        }

        public static Type GetTypeFromName(string typeName)
        {
            if (typeName == "void*")
            {
                return typeof(Agent);
            }

            Type type = Agent.GetTypeFromName(typeName);

            if (type == null)
            {
                type = Utils.GetPrimitiveTypeFromName(typeName);

                if (type == null)
                {
                    Type elementType = Utils.GetElementTypeFromName(typeName);

                    if (elementType != null)
                    {
                        Type vectorType = typeof(List<>).MakeGenericType(elementType);
                        return vectorType;
                    }
                    else
                    {
                        typeName = typeName.Replace("::", ".");
                        type = Utils.GetType(typeName);
                    }
                }
            }

            return type;
        }

        //if it is an array, return the element type
        public static Type GetTypeFromName(string typeName, ref bool bIsArrayType)
        {
            Type elementType = Utils.GetElementTypeFromName(typeName);

            if (elementType != null)
            {
                bIsArrayType = true;
                return elementType;
            }

            Type type = Utils.GetTypeFromName(typeName);

            return type;
        }

        public static string GetNativeTypeName(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return string.Empty;
            }

            if (typeName.StartsWith("vector<"))
            {
                return typeName;
            }

            bool bRef = false;
            string key;

            if (typeName.EndsWith("&"))
            {
                //trim the last '&'
                key = typeName.Substring(0, typeName.Length - 1);
                bRef = true;
            }
            else
            {
                key = typeName;
            }

            if (ms_type_mapping.ContainsKey(key))
            {
                if (bRef)
                {
                    return ms_type_mapping[key] + "&";
                }
                else
                {
                    return ms_type_mapping[key];
                }
            }

            //string[] types = typeName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            //return types[types.Length - 1];
            return typeName;
        }

        public static string GetNativeTypeName(Type type)
        {
            Debug.Check(type != null);

            if (Utils.IsArrayType(type))
            {
                Type itemType = type.GetGenericArguments()[0];
                return string.Format("vector<{0}>", Utils.GetNativeTypeName(itemType));
            }

            return Utils.GetNativeTypeName(type.FullName);
        }

        public static bool IsStringType(Type type)
        {
            return type == typeof(string);
        }

        public static bool IsEnumType(Type type)
        {
            return type != null && type.IsEnum;
        }

        public static bool IsArrayType(Type type)
        {
            return type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        }

        public static bool IsCustomClassType(Type type)
        {
            return type != null && !type.IsByRef && (type.IsClass || type.IsValueType) && type != typeof(void) && !type.IsEnum && !type.IsPrimitive && !IsStringType(type) && !IsArrayType(type);
        }

        public static bool IsCustomStructType(Type type)
        {
            return type != null && !type.IsByRef && type.IsValueType && type != typeof(void) && !type.IsEnum && !type.IsPrimitive && !IsStringType(type) && !IsArrayType(type);
        }

        public static bool IsAgentType(Type type)
        {
            Debug.Check(type != null);
            return (type == typeof(Agent) || type.IsSubclassOf(typeof(Agent)));
        }

        public static bool IsGameObjectType(Type type)
        {
            return (type == typeof(UnityEngine.GameObject) || type.IsSubclassOf(typeof(UnityEngine.GameObject)));
        }

        public static bool IsRefNullType(Type type)
        {
            return type != null && type.IsClass && type != typeof(string);
        }

        public static bool IfEquals(object l, object r)
        {
            if (l == r)
            {
                //if both are null, then equals
                return true;
            }

            if (l == null || r == null)
            {
                //if one of them is null, not equal
                return false;
            }

            Type type = l.GetType();

            if (type != r.GetType())
            {
                return false;
            }

            bool bIsArrayType = Utils.IsArrayType(type);

            if (bIsArrayType)
            {
                IList la = (IList)l;
                IList ra = (IList)r;

                if (la.Count != ra.Count)
                {
                    return false;
                }

                for (int i = 0; i < la.Count; ++i)
                {
                    object li = la[i];
                    object ri = ra[i];

                    bool bi = IfEquals(li, ri);

                    if (!bi)
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                bool bIsStruct = Utils.IsCustomClassType(type);

                if (bIsStruct)
                {
                    FieldInfo[] fields = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    for (int i = 0; i < fields.Length; ++i)
                    {
                        FieldInfo f = fields[i];
                        object lf = f.GetValue(l);
                        object rf = f.GetValue(r);
                        bool bi = IfEquals(lf, rf);

                        if (!bi)
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }

            bool bIsEqual = Details.Equals_(l, r);

            return bIsEqual;
        }

        public static void Clone<T>(ref T o, T c)
        {
            if (c == null)
            {
                o = default(T);
                return ;
            }

            Type type = c.GetType();

            if (type.IsPrimitive || type.IsEnum || type.IsValueType)
            {
                //c is boxed, it needs to make a new copy
                //if (o == null)
                //{
                //    o =(T) Activator.CreateInstance(type);
                //}

                o = c;
            }
            else if (type == typeof(string))
            {
                //string is immutable, it doesn't need to make a new copy
                o = c;
            }
            else if (type.IsArray)
            {
                Type elementType = type.GetElementType();

                if (elementType == null)
                {
                    elementType = Type.GetType(type.FullName.Replace("[]", string.Empty));
                }

                var array = c as Array;
                Array copied = Array.CreateInstance(elementType, array.Length);

                for (int i = 0; i < array.Length; i++)
                {
                    object item = null;
                    Utils.Clone(ref item, array.GetValue(i));
                    //object item = Utils.Clone(array.GetValue(i));

                    copied.SetValue(item, i);
                }

                if (o == null)
                {
                    o = (T) Convert.ChangeType(copied, type);
                }
            }
            else if (Utils.IsArrayType(type))
            {
                Type elementType = type.GetElementType();

                if (elementType == null)
                {
                    elementType = Type.GetType(type.FullName.Replace("[]", string.Empty));
                }

                var array = c as IList;
                o = (T)Activator.CreateInstance(type);

                for (int i = 0; i < array.Count; i++)
                {
                    object item = null;
                    Utils.Clone(ref item, array[i]);

                    ((IList)o).Add(item);
                }
            }
            else
            {
                bool isClass = type.IsClass;

                if (isClass && Utils.IsRefNullType(type))
                {
                    o = c;
                }
                else
                {
                    Debug.Check(!type.IsPrimitive && !type.IsEnum);

                    bool isStruct = type.IsValueType;

                    if (o == null)
                    {
                        o = (T)Activator.CreateInstance(type);
                    }

                    if (isStruct || isClass)
                    {
                        FieldInfo[] fields = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

                        for (int i = 0; i < fields.Length; ++i)
                        {
                            FieldInfo f = fields[i];

                            if (!f.IsLiteral)
                            {
                                object fv = f.GetValue(c);
                                object fv2 = null;
                                Utils.Clone(ref fv2 , fv);

                                f.SetValue(o, fv2);
                            }
                        }
                    }
                    else
                    {
                        o = c;
                    }
                }
            }
        }
    }

    static public class Debug
    {
        [Conditional("BEHAVIAC_DEBUG")]
        [Conditional("UNITY_EDITOR")]
        public static void CheckEqual<T>(T l, T r)
        {
            if (!EqualityComparer<T>.Default.Equals(l, r))
            {
                Break("CheckEqualFailed");
            }
        }

        [Conditional("BEHAVIAC_DEBUG")]
        [Conditional("UNITY_EDITOR")]
        public static void Check(bool b)
        {
            if (!b)
            {
                Break("CheckFailed");
            }
        }

        [Conditional("BEHAVIAC_DEBUG")]
        [Conditional("UNITY_EDITOR")]
        public static void Check(bool b, string message)
        {
            if (!b)
            {
                Break(message);
            }
        }

        [Conditional("BEHAVIAC_DEBUG")]
        [Conditional("UNITY_EDITOR")]
        public static void Check(bool b, string format, object arg0)
        {
            if (!b)
            {
                string message = string.Format(format, arg0);
                Break(message);
            }
        }

        [Conditional("BEHAVIAC_DEBUG")]
        [Conditional("UNITY_EDITOR")]
        public static void Log(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogWarning(string message)
        {
            UnityEngine.Debug.LogWarning(message);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogError(string message)
        {
            UnityEngine.Debug.LogError(message);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogError(Exception ex)
        {
            UnityEngine.Debug.LogError(ex.Message);
        }

        [Conditional("BEHAVIAC_DEBUG")]
        [Conditional("UNITY_EDITOR")]
        public static void Break(string msg)
        {
            LogError(msg);

            UnityEngine.Debug.Break();
            //throw new Exception();
            //System.Diagnostics.Debug.Assert(false);
        }
    }

    #region Detail

    static public class Details
    {
        #region Impl

        private class CompareValue<T> where T : struct, IComparable<T>
        {
            public static bool Greater(object lhs, object rhs)
            {
                return ((T)lhs).CompareTo((T)rhs) > 0;
            }

            public static bool GreaterEqual(object lhs, object rhs)
            {
                return ((T)lhs).CompareTo((T)rhs) >= 0;
            }

            public static bool Less(object lhs, object rhs)
            {
                return ((T)lhs).CompareTo((T)rhs) < 0;
            }

            public static bool LessEqual(object lhs, object rhs)
            {
                return ((T)lhs).CompareTo((T)rhs) <= 0;
            }
        }

        private interface ICompareValue
        {
            bool Greater(object lhs, object rhs);

            bool GreaterEqual(object lhs, object rhs);

            bool Less(object lhs, object rhs);

            bool LessEqual(object lhs, object rhs);
        }

        private class CompareValueInt : ICompareValue
        {
            public bool Greater(object lhs, object rhs)
            {
                return ((int)lhs) > ((int)rhs);
            }

            public bool GreaterEqual(object lhs, object rhs)
            {
                return ((int)lhs) >= ((int)rhs);
            }

            public bool Less(object lhs, object rhs)
            {
                return ((int)lhs) < ((int)rhs);
            }

            public bool LessEqual(object lhs, object rhs)
            {
                return ((int)lhs) <= ((int)rhs);
            }
        }

        private class CompareValueLong : ICompareValue
        {
            public bool Greater(object lhs, object rhs)
            {
                return ((long)lhs) > ((long)rhs);
            }

            public bool GreaterEqual(object lhs, object rhs)
            {
                return ((long)lhs) >= ((long)rhs);
            }

            public bool Less(object lhs, object rhs)
            {
                return ((long)lhs) < ((long)rhs);
            }

            public bool LessEqual(object lhs, object rhs)
            {
                return ((long)lhs) <= ((long)rhs);
            }
        }

        private class CompareValueShort : ICompareValue
        {
            public bool Greater(object lhs, object rhs)
            {
                return ((short)lhs) > ((short)rhs);
            }

            public bool GreaterEqual(object lhs, object rhs)
            {
                return ((short)lhs) >= ((short)rhs);
            }

            public bool Less(object lhs, object rhs)
            {
                return ((short)lhs) < ((short)rhs);
            }

            public bool LessEqual(object lhs, object rhs)
            {
                return ((short)lhs) <= ((short)rhs);
            }
        }

        private class CompareValueByte : ICompareValue
        {
            public bool Greater(object lhs, object rhs)
            {
                return ((sbyte)lhs) > ((sbyte)rhs);
            }

            public bool GreaterEqual(object lhs, object rhs)
            {
                return ((sbyte)lhs) >= ((sbyte)rhs);
            }

            public bool Less(object lhs, object rhs)
            {
                return ((sbyte)lhs) < ((sbyte)rhs);
            }

            public bool LessEqual(object lhs, object rhs)
            {
                return ((sbyte)lhs) <= ((sbyte)rhs);
            }
        }

        private class CompareValueFloat : ICompareValue
        {
            public bool Greater(object lhs, object rhs)
            {
                return ((float)lhs) > ((float)rhs);
            }

            public bool GreaterEqual(object lhs, object rhs)
            {
                return ((float)lhs) >= ((float)rhs);
            }

            public bool Less(object lhs, object rhs)
            {
                return ((float)lhs) < ((float)rhs);
            }

            public bool LessEqual(object lhs, object rhs)
            {
                return ((float)lhs) <= ((float)rhs);
            }
        }

        private class CompareValueUInt : ICompareValue
        {
            public bool Greater(object lhs, object rhs)
            {
                return ((uint)lhs) > ((uint)rhs);
            }

            public bool GreaterEqual(object lhs, object rhs)
            {
                return ((uint)lhs) >= ((uint)rhs);
            }

            public bool Less(object lhs, object rhs)
            {
                return ((uint)lhs) < ((uint)rhs);
            }

            public bool LessEqual(object lhs, object rhs)
            {
                return ((uint)lhs) <= ((uint)rhs);
            }
        }

        private class CompareValueULong : ICompareValue
        {
            public bool Greater(object lhs, object rhs)
            {
                return ((ulong)lhs) > ((ulong)rhs);
            }

            public bool GreaterEqual(object lhs, object rhs)
            {
                return ((ulong)lhs) >= ((ulong)rhs);
            }

            public bool Less(object lhs, object rhs)
            {
                return ((ulong)lhs) < ((ulong)rhs);
            }

            public bool LessEqual(object lhs, object rhs)
            {
                return ((ulong)lhs) <= ((ulong)rhs);
            }
        }

        private class CompareValueUShort : ICompareValue
        {
            public bool Greater(object lhs, object rhs)
            {
                return ((ushort)lhs) > ((ushort)rhs);
            }

            public bool GreaterEqual(object lhs, object rhs)
            {
                return ((ushort)lhs) >= ((ushort)rhs);
            }

            public bool Less(object lhs, object rhs)
            {
                return ((ushort)lhs) < ((ushort)rhs);
            }

            public bool LessEqual(object lhs, object rhs)
            {
                return ((ushort)lhs) <= ((ushort)rhs);
            }
        }

        private class CompareValueUByte : ICompareValue
        {
            public bool Greater(object lhs, object rhs)
            {
                return ((byte)lhs) > ((byte)rhs);
            }

            public bool GreaterEqual(object lhs, object rhs)
            {
                return ((byte)lhs) >= ((byte)rhs);
            }

            public bool Less(object lhs, object rhs)
            {
                return ((byte)lhs) < ((byte)rhs);
            }

            public bool LessEqual(object lhs, object rhs)
            {
                return ((byte)lhs) <= ((byte)rhs);
            }
        }

        private class CompareValueDouble : ICompareValue
        {
            public bool Greater(object lhs, object rhs)
            {
                return ((double)lhs) > ((double)rhs);
            }

            public bool GreaterEqual(object lhs, object rhs)
            {
                return ((double)lhs) >= ((double)rhs);
            }

            public bool Less(object lhs, object rhs)
            {
                return ((double)lhs) < ((double)rhs);
            }

            public bool LessEqual(object lhs, object rhs)
            {
                return ((double)lhs) <= ((double)rhs);
            }
        }

        ///
        private interface IComputeValue
        {
            object Add(object opr1, object opr2);

            object Sub(object opr1, object opr2);

            object Mul(object opr1, object opr2);

            object Div(object opr1, object opr2);
        }

        private class ComputeValueInt : IComputeValue
        {
            public object Add(object lhs, object rhs)
            {
                int r = ((int)lhs) + ((int)rhs);
                return r;
            }

            public object Sub(object lhs, object rhs)
            {
                int r = ((int)lhs) - ((int)rhs);
                return r;
            }

            public object Mul(object lhs, object rhs)
            {
                int r = ((int)lhs) * ((int)rhs);
                return r;
            }

            public object Div(object lhs, object rhs)
            {
                int r = ((int)lhs) / ((int)rhs);
                return r;
            }
        }

        private class ComputeValueLong : IComputeValue
        {
            public object Add(object lhs, object rhs)
            {
                long r = ((long)lhs) + ((long)rhs);
                return r;
            }

            public object Sub(object lhs, object rhs)
            {
                long r = ((long)lhs) - ((long)rhs);
                return r;
            }

            public object Mul(object lhs, object rhs)
            {
                long r = ((long)lhs) * ((long)rhs);
                return r;
            }

            public object Div(object lhs, object rhs)
            {
                long r = ((long)lhs) / ((long)rhs);
                return r;
            }
        }

        private class ComputeValueShort : IComputeValue
        {
            public object Add(object lhs, object rhs)
            {
                short r = (short)(((short)lhs) + ((short)rhs));
                return r;
            }

            public object Sub(object lhs, object rhs)
            {
                short r = (short)(((short)lhs) - ((short)rhs));
                return r;
            }

            public object Mul(object lhs, object rhs)
            {
                short r = (short)(((short)lhs) * ((short)rhs));
                return r;
            }

            public object Div(object lhs, object rhs)
            {
                short r = (short)(((short)lhs) / ((short)rhs));
                return r;
            }
        }

        private class ComputeValueByte : IComputeValue
        {
            public object Add(object lhs, object rhs)
            {
                sbyte r = (sbyte)(((sbyte)lhs) + ((sbyte)rhs));
                return r;
            }

            public object Sub(object lhs, object rhs)
            {
                sbyte r = (sbyte)(((sbyte)lhs) - ((sbyte)rhs));
                return r;
            }

            public object Mul(object lhs, object rhs)
            {
                sbyte r = (sbyte)(((sbyte)lhs) * ((sbyte)rhs));
                return r;
            }

            public object Div(object lhs, object rhs)
            {
                sbyte r = (sbyte)(((sbyte)lhs) / ((sbyte)rhs));
                return r;
            }
        }

        private class ComputeValueFloat : IComputeValue
        {
            public object Add(object lhs, object rhs)
            {
                float r = ((float)lhs) + ((float)rhs);
                return r;
            }

            public object Sub(object lhs, object rhs)
            {
                float r = ((float)lhs) - ((float)rhs);
                return r;
            }

            public object Mul(object lhs, object rhs)
            {
                float r = ((float)lhs) * ((float)rhs);
                return r;
            }

            public object Div(object lhs, object rhs)
            {
                float r = ((float)lhs) / ((float)rhs);
                return r;
            }
        }

        private class ComputeValueUInt : IComputeValue
        {
            public object Add(object lhs, object rhs)
            {
                uint r = ((uint)lhs) + ((uint)rhs);
                return r;
            }

            public object Sub(object lhs, object rhs)
            {
                uint r = ((uint)lhs) - ((uint)rhs);
                return r;
            }

            public object Mul(object lhs, object rhs)
            {
                uint r = ((uint)lhs) * ((uint)rhs);
                return r;
            }

            public object Div(object lhs, object rhs)
            {
                uint r = ((uint)lhs) / ((uint)rhs);
                return r;
            }
        }

        private class ComputeValueULong : IComputeValue
        {
            public object Add(object lhs, object rhs)
            {
                ulong r = ((ulong)lhs) + ((ulong)rhs);
                return r;
            }

            public object Sub(object lhs, object rhs)
            {
                ulong r = ((ulong)lhs) - ((ulong)rhs);
                return r;
            }

            public object Mul(object lhs, object rhs)
            {
                ulong r = ((ulong)lhs) * ((ulong)rhs);
                return r;
            }

            public object Div(object lhs, object rhs)
            {
                ulong r = ((ulong)lhs) / ((ulong)rhs);
                return r;
            }
        }

        private class ComputeValueUShort : IComputeValue
        {
            public object Add(object lhs, object rhs)
            {
                ushort r = (ushort)(((ushort)lhs) + ((ushort)rhs));
                return r;
            }

            public object Sub(object lhs, object rhs)
            {
                ushort r = (ushort)(((ushort)lhs) - ((ushort)rhs));
                return r;
            }

            public object Mul(object lhs, object rhs)
            {
                ushort r = (ushort)(((ushort)lhs) * ((ushort)rhs));
                return r;
            }

            public object Div(object lhs, object rhs)
            {
                ushort r = (ushort)(((ushort)lhs) / ((ushort)rhs));
                return r;
            }
        }

        private class ComputeValueUByte : IComputeValue
        {
            public object Add(object lhs, object rhs)
            {
                byte r = (byte)(((byte)lhs) + ((byte)rhs));
                return r;
            }

            public object Sub(object lhs, object rhs)
            {
                byte r = (byte)(((byte)lhs) - ((byte)rhs));
                return r;
            }

            public object Mul(object lhs, object rhs)
            {
                byte r = (byte)(((byte)lhs) * ((byte)rhs));
                return r;
            }

            public object Div(object lhs, object rhs)
            {
                byte r = (byte)(((byte)lhs) / ((byte)rhs));
                return r;
            }
        }

        private class ComputeValueDouble : IComputeValue
        {
            public object Add(object lhs, object rhs)
            {
                double r = ((double)lhs) + ((double)rhs);
                return r;
            }

            public object Sub(object lhs, object rhs)
            {
                double r = ((double)lhs) - ((double)rhs);
                return r;
            }

            public object Mul(object lhs, object rhs)
            {
                double r = ((double)lhs) * ((double)rhs);
                return r;
            }

            public object Div(object lhs, object rhs)
            {
                double r = ((double)lhs) / ((double)rhs);
                return r;
            }
        }

        #endregion Impl

        private static Dictionary<Type, ICompareValue> ms_comparers = null;

        public static void RegisterCompareValue()
        {
            Debug.Check(ms_comparers == null);

            ms_comparers = new Dictionary<Type, ICompareValue>();

            ms_comparers[typeof(int)] = new CompareValueInt();
            ms_comparers[typeof(long)] = new CompareValueLong();
            ms_comparers[typeof(short)] = new CompareValueShort();
            ms_comparers[typeof(sbyte)] = new CompareValueByte();
            ms_comparers[typeof(float)] = new CompareValueFloat();

            ms_comparers[typeof(uint)] = new CompareValueUInt();
            ms_comparers[typeof(ulong)] = new CompareValueULong();
            ms_comparers[typeof(ushort)] = new CompareValueUShort();
            ms_comparers[typeof(byte)] = new CompareValueUByte();
            ms_comparers[typeof(double)] = new CompareValueDouble();
        }

        public static bool Equals_(object lhs, object rhs)
        {
            return System.Object.Equals(lhs, rhs);
            //return ((lhs == rhs) ||
            //    (((lhs != null) && (rhs != null)) && lhs.Equals(rhs)));
        }

        public static bool Greater(object lhs, object rhs)
        {
            Debug.Check(lhs != null && rhs != null);
            Type lhs_t = lhs.GetType();
            Debug.Check(lhs_t == rhs.GetType());
            Debug.Check(ms_comparers != null);

            if (ms_comparers.ContainsKey(lhs_t))
            {
                ICompareValue d = ms_comparers[lhs_t];
                return d.Greater(lhs, rhs);
            }
            else
            {
                Debug.Check(false);
            }

            return false;
        }

        public static bool GreaterEqual(object lhs, object rhs)
        {
            Debug.Check(lhs != null && rhs != null);
            Type lhs_t = lhs.GetType();
            Debug.Check(lhs_t == rhs.GetType());

            if (ms_comparers.ContainsKey(lhs_t))
            {
                ICompareValue d = ms_comparers[lhs_t];
                return d.GreaterEqual(lhs, rhs);
            }
            else
            {
                Debug.Check(false);
            }

            return false;
        }

        public static bool Less(object lhs, object rhs)
        {
            Debug.Check(lhs != null && rhs != null);
            Type lhs_t = lhs.GetType();
            Debug.Check(lhs_t == rhs.GetType());

            if (ms_comparers.ContainsKey(lhs_t))
            {
                ICompareValue d = ms_comparers[lhs_t];
                return d.Less(lhs, rhs);
            }
            else
            {
                Debug.Check(false);
            }

            return false;
        }

        public static bool LessEqual(object lhs, object rhs)
        {
            Debug.Check(lhs != null && rhs != null);
            Type lhs_t = lhs.GetType();
            Debug.Check(lhs_t == rhs.GetType());

            if (ms_comparers.ContainsKey(lhs_t))
            {
                ICompareValue d = ms_comparers[lhs_t];
                return d.LessEqual(lhs, rhs);
            }
            else
            {
                Debug.Check(false);
            }

            return false;
        }

        private static Dictionary<Type, IComputeValue> ms_computers = null;

        public static void RegisterComputeValue()
        {
            ms_computers = new Dictionary<Type, IComputeValue>();

            ms_computers[typeof(int)] = new ComputeValueInt();
            ms_computers[typeof(long)] = new ComputeValueLong();
            ms_computers[typeof(short)] = new ComputeValueShort();
            ms_computers[typeof(sbyte)] = new ComputeValueByte();
            ms_computers[typeof(float)] = new ComputeValueFloat();

            ms_computers[typeof(uint)] = new ComputeValueUInt();
            ms_computers[typeof(ulong)] = new ComputeValueULong();
            ms_computers[typeof(ushort)] = new ComputeValueUShort();
            ms_computers[typeof(byte)] = new ComputeValueUByte();
            ms_computers[typeof(double)] = new ComputeValueDouble();
        }

        public static object ComputeValue(object value1, object value2, EComputeOperator opr)
        {
            Debug.Check(value1 != null && value2 != null);
            Type lhs_t = value1.GetType();
            Debug.Check(lhs_t == value2.GetType());
            Debug.Check(ms_computers != null);

            if (ms_computers.ContainsKey(lhs_t))
            {
                IComputeValue d = ms_computers[lhs_t];

                switch (opr)
                {
                    case EComputeOperator.E_ADD:
                        return d.Add(value1, value2);

                    case EComputeOperator.E_SUB:
                        return d.Sub(value1, value2);

                    case EComputeOperator.E_MUL:
                        return d.Mul(value1, value2);

                    case EComputeOperator.E_DIV:
                        return d.Div(value1, value2);

                    default:
                        Debug.Check(false);
                        break;
                }
            }

            return null;
        }

        public static void Cleanup()
        {
            if (ms_comparers != null)
            {
                ms_comparers.Clear();
                ms_comparers = null;
            }

            if (ms_computers != null)
            {
                ms_computers.Clear();
                ms_computers = null;
            }
        }
    }

    #endregion Detail

    //public static class ListExtra
    //{
    //    public static void Resize<T>(this List<T> list, int sz, T c = default(T))
    //    {
    //        int cur = list.Count;
    //        if (sz < cur)
    //        {
    //            list.RemoveRange(sz, cur - sz);
    //        }
    //        else if (sz > cur)
    //        {
    //            for (int i = 0; i < sz - cur; ++i)
    //            {
    //                list.Add(c);
    //            }
    //        }
    //    }
    //}

    static public class StringUtils
    {
        ///the first char is '{', to return the paired '}'
        private static int SkipPairedBrackets(string src, int indexBracketBegin)
        {
            if (!string.IsNullOrEmpty(src) && src[indexBracketBegin] == '{')
            {
                int depth = 0;
                int pos = indexBracketBegin;

                while (pos < src.Length)
                {
                    if (src[pos] == '{')
                    {
                        depth++;
                    }
                    else if (src[pos] == '}')
                    {
                        if (--depth == 0)
                        {
                            return pos;
                        }
                    }

                    pos++;
                }
            }

            return -1;
        }

        private static object FromStringStruct(Type type, string src)
        {
            object objValue = Activator.CreateInstance(type);
            Dictionary<string, FieldInfo> structMembers = new Dictionary<string, FieldInfo>();
            FieldInfo[] fields = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

            for (int i = 0; i < fields.Length; ++i)
            {
                FieldInfo f = fields[i];

                if (!f.IsLiteral)
                {
                    structMembers.Add(f.Name, f);
                }
            }

            if (string.IsNullOrEmpty(src))
            {
                return objValue;
            }

            //{color=0;id=;type={bLive=false;name=0;weight=0;};}
            //the first char is '{'
            //the last char is '}'
            int posCloseBrackets = SkipPairedBrackets(src, 0);
            Debug.Check(posCloseBrackets != -1);

            //{color=0;id=;type={bLive=false;name=0;weight=0;};}
            int posBegin = 1;
            int posEnd = src.IndexOf(';', posBegin);

            while (posEnd != -1)
            {
                Debug.Check(src[posEnd] == ';');

                //the last one might be empty
                if (posEnd > posBegin)
                {
                    int posEqual = src.IndexOf('=', posBegin);
                    Debug.Check(posEqual > posBegin);

                    int length = posEqual - posBegin;
                    string memmberName = src.Substring(posBegin, length);
                    string memberValueStr;
                    char c = src[posEqual + 1];

                    if (c != '{')
                    {
                        length = posEnd - posEqual - 1;
                        memberValueStr = src.Substring(posEqual + 1, length);
                    }
                    else
                    {
                        int pStructBegin = 0;
                        pStructBegin += posEqual + 1;
                        int posCloseBrackets_ = SkipPairedBrackets(src, pStructBegin);
                        length = posCloseBrackets_ - pStructBegin + 1;

                        memberValueStr = src.Substring(posEqual + 1, length);

                        posEnd = posEqual + 1 + length;
                    }

                    if (structMembers.ContainsKey(memmberName))
                    {
                        FieldInfo memberType = structMembers[memmberName];
                        Debug.Check(memberType != null);
                        object memberValue = FromString(memberType.FieldType, memberValueStr, false);
                        memberType.SetValue(objValue, memberValue);
                    }
                }

                //skip ';'
                posBegin = posEnd + 1;

                //{color=0;id=;type={bLive=false;name=0;weight=0;};}
                posEnd = src.IndexOf(';', posBegin);

                if (posEnd > posCloseBrackets)
                {
                    break;
                }
            }

            return objValue;
        }

        private static object FromStringVector(Type type, string src)
        {
            Type vectorType = typeof(List<>).MakeGenericType(type);
            IList objVector = (IList)Activator.CreateInstance(vectorType);

            if (string.IsNullOrEmpty(src))
            {
                return objVector;
            }

            int semiColon = src.IndexOf(':');
            Debug.Check(semiColon != -1);
            string countStr = src.Substring(0, semiColon);
            int count = int.Parse(countStr);

            int b = semiColon + 1;
            int sep = b;

            if (b < src.Length && src[b] == '{')
            {
                sep = SkipPairedBrackets(src, b);
                Debug.Check(sep != -1);
            }

            sep = src.IndexOf('|', sep);

            while (sep != -1)
            {
                int len = sep - b;
                string elemStr = src.Substring(b, len);
                object elemObject = FromString(type, elemStr, false);

                objVector.Add(elemObject);

                b = sep + 1;

                if (b < src.Length && src[b] == '{')
                {
                    sep = SkipPairedBrackets(src, b);
                    Debug.Check(b != -1);
                }
                else
                {
                    sep = b;
                }

                sep = src.IndexOf('|', sep);
            }

            if (b < src.Length)
            {
                int len = src.Length - b;
                string elemStr = src.Substring(b, len);
                object elemObject = FromString(type, elemStr, false);

                objVector.Add(elemObject);
            }

            Debug.Check(objVector.Count == count);

            return objVector;
        }

        public static bool IsValidString(string str)
        {
            if (string.IsNullOrEmpty(str) || (str[0] == '\"' && str[1] == '\"'))
            {
                return false;
            }

            return true;
        }

        public static object FromString(Type type, string valStr, bool bStrIsArrayType /*= false*/)
        {
            if (!string.IsNullOrEmpty(valStr) && valStr == "null")
            {
                Debug.Check(Utils.IsRefNullType(type));
                return null;
            }

            if (type.IsByRef)
            {
                type = type.GetElementType();
            }

            bool bIsArrayType = Utils.IsArrayType(type);

            object v = null;

            //customized type, struct or enum
            if (bStrIsArrayType || bIsArrayType)
            {
                if (bIsArrayType)
                {
                    Type elemType = type.GetGenericArguments()[0];
                    v = StringUtils.FromStringVector(elemType, valStr);
                }
                else
                {
                    v = StringUtils.FromStringVector(type, valStr);
                }
            }
            else if (type == typeof(behaviac.Property))
            {
                v = Condition.LoadProperty(valStr);
            }
            else if (Utils.IsCustomClassType(type))
            {
                v = StringUtils.FromStringStruct(type, valStr);
            }
            else
            {
                v = Utils.FromStringPrimitive(type, valStr);
            }

            return v;
        }

        public static string ToString(object value)
        {
            string valueStr = "";

            if (value != null)
            {
                Type type = value.GetType();
                bool bIsArrayType = Utils.IsArrayType(type);

                //customized type, struct or enum
                if (bIsArrayType)
                {
                    IList list = value as IList;
                    valueStr = string.Format("{0}:", list.Count);

                    if (list.Count > 0)
                    {
                        for (int i = 0; i < list.Count - 1; ++i)
                        {
                            object e = list[i];
                            string eStr = StringUtils.ToString(e);
                            valueStr += string.Format("{0}|", eStr);
                        }

                        object eLast = list[list.Count - 1];
                        string eStrLast = StringUtils.ToString(eLast);
                        valueStr += string.Format("{0}", eStrLast);
                    }
                }
                else if (Utils.IsCustomClassType(type))
                {
                    bool bIsNullValueType = Utils.IsRefNullType(type);

                    if (bIsNullValueType)
                    {
                        valueStr = string.Format("{0:x08}", value.GetHashCode());
                    }
                    else
                    {
                        valueStr = "{";
                        FieldInfo[] fields = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

                        for (int i = 0; i < fields.Length; ++i)
                        {
                            FieldInfo f = fields[i];
                            object m = f.GetValue(value);
                            string mStr = StringUtils.ToString(m);
                            valueStr += string.Format("{0}={1};", f.Name, mStr);
                        }

                        valueStr += "}";
                    }
                }
                else
                {
                    valueStr = value.ToString();
                }
            }
            else
            {
                valueStr = "null";
            }

            return valueStr;
        }

        public static string FindExtension(string path)
        {
            return Path.GetExtension(path);
        }

        //public static bool IsNullOrEmpty(this string s)
        //{
        //    return string.IsNullOrEmpty(s);
        //}

        //
        public static int FirstToken(string params_, char sep, ref string token)
        {
            //const int 5
            int end = params_.IndexOf(sep);

            if (end != -1)
            {
                token = params_.Substring(0, end);
                return end;
            }

            return -1;
        }

        public static List<string> SplitTokens(string str)
        {
            List<string> ret = new List<string>();
            //"int Self.AgentArrayAccessTest::ListInts[int Self.AgentArrayAccessTest::l_index]"
            int pB = 0;
            int i = 0;

            bool bBeginIndex = false;

            while (i < str.Length)
            {
                bool bFound = false;
                char c = str[i];

                if (c == ' ' && !bBeginIndex)
                {
                    bFound = true;
                }
                else if (c == '[')
                {
                    bBeginIndex = true;
                    bFound = true;
                }
                else if (c == ']')
                {
                    bBeginIndex = false;
                    bFound = true;
                }

                if (bFound)
                {
                    string strT = ReadToken(str, pB, i);
                    Debug.Check(strT.Length > 0);
                    ret.Add(strT);

                    pB = i + 1;
                }

                i++;
            }

            string t = ReadToken(str, pB, i);

            if (t.Length > 0)
            {
                ret.Add(t);
            }

            return ret;
        }

        private static string ReadToken(string str, int pB, int end)
        {
            string strT = "";
            int p = pB;

            while (p < end)
            {
                strT += str[p++];
            }

            return strT;
        }

        public static bool ParseForStruct(Type type, string str, ref string strT, Dictionary<string, Property> props)
        {
            int pB = 0;
            int i = 0;

            while (i < str.Length)
            {
                char c = str[i];

                if (c == ';' || c == '{' || c == '}')
                {
                    int p = pB;

                    while (p <= i)
                    {
                        strT += str[p++];
                    }

                    pB = i + 1;
                }
                else if (c == ' ')
                {
                    //par or property
                    string propName = "";
                    int p = pB;

                    while (str[p] != '=')
                    {
                        propName += str[p++];
                    }

                    //skip '='
                    Debug.Check(str[p] == '=');
                    p++;

                    string typeStr = "";

                    while (str[p] != ' ')
                    {
                        typeStr += str[p++];
                    }

                    bool bStatic = false;

                    if (typeStr == "static")
                    {
                        //skip ' '
                        Debug.Check(str[p] == ' ');
                        p++;

                        while (str[p] != ' ')
                        {
                            typeStr += str[p++];
                        }

                        bStatic = true;
                    }

                    string parName = "";

                    //skip ' '
                    Debug.Check(str[i] == ' ');
                    i++;

                    while (str[i] != ';')
                    {
                        parName += str[i++];
                    }

                    props[propName] = behaviac.Property.Create(typeStr, parName, bStatic, null);

                    //skip ';'
                    Debug.Check(str[i] == ';');

                    pB = i + 1;
                }

                i++;
            }

            return true;
        }
    }
}
