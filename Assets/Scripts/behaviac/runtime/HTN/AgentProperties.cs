using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using Mono.Xml;

namespace behaviac
{
    public partial class AgentProperties
    {
        private Dictionary<uint, Property> m_properties = new Dictionary<uint, Property>();
        private Dictionary<uint, Property> m_locals = new Dictionary<uint, Property>();
        private List<Property> m_properties_instance = new List<Property>();

        // called before loading files, otherwise, locals have been added and will be instantiated
        public void Instantiate(Agent pAgent)
        {
            var e = this.m_properties.Values.GetEnumerator();
            while (e.MoveNext())
            {
                e.Current.Instantiate(pAgent);
            }
        }

        private Property AddProperty(string typeName, bool bIsStatic, string variableName, string valueStr, string agentType)
        {
            Property pProperty = Property.Create(typeName, "Self", agentType, variableName, valueStr);
            pProperty.IsStatic = bIsStatic;

            pProperty.NativeTypeName = typeName;
            uint variableId = Utils.MakeVariableId(variableName);
            this.m_properties[variableId] = pProperty;
            return pProperty;
        }

        private Property GetProperty(uint variableId)
        {
            if (this.m_properties.ContainsKey(variableId))
            {
                return this.m_properties[variableId];
            }

            if (this.m_locals.ContainsKey(variableId))
            {
                return this.m_locals[variableId];
            }

            return null;
        }

        private Property GetProperty(string variableName)
        {
            uint variableId = Utils.MakeVariableId(variableName);

            Property p = this.GetProperty(variableId);

            return p;
        }

        public Property GetLocal(string variableName)
        {
            uint variableId = Utils.MakeVariableId(variableName);

            if (this.m_locals.ContainsKey(variableId))
            {
                return this.m_locals[variableId];
            }

            return null;
        }

        public void AddPropertyInstance(Property pPropertyInstance)
        {
            m_properties_instance.Add(pPropertyInstance);
        }

        private Property AddLocal(string typeName, string variableName, string valueStr)
        {
            Debug.Check(!variableName.EndsWith("]"));

            string agentType = null;
            Property pProperty = Property.Create(typeName, "Self", agentType, variableName, valueStr);
            pProperty.IsLocal = true;

            pProperty.NativeTypeName = typeName;

            uint variableId = Utils.MakeVariableId(variableName);
            this.m_locals[variableId] = pProperty;
            return pProperty;
        }

        public static Property AddLocal(string agentType, string typeName, string variableName, string valueStr)
        {
            AgentProperties bb = AgentProperties.Get(agentType);

            //if agent type has no property and custom property
            if (bb == null)
            {
                bb = new AgentProperties(agentType);
                agent_type_blackboards[agentType] = bb;
            }

            Property pProperty = bb.AddLocal(typeName, variableName, valueStr);

            return pProperty;
        }

        public static Property GetProperty(string agentType, string variableName)
        {
            AgentProperties bb = AgentProperties.Get(agentType);
            Debug.Check(bb != null);
            Property pProperty = bb.GetProperty(variableName);

            return pProperty;
        }

        public static Property GetProperty(string agentType, uint variableId)
        {
            AgentProperties bb = AgentProperties.Get(agentType);
            Debug.Check(bb != null);
            Property pProperty = bb.GetProperty(variableId);

            return pProperty;
        }

        public static void AddPropertyInstance(string agentType, Property pPropertyInstance)
        {
            AgentProperties bb = AgentProperties.Get(agentType);
            Debug.Check(bb != null);

            bb.AddPropertyInstance(pPropertyInstance);
        }


        private Type agent_type;

        public Type AgentType
        {
            get
            {
                return this.agent_type;
            }
        }

        public AgentProperties(string agentType)
        {
            this.agent_type = Agent.GetTypeFromName(agentType);
        }

        private static Dictionary<string, AgentProperties> agent_type_blackboards = new Dictionary<string, AgentProperties>();

        public static AgentProperties Get(string agentType)
        {
            if (agent_type_blackboards.ContainsKey(agentType))
            {
                return agent_type_blackboards[agentType];
            }

            return null;
        }

