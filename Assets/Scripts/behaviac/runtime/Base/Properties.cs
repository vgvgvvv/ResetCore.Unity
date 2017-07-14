/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Tencent is pleased to support the open source community by making behaviac available.
//
// Copyright (C) 2015 THL A29 Limited, a Tencent company. All rights reserved.
//
// Licensed under the BSD 3-Clause License (the "License"); you may not use this file except in compliance with
// the License. You may obtain a copy of the License at http://opensource.org/licenses/BSD-3-Clause
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is
// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace behaviac
{
    public class Property
    {
        public Property(CMemberBase pMemberBase, bool bIsConst)
        {
            m_memberBase = pMemberBase;
            m_variableId = 0;
            m_bValidDefaultValue = false;
            m_bIsConst = bIsConst;
        }

        protected Property(Property copy)
        {
            m_variableName = copy.m_variableName;
            m_instanceName = copy.m_instanceName;
            m_variableId = copy.m_variableId;
            m_memberBase = copy.m_memberBase;

            m_parent = copy.m_parent;
            m_index = copy.m_index;

            m_bValidDefaultValue = copy.m_bValidDefaultValue;
            m_defaultValue = copy.m_defaultValue;
            m_bIsConst = copy.m_bIsConst;
            m_bIsStatic = copy.m_bIsStatic;
            m_bIsLocal = copy.m_bIsLocal;
            m_strNativeTypeName = copy.m_strNativeTypeName;
        }

        protected Property(Property parent, string indexStr)
        {
            m_variableName = parent.m_variableName + "[]";
            m_instanceName = parent.m_instanceName;
            m_variableId = Utils.MakeVariableId(m_variableName);
            //m_refParName = parent.m_refParName;
            //m_refParNameId = parent.m_refParNameId;
            m_memberBase = null;
            m_parent = parent;

            m_bValidDefaultValue = parent.m_bValidDefaultValue;

            if (m_bValidDefaultValue)
            {
                IList asList = parent.m_defaultValue as IList;

                if (asList != null && asList.Count > 0)
                {
                    m_defaultValue = asList[0];
                }
            }

            m_bIsConst = false;
            m_bIsStatic = false;
            m_bIsLocal = parent.m_bIsLocal;
            //m_strNativeTypeName = parent.m_strNativeTypeName;
            //m_strNativeTypeName = "int";

            string[] tokens = indexStr.Split(' ');

            if (tokens.Length == 1)
            {
                this.m_index = Property.Create("int", indexStr);
            }
            else
            {
                Debug.Check(tokens[0] == "int");
                this.m_index = Property.Create(tokens[0], tokens[1], false, null);
            }
        }

        ~Property()
        {
        }

        public string Name
        {
            get
            {
                return this.m_variableName;
            }
            set
            {
                this.m_variableName = value;

                this.m_variableId = Utils.MakeVariableId(this.m_variableName);
            }
        }

        public string InstanceName
        {
            get
            {
                if (!string.IsNullOrEmpty(this.m_instanceName))
                {
                    return this.m_instanceName;
                }

                return m_memberBase != null ? m_memberBase.InstanceName : null;
            }
            set
            {
                this.m_instanceName = value;
            }
        }
        /// <summary>
        /// get variableId
        /// </summary>
        /// <returns></returns>
        public uint GetVariableId()
        {
            return m_variableId;
        }
        public uint VariableId
        {
            get
            {
                return m_variableId;
            }
        }

        private bool m_bIsStatic;

        public bool IsStatic
        {
            get
            {
                return this.m_bIsStatic;
            }
            set
            {
                this.m_bIsStatic = value;
            }
        }

        private bool m_bIsLocal;

        public bool IsLocal
        {
            get
            {
                return this.m_bIsLocal;
            }
            set
            {
                this.m_bIsLocal = value;
            }
        }

        public Agent GetParentAgent(Agent pAgent)
        {
            return Utils.GetParentAgent(pAgent, this.m_instanceName);
        }

        public Type PropertyType
        {
            get
            {
                if (this.m_defaultValue != null)
                {
                    return this.m_defaultValue.GetType();
                }

                if (this.m_memberBase != null)
                {
                    return this.m_memberBase.MemberType;
                }

                return null;
            }
        }
        private string m_strNativeTypeName;
        public string NativeTypeName
        {
            get
            {
                return m_strNativeTypeName;

                //string strName = null;
                //if (PropertyType != null)
                //{
                //    strName = Utils.GetNativeTypeName(PropertyType);
                //    if (!string.IsNullOrEmpty(strName))
                //    {
                //        return strName;
                //    }
                //}
                //if (!string.IsNullOrEmpty(m_strNativeTypeName))
                //{
                //    return Utils.GetNativeTypeName(m_strNativeTypeName);
                //}
                //return null;
            }
            set
            {
                m_strNativeTypeName = Utils.GetNativeTypeName(value).Replace("::", ".");
            }
        }

        public void SetFrom(Agent pAgentFrom, Property from, Agent pAgentTo)
        {
            object retV = from.GetValue(pAgentFrom);

            this.SetValue(pAgentTo, retV);
        }

        public void SetDefaultInteger(int count)
        {
            this.m_bValidDefaultValue = true;

            Utils.ConvertFromInteger(count, ref this.m_defaultValue);
        }

        public uint GetDefaultInteger()
        {
            //VariableType retV = this.GetDefaultValue();

            //uint result = Utils.ConvertToInteger(retV);
            //return result;
            return 0;
        }

        public float GetRange()
        {
            if (this.m_memberBase != null)
            {
                return this.m_memberBase.GetRange();
            }

            return 1.0f;
        }

        public float DifferencePercentage(Property other, float range)
        {
            //return const_cast<CMemberBase*>(this.m_memberBase).DifferencePercentage(this, other);
            object lV = this.GetDefaultValue();
            object rV = other.GetDefaultValue();

            float leftValue = 0.0f;
            float rightValue = 0.0f;

            if (lV.GetType() == typeof(float))
            {
                leftValue = (float)lV;
                rightValue = (float)rV;
            }
            else if (lV.GetType() == typeof(long))
            {
                leftValue = (long)lV;
                rightValue = (long)rV;
            }
            else if (lV.GetType() == typeof(int))
            {
                leftValue = (int)lV;
                rightValue = (int)rV;
            }
            else if (lV.GetType() == typeof(short))
            {
                leftValue = (short)lV;
                rightValue = (short)rV;
            }
            else if (lV.GetType() == typeof(sbyte))
            {
                leftValue = (sbyte)lV;
                rightValue = (sbyte)rV;
            }
            else if (lV.GetType() == typeof(ulong))
            {
                leftValue = (ulong)lV;
                rightValue = (ulong)rV;
            }
            else if (lV.GetType() == typeof(uint))
            {
                leftValue = (uint)lV;
                rightValue = (uint)rV;
            }
            else if (lV.GetType() == typeof(ushort))
            {
                leftValue = (ushort)lV;
                rightValue = (ushort)rV;
            }
            else if (lV.GetType() == typeof(byte))
            {
                leftValue = (byte)lV;
                rightValue = (byte)rV;
            }

            float d = leftValue - rightValue;

            if (d < 0.0f)
            {
                d = -d;
            }

            return d / range;
        }

        public void Instantiate(Agent pAgent)
        {
            //if it is a member property, don't instantiate it, otherwise it will have two copies can causes inconsistency
            if (this.m_memberBase == null)
            {
                object v = this.GetDefaultValue();

                if (this.IsStatic)
                {
                    this.SetValue(pAgent, v);
                }
                else if (v != null)
                {
                    pAgent.Instantiate(v, this);
                }
                else 
                {
                    Debug.Check(true);
                }
            }
        }

        public void UnInstantiate(Agent pAgent)
        {
            if (this.m_memberBase == null)
            {
                pAgent.UnInstantiate(this.m_variableName);
            }
        }

        public void UnLoad(Agent pAgent)
        {
            if (this.m_memberBase == null)
            {
                pAgent.UnLoad(this.m_variableName);
            }
        }

        public void SetDefaultValue(Property r)
        {
            object v = r.GetDefaultValue();
            this.SetDefaultValue(v);
        }

        public void SetDefaultValue(object v)
        {
            this.m_bValidDefaultValue = true;
            //this.m_defaultValue = Utils.Clone(v);
            this.m_defaultValue = v;
        }

        public bool SetDefaultValue(string valStr)
        {
            this.m_defaultValue = this.FromString(valStr);

            if (this.m_defaultValue != null)
            {
                this.m_bValidDefaultValue = true;

                return true;
            }

            return false;
        }

        public object FromString(string valStr)
        {
            Type type = null;

            if (this.m_memberBase != null)
            {
                type = this.m_memberBase.MemberType;
            }
            else if (this.m_bValidDefaultValue)
            {
                Debug.Check(this.m_defaultValue != null);
                type = this.m_defaultValue.GetType();
            }
            else
            {
                Debug.Check(false);
            }

            object v = StringUtils.FromString(type, valStr, false);

            return v;
        }

        public Property CreateElelmentAccessor(string vecotrAcessorIndex)
        {
            Property elementAccessor = new Property(this, vecotrAcessorIndex);

            return elementAccessor;
        }

        public bool SetDefaultValue(string valStr, Type type)
        {
            if (this.m_memberBase == null)
            {
                this.m_defaultValue = StringUtils.FromString(type, valStr, false);

                if (this.m_defaultValue != null)
                {
                    this.m_bValidDefaultValue = true;

                    return true;
                }
            }

            return false;
        }

        private object GetDefaultValue()
        {
            Debug.Check(this.m_bValidDefaultValue);

            return this.m_defaultValue;
        }

        protected virtual void SetValue(object value)
        {
            Debug.Check(false, "should call parent's SetValue method");
        }

        public void SetValue(Agent pSelf, object v)
        {
            Debug.Check(pSelf != null);
            Debug.Check(!m_bIsConst);

            if (this.m_parent != null)
            {
                this.SetVectorElement(pSelf, v);
                return;
            }

            string staticClassName = null;

            if (this.m_memberBase != null)
            {
                //don't update member as v is indeed the member itsef!
                //this.m_memberBase.Set(parent, v);

                if (this.m_memberBase.ISSTATIC())
                {
                    staticClassName = this.m_memberBase.GetClassNameString();
                }
            }
            else if (this.IsStatic)
            {
                staticClassName = pSelf.GetClassTypeName();
            }

            pSelf.SetVariableRegistry(this.m_bIsLocal, this.m_memberBase, this.m_variableName, v, staticClassName, this.m_variableId);
        }

        public object GetValue(Agent pSelf)
        {
            if (this.m_parent != null)
            {
                return this.GetVectorElement(pSelf);
            }

            if (pSelf == null || m_bIsConst)
            {
                return this.GetDefaultValue();
            }
            else
            {
                string staticClassName = null;

                if (this.m_memberBase != null)
                {
#if !BEHAVIAC_RELEASE
                    //Agent pInstance = this.GetParentAgent(pSelf);
                    //Debug.Check(pSelf == pInstance);
#endif
                }
                else if (this.IsStatic)
                {
                    staticClassName = pSelf.GetClassTypeName();
                }

                object pVariable = pSelf.GetVariableRegistry(staticClassName, this.m_memberBase, this.m_variableId);

                return pVariable;
            }
        }

        private object GetVectorElement(Agent pSelf)
        {
            Debug.Check(this.m_index != null);
            Agent parentParent = this.m_parent.GetParentAgent(pSelf);
            object parentObject = this.m_parent.GetValue(parentParent);

            if (parentObject != null)
            {
                Agent indexParent = this.m_index.GetParentAgent(pSelf);
                object indexObject = this.m_index.GetValue(indexParent);

                Debug.Check(parentObject is IList);
                IList asList = parentObject as IList;
                Debug.Check(indexObject is int);
                int index = (int)indexObject;

                if (index < asList.Count)
                {
                    object elementObject = asList[index];

                    return elementObject;
                }

                Debug.Check(false);
            }
            else
            {
                Debug.Check(true);
            }

            return null;
        }

        private void SetVectorElement(Agent pSelf, object v)
        {
            Debug.Check(this.m_index != null);
            Agent parentParent = this.m_parent.GetParentAgent(pSelf);
            object parentObject = this.m_parent.GetValue(parentParent);

            Agent indexParent = this.m_index.GetParentAgent(pSelf);
            object indexObject = this.m_index.GetValue(indexParent);

            Debug.Check(parentObject is IList);
            IList asList = parentObject as IList;
            Debug.Check(indexObject is int);
            int index = (int)indexObject;

            asList[index] = v;

#if !BEHAVIAC_RELEASE

            if (pSelf != null && pSelf.PlanningTop >= 0)
            {
                string varName = string.Format("{0}[{1}]", this.m_parent.Name, index);
                LogManager.Instance.LogVarValue(pSelf, varName, v);
            }

#endif
        }

        public object GetValue(Agent parent, Agent pSelf)
        {
            if (parent == null || m_bIsConst)
            {
                return this.GetDefaultValue();
            }
            else
            {
                if (this.m_parent != null)
                {
                    return this.GetVectorElement(pSelf);
                }

                string staticClassName = null;

                if (this.m_memberBase != null)
                {
                    Agent pInstance = this.GetParentAgent(pSelf);
                    pSelf = pInstance;
                }
                else if (this.IsStatic)
                {
                    staticClassName = pSelf.GetClassTypeName();
                }

                object pVariable = pSelf.GetVariableRegistry(staticClassName, this.m_memberBase, this.m_variableId);

                return pVariable;
            }
        }

#if K_TYPE_CREATOR_
        template<typename T>
        static bool Register(string typeName)
        {
            PropertyCreators()[typeName] = &Creator<T>;

            return true;
        }
        template<typename T>
        static void UnRegister(string typeName)
        {
            PropertyCreators().erase(typeName);
        }

        static void RegisterBasicTypes();
        static void UnRegisterBasicTypes();
#endif//#if K_TYPE_CREATOR_

        private static string ParseInstanceNameProperty(string fullName, ref string instanceName, ref string agentType)
        {
            //Self.AgentActionTest::Action2(0)
            int pClassBegin = fullName.IndexOf('.');

            if (pClassBegin != -1)
            {
                instanceName = fullName.Substring(0, pClassBegin).Replace("::", ".");

                string propertyName = fullName.Substring(pClassBegin + 1);
                int variableEnd = propertyName.LastIndexOf(':');
                Debug.Check(variableEnd != -1);

                agentType = propertyName.Substring(0, variableEnd - 1).Replace("::", ".");
                string variableName = propertyName.Substring(variableEnd + 1);
                return variableName;
            }

            return fullName;
        }

        public static Property Create(string typeName, string valueStr)
        {
            bool bConst = true;

            Property p = Property.create(null, bConst, typeName, null, null, valueStr);

            return p;
        }

        public static Property Create(string typeName, string fullName, bool bIsStatic, string arrayIndexStr)
        {
            string instanceName = null;
            string agentType = null;
            string variableName = ParseInstanceNameProperty(fullName, ref instanceName, ref agentType);

            Property pProperty = AgentProperties.GetProperty(agentType, variableName);

            if (pProperty != null)
            {
                Debug.Check(pProperty != null);

                if (pProperty.InstanceName != instanceName)
                {
                    Debug.Check(pProperty.Name == variableName);

                    Property pNew = pProperty.clone();
                    AgentProperties.AddPropertyInstance(agentType, pNew);

                    pProperty = pNew;
                    pProperty.InstanceName = instanceName;
                }

                Debug.Check(pProperty.IsStatic == bIsStatic);

                if (!string.IsNullOrEmpty(arrayIndexStr))
                {
                    Property vectorAccessor = pProperty.CreateElelmentAccessor(arrayIndexStr);

                    return vectorAccessor;
                }
            }
            else
            {
                //Debug.Check(false, "accessing a not declared local variable");
                string valueStr = null;
                pProperty = AgentProperties.AddLocal(agentType, typeName, variableName, valueStr);
            }

            return pProperty;
        }

        public static Property Create(string typeName, string instanceName, string agentType, string propertyName, string valueStr)
        {
            Debug.Check(!propertyName.EndsWith("]"));
            Debug.Check(!string.IsNullOrEmpty(propertyName));
            CMemberBase pMember = null;
            bool bConst = false;

            if (!string.IsNullOrEmpty(agentType))
            {
                pMember = Agent.FindMemberBase(agentType, propertyName);
            }
            else
            {
                Debug.Check(true);
            }

            Property p = Property.create(pMember, bConst, typeName, propertyName, instanceName, valueStr);

            return p;
        }

        public virtual Property clone()
        {
            Property p = new Property(this);

            return p;
        }

        public static void Cleanup()
        {
        }

        private static Property create(CMemberBase pMember, bool bConst, string typeName, string variableName, string instanceName, string valueStr)
        {
            Debug.Check(string.IsNullOrEmpty(variableName) || !variableName.EndsWith("]"));

            Property pProperty = null;

            if (pMember != null)
            {
                Debug.Check(!string.IsNullOrEmpty(variableName));
                pProperty = pMember.CreateProperty(valueStr, bConst);
                Debug.Check(pProperty != null);
            }
            else
            {
                pProperty = new Property(null, bConst);
                typeName = typeName.Replace("*", "");
                object v = PasrseTypeValue(typeName, valueStr);

                pProperty.SetDefaultValue(v);
            }

            Debug.Check(pProperty != null);

            if (!bConst)
            {
                Debug.Check(!string.IsNullOrEmpty(variableName));
                pProperty.Name = variableName;
                pProperty.InstanceName = instanceName;
            }

            return pProperty;
        }

        private static object PasrseTypeValue(string typeName, string valueStr)
        {
            bool bArrayType = false;
            Type type = Utils.GetTypeFromName(typeName, ref bArrayType);
            Debug.Check(type != null);

            if (bArrayType || !Utils.IsRefNullType(type))
            {
                if (!string.IsNullOrEmpty(valueStr))
                {
                    return StringUtils.FromString(type, valueStr, bArrayType);
                }
                else if (type == typeof(string))
                {
                    return string.Empty;
                }
            }

            return null;
        }

        protected string m_variableName;
        protected string m_instanceName;
        protected uint m_variableId;

        protected Property m_parent;
        protected Property m_index;

        protected readonly CMemberBase m_memberBase;

        protected object m_defaultValue;
        private bool m_bValidDefaultValue;
        protected readonly bool m_bIsConst;
    }

    public abstract class IVariable
    {
        public IVariable() { }
        public IVariable(CMemberBase pMember, string variableName, uint id)
        {
            m_id = id;
            m_name = variableName;
            m_property = null;
            m_pMember = pMember;
            m_instantiated = 1;
#if !BEHAVIAC_RELEASE
            m_changed = true;
#endif
        }

        public IVariable(CMemberBase pMember, Property property_)
        {
            m_property = property_;
            m_pMember = pMember;
            m_instantiated = 1;

            Debug.Check(this.m_property != null);

            this.m_name = this.m_property.Name;
            this.m_id = this.m_property.VariableId;
#if !BEHAVIAC_RELEASE
            m_changed = true;
#endif
        }

        public IVariable(IVariable copy)
        {
            m_id = copy.m_id;
            m_name = copy.m_name;
            m_property = copy.m_property;
            m_pMember = copy.m_pMember;
            m_instantiated = copy.m_instantiated;

#if !BEHAVIAC_RELEASE
            m_changed = copy.m_changed;
#endif
        }

        ~IVariable()
        { }

#if !BEHAVIAC_RELEASE

        public bool IsChanged()
        {
            return m_changed;
        }

#endif

        public uint GetId()
        {
            return this.m_id;
        }

        public Property GetProperty()
        {
            return this.m_property;
        }

        public void SetProperty(Property p)
        {
            if (p != null)
            {
                Debug.Check(this.m_name == p.Name);
                Debug.Check(this.m_id == p.VariableId);
            }

            this.m_property = p;
        }

        class TypeCreator
        {
            public delegate IVariable IVariableCreator();

            IVariableCreator creator;
            MethodInfo methodInfo;

            public TypeCreator(IVariableCreator c)
            {
                this.creator = c;
            }

            public TypeCreator(MethodInfo m)
            {
                this.methodInfo = m;
            }

            public IVariable Create()
            {
                if (creator != null)
                {
                    return creator();
                }
                else if (methodInfo != null)
                {
                    return (IVariable)methodInfo.Invoke(null, null);
                }
                else
                {
                    Debug.Check(false);
                }

                return null;
            }
        }

        static Dictionary<string, TypeCreator> ms_IVariableCreators = null;

        public static IVariable Creator<T>()
        {
            TVariable<T> p = new TVariable<T>();
            return p;
        }

        public static bool Register<T>(string typeName)
        {
            TypeCreator tc = new TypeCreator(Creator<T>);

            IVariableCreators()[typeName] = tc;

            string vectorTypeName = string.Format("vector<{0}>", typeName);

            TypeCreator tcl = new TypeCreator(Creator<List<T>>);
            IVariableCreators()[vectorTypeName] = tcl;
            return true;
        }

        public static void UnRegister<T>(string typeName)
        {
            IVariableCreators().Remove(typeName);

            string vectorTypeName = string.Format("vector<{0}>", typeName);

            IVariableCreators().Remove(vectorTypeName);
        }

        public static bool RegisterType(Type type, string typeName)
        {
            Type c = typeof(IVariable);
            MethodInfo mi = c.GetMethod("Creator");
            MethodInfo m = mi.MakeGenericMethod(type);
            TypeCreator tc = new TypeCreator(m);

            IVariableCreators()[typeName] = tc;

            string vectorTypeName = string.Format("vector<{0}>", typeName);
            Type tl = typeof(List<>).MakeGenericType(type);
            MethodInfo ml = mi.MakeGenericMethod(tl);
            TypeCreator tcl = new TypeCreator(ml);
            IVariableCreators()[vectorTypeName] = tcl;

            return true;
        }

        public static void UnRegisterType(Type type, string typeName)
        {
            IVariableCreators().Remove(typeName);

            string vectorTypeName = string.Format("vector<{0}>", typeName);

            IVariableCreators().Remove(vectorTypeName);
        }

        public static void RegisterBasicTypes()
        {
            IVariable.Register<bool>("bool");
            IVariable.Register<Boolean>("Boolean");
            IVariable.Register<byte>("byte");
            IVariable.Register<byte>("ubyte");
            IVariable.Register<Byte>("Byte");
            IVariable.Register<char>("char");
            IVariable.Register<Char>("Char");
            IVariable.Register<decimal>("decimal");
            IVariable.Register<Decimal>("Decimal");
            IVariable.Register<double>("double");
            IVariable.Register<Double>("Double");
            IVariable.Register<float>("float");
            IVariable.Register<int>("int");
            IVariable.Register<Int16>("Int16");
            IVariable.Register<Int32>("Int32");
            IVariable.Register<Int64>("Int64");
            IVariable.Register<long>("long");
            IVariable.Register<long>("llong");

            IVariable.Register<sbyte>("sbyte");
            IVariable.Register<SByte>("SByte");
            IVariable.Register<short>("short");
            IVariable.Register<ushort>("ushort");

            IVariable.Register<uint>("uint");
            IVariable.Register<UInt16>("UInt16");
            IVariable.Register<UInt32>("UInt32");
            IVariable.Register<UInt64>("UInt64");
            IVariable.Register<ulong>("ulong");
            IVariable.Register<ulong>("ullong");
            IVariable.Register<Single>("Single");
            IVariable.Register<string>("string");
            IVariable.Register<String>("String");
            IVariable.Register<object>("object");
            IVariable.Register<UnityEngine.GameObject>("UnityEngine.GameObject");
            IVariable.Register<UnityEngine.Vector2>("UnityEngine.Vector2");
            IVariable.Register<UnityEngine.Vector3>("UnityEngine.Vector3");
            IVariable.Register<UnityEngine.Vector4>("UnityEngine.Vector4");
            IVariable.Register<behaviac.Agent>("behaviac.Agent");
            IVariable.Register<behaviac.EBTStatus>("behaviac.EBTStatus");
        }

        public static void UnRegisterBasicTypes()
        {
            IVariable.UnRegister<bool>("bool");
            IVariable.UnRegister<Boolean>("Boolean");
            IVariable.UnRegister<byte>("byte");
            IVariable.UnRegister<byte>("ubyte");
            IVariable.UnRegister<Byte>("Byte");
            IVariable.UnRegister<char>("char");
            IVariable.UnRegister<Char>("Char");
            IVariable.UnRegister<decimal>("decimal");
            IVariable.UnRegister<Decimal>("Decimal");
            IVariable.UnRegister<double>("double");
            IVariable.UnRegister<Double>("Double");
            IVariable.UnRegister<float>("float");
            IVariable.UnRegister<Single>("Single");
            IVariable.UnRegister<int>("int");
            IVariable.UnRegister<Int16>("Int16");
            IVariable.UnRegister<Int32>("Int32");
            IVariable.UnRegister<Int64>("Int64");
            IVariable.UnRegister<long>("long");
            IVariable.UnRegister<long>("llong");
            IVariable.UnRegister<sbyte>("sbyte");
            IVariable.UnRegister<SByte>("SByte");
            IVariable.UnRegister<short>("short");
            IVariable.UnRegister<ushort>("ushort");

            IVariable.UnRegister<uint>("uint");
            IVariable.UnRegister<UInt16>("UInt16");
            IVariable.UnRegister<UInt32>("UInt32");
            IVariable.UnRegister<UInt64>("UInt64");
            IVariable.UnRegister<ulong>("ulong");
            IVariable.UnRegister<ulong>("ullong");

            IVariable.UnRegister<string>("string");
            IVariable.UnRegister<String>("String");
            IVariable.UnRegister<object>("object");
            IVariable.UnRegister<UnityEngine.GameObject>("UnityEngine.GameObject");
            IVariable.UnRegister<UnityEngine.Vector2>("UnityEngine.Vector2");
            IVariable.UnRegister<UnityEngine.Vector3>("UnityEngine.Vector3");
            IVariable.UnRegister<UnityEngine.Vector4>("UnityEngine.Vector4");
            IVariable.UnRegister<behaviac.Agent>("behaviac.Agent");
            IVariable.UnRegister<behaviac.EBTStatus>("behaviac.EBTStatus");
        }

        /// <summary>
        /// get IVariable Creators
        /// </summary>
        /// <returns></returns>
        static Dictionary<string, TypeCreator> IVariableCreators()
        {
            if (ms_IVariableCreators == null)
            {
                ms_IVariableCreators = new Dictionary<string, TypeCreator>();
            }

            return ms_IVariableCreators;
        }

        private static TypeCreator GetIVariableCreatorByTypeName(string typeName)
        {
            Dictionary<string, TypeCreator> d = IVariableCreators();

            if (!d.ContainsKey(typeName))
            {
                Debug.LogError(typeName + " can not found!");
            }

            TypeCreator pIVariableCreator = d[typeName];
            Debug.Check(pIVariableCreator != null);
            return pIVariableCreator;
        }

        public virtual void Log(Agent pAgent)
        {
            Debug.Check(false, "this step must go to parent's method");
        }

        public void Set(CMemberBase pMember, string variableName, uint id)
        {
            m_id = id;
            m_name = variableName;
            m_property = null;
            m_pMember = pMember;
            m_instantiated = 1;
            m_instantiated = 1;

#if !BEHAVIAC_RELEASE
            m_changed = true;
#endif
        }

        public void Set(CMemberBase pMember, Property property_, object value_)
        {
            m_pMember = pMember;
            m_property = property_;
            m_instantiated = 1;

            Debug.Check(this.m_property != null);

            this.m_name = this.m_property.Name;
            this.m_id = this.m_property.GetVariableId();
            this.SetValueObject(value_);
#if !BEHAVIAC_RELEASE
            m_changed = true;
#endif
        }

        public string Name
        {
            get
            {
                return this.m_name;
            }
        }

        public virtual object GetValueObject(Agent pAgent)
        {
            Debug.Check(false, "this step must go to parent's method");
            return null;
        }
        protected virtual void SetValueObject(object value)
        {
            Debug.Check(false, "should call parent's SetValue method");
        }
        public virtual void SetValueObject(object value, Agent pAgent)
        {
            Debug.Check(false, "should call parent's SetValue method");
        }

        public void Reset()
        {
#if !BEHAVIAC_RELEASE
            this.m_changed = false;
#endif
        }

        public virtual IVariable clone()
        {
            Debug.Check(false);
            return null;
        }
        public virtual void CopyTo(Agent pAgent)
        {
            Debug.Check(false, "should call parent's method");
        }
        public virtual void Save(ISerializableNode node)
        {
            //base.Save(node);
            Debug.Check(false);
        }
        public static IVariable CreateVariable(CMemberBase pMember, string typeNameIn, string variableName, uint varId)
        {

            string typeName = null;

            if (pMember != null)
            {
                //Debug.Check(!string.IsNullOrEmpty(typeNameIn));
                typeName = Utils.GetNativeTypeName(pMember.MemberType);
            }
            else
            {
                Debug.Check(!string.IsNullOrEmpty(typeNameIn));
                typeName = typeNameIn;
            }

            TypeCreator pIVariableCreator = GetIVariableCreatorByTypeName(typeName);

            IVariable pIV = pIVariableCreator.Create();

            pIV.Set(pMember, variableName, varId);

            return pIV;
            //behaviac.Debug.Check(pVar != null);
            //pVar.SetValue(value, null);
        }

        /// <summary>
        /// create a Variable
        /// </summary>
        /// <param name="property_"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IVariable CreateVariable(Property property_, object value)
        {
            string typeName = null;

            string fullName = property_.NativeTypeName;

            if (!string.IsNullOrEmpty(fullName))
            {
                typeName = fullName;
            }
            else
            {
                Debug.Check(false);
                typeName = "object";
            }

            TypeCreator pIVariableCreator = GetIVariableCreatorByTypeName(typeName);

            IVariable pIV = pIVariableCreator.Create();
            pIV.Set(null, property_, value);

            return pIV;
        }



        /// <summary>
        /// Deep copy
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <param name="obj"></param>
        /// TODO: disappear in 3.0 , but still used in 2.1.6
        public void Load(ISerializableNode node)
        {
            //base.Load(node);
        }

        public virtual void SetFromString(Agent pAgent, string valueString)
        {
            Debug.Check(false, "should call parent's method.");
        }

        protected uint m_id;
        protected string m_name;
        protected Property m_property;
        protected CMemberBase m_pMember;
        public byte m_instantiated;

#if !BEHAVIAC_RELEASE
        protected bool m_changed;
#endif
    };

    public class TVariable<VariableType> : IVariable
    {
        public override object GetValueObject(Agent pAgent)
        {
            VariableType v = GetValue(pAgent);
            return v;
        }
        protected override void SetValueObject(object value)
        {
            //force convert object type to VariableType not a good way, however it is only called in the Instantiate
            //this.m_value = (VariableType)value;
            //this.m_value = Utils.Clone((VariableType)value);
            Utils.Clone(ref this.m_value , (VariableType)value);
        }
        public override void SetValueObject(object v, Agent pAgent)
        {
            VariableType v_ = (VariableType)v;
            this.SetValue(v_, pAgent);
        }

        public TVariable() { }
        public TVariable(TVariable<VariableType> copy)
            : base(copy)
        {
            //this.m_value = Utils.Clone(copy.m_value);
            this.m_value = copy.m_value;
        }
        public TVariable(CMemberBase pMember, string variableName, uint varId)
            : base(pMember, variableName, varId)
        {
        }
        public TVariable(CMemberBase pMember, Property property_, VariableType value)
            : base(pMember, property_)
        {
            m_value = value;
        }
        public override IVariable clone()
        {
            IVariable pVar = new TVariable<VariableType>(this);
            return pVar;
        }
        public override void Log(Agent pAgent)
        {
            //BEHAVIAC_ASSERT(this.m_changed);

            LogManager.Instance.LogVarValue(pAgent, this.m_name, this.m_value);

#if !BEHAVIAC_RELEASE
            this.m_changed = false;
#endif
        }
        public override void CopyTo(Agent pAgent)
        {
            if (this.m_pMember != null)
            {
                this.m_pMember.Set(pAgent, m_value);
            }
            else
            {
                Debug.Check(true);
            }
        }
        public override void SetFromString(Agent pAgent, string valueString)
        {
            if (!string.IsNullOrEmpty(valueString))
            {
                object value = StringUtils.FromString(this.m_property.PropertyType, valueString, false);

                if (!(Details.Equals_(this.m_value, value)))
                {
                    this.m_value = (VariableType) value;
#if !BEHAVIAC_RELEASE
                    this.m_changed = true;
#endif

                    if (!Object.ReferenceEquals(pAgent, null))
                    {
                        if (this.m_pMember != null)
                        {
                            this.m_pMember.Set(pAgent, value);
                        }
                    }
                }
            }
        }
        public override void Save(ISerializableNode node)
        {
            //base.Save(node);
            CSerializationID variableId = new CSerializationID("var");
            ISerializableNode varNode = node.newChild(variableId);

            CSerializationID nameId = new CSerializationID("name");
            varNode.setAttr(nameId, this.m_name);

            CSerializationID valueId = new CSerializationID("value");
            varNode.setAttr(valueId, this.m_value);
        }

        public VariableType GetValue(Agent pAgent)
        {
            //don't use member property if in planning
            if (this.m_pMember != null && (pAgent == null || pAgent.PlanningTop == -1))
            {
                return (VariableType)this.m_pMember.Get(pAgent);
            }

            return this.m_value;
        }

        public void SetValue(VariableType value, Agent pAgent)
        {

            bool bProperty = false;

            //don't update member property if in planning
            if (this.m_pMember != null && (pAgent == null || pAgent.PlanningTop == -1))
            {
                this.m_pMember.Set(pAgent, value);
                //devlelopment version needs to update m_value even for property, as it needs to be used in the logging
#if !BEHAVIAC_RELEASE
#else
                bProperty = true;
#endif
            }

            if (!bProperty && !(Details.Equals_(this.m_value, value)))
            {
                if (this.m_value == null || !this.m_value.GetType().IsValueType)
                {
                    //this.m_value = Utils.Clone(value);
                    Utils.Clone(ref this.m_value , value);
                }
                else
                {
                    this.m_value = value;
                }

#if !BEHAVIAC_RELEASE
                Type valueType = this.m_value != null ? this.m_value.GetType() : null;
                Debug.Check(this.m_property == null || this.m_property.PropertyType == null || this.m_property.PropertyType == valueType);

                this.m_changed = true;

                if (pAgent != null && pAgent.PlanningTop >= 0)
                {
                    this.Log(pAgent);
                }

#endif
            }
            else
            {
                //don't clear it here, it will be cleared after being logged
                //this.m_changed = false;
            }
        }
        private VariableType m_value;
    }
    public class Variables
    {
        public Variables()
        {
            Debug.Check(this.m_variables.Count == 0);
        }

        ~Variables()
        {
            this.Clear();
        }

        public void Clear()
        {
            this.m_variables.Clear();
        }

        public bool IsExisting(uint varId)
        {
            return this.m_variables.ContainsKey(varId);
        }
        public void Instantiate(Agent pAgent, Property property_, object value)
        {
            Debug.Check(property_ != null);

            uint varId = property_.VariableId;

            if (!this.m_variables.ContainsKey(varId))
            {
                //IVariable pVar = new IVariable(null, property_);
                IVariable pVar = IVariable.CreateVariable(property_, value);

                //behaviac.Debug.Check(pVar != null);
                //pVar.SetValue(value, null);
                m_variables[varId] = pVar;
                //IVariable pVar = new IVariable(null, property_);
                //behaviac.Debug.Check(pVar != null);
                //pVar.SetValue(value, pAgent);
                //m_variables[varId] = pVar;
            }
            else
            {
                IVariable pVar = this.m_variables[varId];
                Debug.Check(pVar.m_instantiated < 255, "dead loop?!");

                //don't update it, so the par set by outer scope can override the one in the internal
                if (pVar.m_instantiated == 0)
                {
                    pVar.SetProperty(property_);
                }
                else
                {
                    Debug.Check(pVar.GetValueObject(pAgent) == null ||
                                (property_.GetValue(pAgent) == null && Utils.IsRefNullType(pVar.GetValueObject(pAgent).GetType())) ||
                                pVar.GetValueObject(pAgent).GetType() == property_.GetValue(pAgent).GetType(),
                                "the same name par doesn't have the same type");
                }

                //use the original value, don't update it
                pVar.m_instantiated++;
            }
        }
        public void UnInstantiate(string variableName)
        {
            Debug.Check(!string.IsNullOrEmpty(variableName));

            uint varId = Utils.MakeVariableId(variableName);

            //Debug.Check(this.m_variables.ContainsKey(varId));

            if (this.m_variables.ContainsKey(varId))
            {
                IVariable pVar = this.m_variables[varId];
                Debug.Check(pVar.m_instantiated >= 1);

                //don't erase it as it might be accessed after the bt's ticking
                //this.m_variables.erase(varId);
                pVar.m_instantiated--;

                if (pVar.m_instantiated == 0)
                {
                    pVar.SetProperty(null);
                }
            }
        }

        public void UnLoad(string variableName)
        {
            Debug.Check(!string.IsNullOrEmpty(variableName));

            uint varId = Utils.MakeVariableId(variableName);

            //Debug.Check(this.m_variables.ContainsKey(varId));

            if (this.m_variables.ContainsKey(varId))
            {
                this.m_variables.Remove(varId);
            }
        }

        public void SetFromString(Agent pAgent, string variableName, string valueStr)
        {
            Debug.Check(!string.IsNullOrEmpty(variableName));

            Debug.Check(variableName.LastIndexOf(':') == -1);

            uint varId = Utils.MakeVariableId(variableName);

            if (this.m_variables.ContainsKey(varId))
            {
                IVariable pVar = this.m_variables[varId];

                pVar.SetFromString(pAgent, valueStr);
            }
        }
        public virtual void SetObject(bool bMemberSet, Agent pAgent, bool bLocal, CMemberBase pMember, string variableName, object value, uint varId)
        {
            Debug.Check(!string.IsNullOrEmpty(variableName));

            if (varId == 0)
            {
                varId = Utils.MakeVariableId(variableName);
            }

            if (!this.m_variables.ContainsKey(varId))
            {
                if (bMemberSet)
                {
                    if (pMember == null)
                    {
                        if (pAgent != null)
                        {
                            pMember = pAgent.FindMember(variableName);
                        }
                    }
                }
                else
                {
                    pMember = null;
                }

                if (value != null)
                {
                    string typeName = Utils.GetNativeTypeName(value.GetType());
                    Debug.Check(!string.IsNullOrEmpty(typeName));

                    IVariable pVar = IVariable.CreateVariable(pMember, typeName, variableName, varId);
                    behaviac.Debug.Check(pVar != null);
                    m_variables[varId] = pVar;
                    pVar.SetValueObject(value, pAgent);
                }
            }
            else
            {
                IVariable pVar = this.m_variables[varId];
                pVar.SetValueObject(value, pAgent);
            }
        }

        public virtual void Set<VariableType>(bool bMemberSet, Agent pAgent, bool bLocal, CMemberBase pMember, string variableName, VariableType value, uint varId)
        {
            Debug.Check(!string.IsNullOrEmpty(variableName));

            if (varId == 0)
            {
                varId = Utils.MakeVariableId(variableName);
            }

            if (!this.m_variables.ContainsKey(varId))
            {
                if (bMemberSet)
                {
                    if (pMember == null)
                    {
                        if (pAgent != null)
                        {
                            pMember = pAgent.FindMember(variableName);
                        }
                    }
                }
                else
                {
                    pMember = null;
                }

                string typeName = Utils.GetNativeTypeName(typeof(VariableType));
                IVariable pVar = IVariable.CreateVariable(pMember, typeName, variableName, varId);
                behaviac.Debug.Check(pVar != null);
                m_variables[varId] = pVar;
                TVariable<VariableType> tpVar = (TVariable<VariableType>)pVar;
                tpVar.SetValue(value, pAgent);
            }
            else
            {
                IVariable pVar = this.m_variables[varId];
                TVariable<VariableType> tpVar = (TVariable<VariableType>)pVar;
                tpVar.SetValue(value, pAgent);
            }
        }

        public virtual object GetObject(Agent pAgent, bool bMemberGet, CMemberBase pMember, uint varId)
        {
            if (!this.m_variables.ContainsKey(varId))
            {
                if (bMemberGet)
                {
                    if (pMember != null)
                    {
                        object val = pMember.Get(pAgent);
                        return val;
                    }
                }

                //Debug.Check(false, "a compatible property is not found");
                return null;
            }
            else
            {
                //par
                IVariable pVar = this.m_variables[varId];
                Debug.Check(pVar != null);

                //if out of scope
                if (pVar.m_instantiated > 0)
                {
                    return pVar.GetValueObject(pAgent);
                }
                else
                {
                    string msg = string.Format("A Local '{0}' has been out of scope!", pVar.Name);
                    Debug.LogWarning(msg);
                }
            }

            return null;
        }

        public virtual VariableType Get<VariableType>(Agent pAgent, bool bMemberGet, CMemberBase pMember, uint varId)
        {
            if (!this.m_variables.ContainsKey(varId))
            {
                if (bMemberGet)
                {
                    if (pMember != null)
                    {
                        VariableType val = (VariableType)pMember.Get(pAgent);
                        return val;
                    }
                }

                //Debug.Check(false, "a compatible property is not found");
                return default(VariableType);
            }
            else
            {
                //par
                IVariable pVar = this.m_variables[varId];
                Debug.Check(pVar != null);

                //if out of scope
                if (pVar.m_instantiated > 0)
                {
                    return (VariableType)pVar.GetValueObject(pAgent);
                }
            }

            return default(VariableType);
        }

        public virtual void Log(Agent pAgent, bool bForce)
        {
#if !BEHAVIAC_RELEASE
            if (Config.IsLoggingOrSocketing)
            {
                var e = this.m_variables.Values.GetEnumerator();
                while (e.MoveNext())
                {
                    if (bForce || e.Current.IsChanged())
                    {
                        e.Current.Log(pAgent);
                    }
                }
            }
#endif
        }

        public void Reset()
        {
            var e = this.m_variables.Values.GetEnumerator();
            while (e.MoveNext())
            {
                e.Current.Reset();
            }
        }

        public void Unload()
        {
            this.m_variables.Clear();
        }

        public void CopyTo(Agent pAgent, Variables target)
        {
            target.m_variables.Clear();

            var e = this.m_variables.Values.GetEnumerator();
            while (e.MoveNext())
            {
                IVariable pNew = e.Current.clone();

                target.m_variables[pNew.GetId()] = pNew;
            }

            if (!Object.ReferenceEquals(pAgent, null))
            {
                e = target.m_variables.Values.GetEnumerator();
                while (e.MoveNext())
                {
                    e.Current.CopyTo(pAgent);
                }
            }
        }

        private void Save(ISerializableNode node)
        {
            CSerializationID variablesId = new CSerializationID("vars");
            ISerializableNode varsNode = node.newChild(variablesId);

            var e = this.m_variables.Values.GetEnumerator();
            while (e.MoveNext())
            {
                e.Current.Save(varsNode);
            }
        }

        private void Load(ISerializableNode node)
        {
        }

        protected Dictionary<uint, IVariable> m_variables = new Dictionary<uint, IVariable>();

        public Dictionary<uint, IVariable> Vars
        {
            get
            {
                return this.m_variables;
            }
        }
    };
}//namespace behaviac