        private void ClearLocals()
        {
            //this.m_properties.Clear();
            this.m_locals.Clear();
        }

        private void cleanup()
        {
            this.m_properties_instance.Clear();

            this.m_properties.Clear();
            this.m_locals.Clear();
        }

        public static void UnloadLocals()
        {
            var e = agent_type_blackboards.Values.GetEnumerator();
            while (e.MoveNext())
            {
                e.Current.ClearLocals();
            }

            //don't clear agent types
            //agent_type_blackboards.Clear();
        }

        public static void Cleanup()
        {
            var e = agent_type_blackboards.Values.GetEnumerator();
            while (e.MoveNext())
            {
                e.Current.cleanup();
            }

            agent_type_blackboards.Clear();
        }

        #region Serialize

        static partial void load_cs();

        private static bool _generatedRegisterationTypes = false;
        public static bool GeneratedRegisterationTypes
        {
            get { return _generatedRegisterationTypes; }
            set { _generatedRegisterationTypes = value; }
        }

        static partial void RegisterTypes_();

        static partial void UnRegisterTypes_();

        public static void RegisterTypes()
        {
            GeneratedRegisterationTypes = false;

            RegisterTypes_();
        }

        public static void UnRegisterTypes()
        {
            GeneratedRegisterationTypes = false;

            UnRegisterTypes_();
        }

        private static bool load_xml(byte[] pBuffer)
        {
            try
            {
                Debug.Check(pBuffer != null);
                string xml = System.Text.Encoding.UTF8.GetString(pBuffer);

                SecurityParser xmlDoc = new SecurityParser();
                xmlDoc.LoadXml(xml);

                SecurityElement rootNode = xmlDoc.ToXml();

                if (rootNode.Children == null || rootNode.Tag != "agents" && rootNode.Children.Count != 1)
                {
                    return false;
                }

                string versionStr = rootNode.Attribute("version");
                Debug.Check(!string.IsNullOrEmpty(versionStr));

                foreach(SecurityElement bbNode in rootNode.Children)
                {
                    if (bbNode.Tag == "agent" && bbNode.Children != null)
                    {
                        string agentType = bbNode.Attribute("type").Replace("::", ".");

                        AgentProperties bb = new AgentProperties(agentType);
                        agent_type_blackboards[agentType] = bb;

                        foreach(SecurityElement propertiesNode in bbNode.Children)
                        {
                            if (propertiesNode.Tag == "properties" && propertiesNode.Children != null)
                            {
                                foreach(SecurityElement propertyNode in propertiesNode.Children)
                                {
                                    if (propertyNode.Tag == "property")
                                    {
                                        string name = propertyNode.Attribute("name");
                                        string type = propertyNode.Attribute("type").Replace("::", ".");
                                        string memberStr = propertyNode.Attribute("member");
                                        bool bIsMember = false;

                                        if (!string.IsNullOrEmpty(memberStr) && memberStr == "true")
                                        {
                                            bIsMember = true;
                                        }

                                        string isStatic = propertyNode.Attribute("static");
                                        bool bIsStatic = false;

                                        if (!string.IsNullOrEmpty(isStatic) && isStatic == "true")
                                        {
                                            bIsStatic = true;
                                        }

                                        //string agentTypeMember = agentType;
                                        string agentTypeMember = null;
                                        string valueStr = null;

                                        if (!bIsMember)
                                        {
                                            valueStr = propertyNode.Attribute("defaultvalue");
                                        }
                                        else
                                        {
                                            agentTypeMember = propertyNode.Attribute("agent").Replace("::", ".");
                                        }

                                        bb.AddProperty(type, bIsStatic, name, valueStr, agentTypeMember);
                                    }
                                }
                            }
                            else if (propertiesNode.Tag == "methods" && propertiesNode.Children != null)
                            {
                                Agent.CTagObjectDescriptor objectDesc = Agent.GetDescriptorByName(agentType);
                                foreach(SecurityElement methodNode in propertiesNode.Children)
                                {
                                    if (methodNode.Tag == "method")
                                    {
                                        //string eventStr = methodNode.Attribute("isevent");
                                        //bool bEvent = (eventStr == "true");
                                        //string taskStr = methodNode.Attribute("istask");
                                        //bool bTask = (taskStr == "true");
                                        //skip those other custom method
                                        string methodName = methodNode.Attribute("name");
                                        //string type = methodNode.Attribute("returntype").Replace("::", ".");
                                        //string isStatic = methodNode.Attribute("static");
                                        //string agentTypeStr = methodNode.Attribute("agent").Replace("::", ".");
                                        CCustomMethod customeMethod = new CTaskMethod(agentType, methodName);

                                        if (methodNode.Children != null)
                                        {
                                            foreach(SecurityElement paramNode in methodNode.Children)
                                            {
                                                if (paramNode.Tag == "parameter")
                                                {
                                                    string paramName = paramNode.Attribute("name");
                                                    Debug.Check(!string.IsNullOrEmpty(paramName));

                                                    string paramType = paramNode.Attribute("type");

                                                    //string paramFullName = string.Format("{0}::{1}", paramType, paramName);

                                                    customeMethod.AddParamType(paramType);
                                                }
                                            }
                                        }

                                        objectDesc.ms_methods.Add(customeMethod);
                                    }
                                }//end of for methodNode
                            }//end of methods
                        }//end of for propertiesNode
                    }
                }//end of for bbNode

                return true;
            }
            catch (Exception e)
            {
                Debug.Check(false, e.Message);
            }

            Debug.Check(false);
            return false;
        }

        private static bool load_bson(byte[] pBuffer)
        {
            try
            {
                BsonDeserizer d = new BsonDeserizer();

                if (d.Init(pBuffer))
                {
                    BsonDeserizer.BsonTypes type = d.ReadType();

                    if (type == BsonDeserizer.BsonTypes.BT_AgentsElement)
                    {
                        bool bOk = d.OpenDocument();
                        Debug.Check(bOk);

                        string verStr = d.ReadString();
                        int version = int.Parse(verStr);

                        {
                            type = d.ReadType();

                            while (type != BsonDeserizer.BsonTypes.BT_None)
                            {
                                if (type == BsonDeserizer.BsonTypes.BT_AgentElement)
                                {
                                    load_agent(version, d);
                                }

                                type = d.ReadType();
                            }

                            Debug.Check(type == BsonDeserizer.BsonTypes.BT_None);
                        }

                        d.CloseDocument(false);
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Check(false, e.Message);
            }

            Debug.Check(false);
            return false;
        }

        private static bool load_agent(int version, BsonDeserizer d)
        {
            try
            {
                d.OpenDocument();

                string agentType = d.ReadString().Replace("::", ".");
                string pBaseName = d.ReadString();
                Debug.Check(!string.IsNullOrEmpty(pBaseName));
                AgentProperties bb = new AgentProperties(agentType);
                agent_type_blackboards[agentType] = bb;

                BsonDeserizer.BsonTypes type = d.ReadType();

                while (type != BsonDeserizer.BsonTypes.BT_None)
                {
                    if (type == BsonDeserizer.BsonTypes.BT_PropertiesElement)
                    {
                        d.OpenDocument();
                        type = d.ReadType();

                        while (type != BsonDeserizer.BsonTypes.BT_None)
                        {
                            if (type == BsonDeserizer.BsonTypes.BT_PropertyElement)
                            {
                                d.OpenDocument();
                                string variableName = d.ReadString();
                                string typeName = d.ReadString();
                                string memberStr = d.ReadString();
                                bool bIsMember = false;

                                if (!string.IsNullOrEmpty(memberStr) && memberStr == "true")
                                {
                                    bIsMember = true;
                                }

                                string isStatic = d.ReadString();
                                bool bIsStatic = false;

                                if (!string.IsNullOrEmpty(isStatic) && isStatic == "true")
                                {
                                    bIsStatic = true;
                                }

                                string valueStr = null;
                                //string agentTypeMember = agentType;
                                string agentTypeMember = null;

                                if (!bIsMember)
                                {
                                    valueStr = d.ReadString();
                                }
                                else
                                {
                                    agentTypeMember = d.ReadString().Replace("::", ".");
                                }

                                d.CloseDocument(true);

                                bb.AddProperty(typeName, bIsStatic, variableName, valueStr, agentTypeMember);
                            }
                            else
                            {
                                Debug.Check(false);
                            }

                            type = d.ReadType();
                        }//end of while

                        d.CloseDocument(false);
                    }
                    else if (type == BsonDeserizer.BsonTypes.BT_MethodsElement)
                    {
                        load_methods(d, agentType, type);
                    }
                    else
                    {
                        Debug.Check(type == BsonDeserizer.BsonTypes.BT_None);
                    }

                    type = d.ReadType();
                }

                d.CloseDocument(false);
                return true;
            }
            catch (Exception ex)
            {
                Debug.Check(false, ex.Message);
            }

            return false;
        }

        //return true if there is a task method loaded
        private static void load_methods(BsonDeserizer d, string agentType, BsonDeserizer.BsonTypes type)
        {
            Agent.CTagObjectDescriptor objectDesc = Agent.GetDescriptorByName(agentType);
            d.OpenDocument();
            type = d.ReadType();

            while (type == BsonDeserizer.BsonTypes.BT_MethodElement)
            {
                d.OpenDocument();

                string methodName = d.ReadString();
                //string returnTypeStr = d.ReadString();
                //returnTypeStr = returnTypeStr.Replace("::", ".");
                //string isStatic = d.ReadString();
                //string eventStr = d.ReadString();
                //bool bEvent = (eventStr == "true");
                string agentStr = d.ReadString();
                Debug.Check(!string.IsNullOrEmpty(agentStr));

                CCustomMethod customeMethod = new CTaskMethod(agentType, methodName);

                type = d.ReadType();

                while (type == BsonDeserizer.BsonTypes.BT_ParameterElement)
                {
                    d.OpenDocument();
                    string paramName = d.ReadString();
                    Debug.Check(!string.IsNullOrEmpty(paramName));
                    string paramType = d.ReadString();

                    customeMethod.AddParamType(paramType);

                    d.CloseDocument(true);
                    type = d.ReadType();
                }

                objectDesc.ms_methods.Add(customeMethod);

                d.CloseDocument(false);
                type = d.ReadType();
            }

            d.CloseDocument(false);
        }

        public static bool Load()
        {
            string relativePath = "behaviac.bb";
            string fullPath = Path.Combine(Workspace.Instance.FilePath, relativePath);

            bool bLoadResult = false;
            Workspace.EFileFormat f = Workspace.Instance.FileFormat;
            string ext = "";

            Workspace.Instance.HandleFileFormat(fullPath, ref ext, ref f);

            switch (f)
            {
                case Workspace.EFileFormat.EFF_default:
                    Debug.Check(false);
                    break;

                case Workspace.EFileFormat.EFF_xml:
                {
                    byte[] pBuffer = Workspace.Instance.ReadFileToBuffer(fullPath, ext);

                    if (pBuffer != null)
                    {
                        bLoadResult = load_xml(pBuffer);

                        Workspace.Instance.PopFileFromBuffer(fullPath, ext, pBuffer);
                    }
                    else
                    {
                        Debug.LogError(string.Format("'{0}' doesn't exist!, Please override Workspace and its GetWorkspaceExportPath()", fullPath));
                        Debug.Check(false);
                    }
                }
                break;

                case Workspace.EFileFormat.EFF_bson:
                {
                    byte[] pBuffer = Workspace.Instance.ReadFileToBuffer(fullPath, ext);

                    if (pBuffer != null)
                    {
                        bLoadResult = load_bson(pBuffer);

                        Workspace.Instance.PopFileFromBuffer(fullPath, ext, pBuffer);
                    }
                    else
                    {
                        Debug.LogError(string.Format("'{0}' doesn't exist!, Please override Workspace and its GetWorkspaceExportPath()", fullPath));
                        Debug.Check(false);
                    }
                }
                break;

                case Workspace.EFileFormat.EFF_cs:
                {
                    load_cs();
                }
                break;

                default:
                    Debug.Check(false);
                    break;
            }

            return bLoadResult;
        }

        #endregion Serialize
    }
}
