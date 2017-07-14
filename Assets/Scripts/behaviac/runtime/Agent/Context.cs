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
using System.Collections.Generic;

namespace behaviac
{
    public class Context
    {
        private static Dictionary<int, Context> ms_contexts = new Dictionary<int, Context>();

        private Dictionary<string, Agent> m_namedAgents = new Dictionary<string, Agent>();
        private Dictionary<string, Variables> m_static_variables = new Dictionary<string, Variables>();

        public struct HeapItem_t : IComparable<HeapItem_t>
        {
            public int priority;
            public Dictionary<int, Agent> agents;

            public int CompareTo(HeapItem_t other)
            {
                if (this.priority < other.priority)
                {
                    return -1;
                }
                else if (this.priority > other.priority)
                {
                    return 1;
                }

                return 0;
            }
        }

        private List<HeapItem_t> m_agents;

        public List<HeapItem_t> Agents
        {
            get
            {
                if (m_agents == null)
                {
                    m_agents = new List<HeapItem_t>();
                }

                return m_agents;
            }
            set
            {
                m_agents = value;
            }
        }

        private int m_context_id;

        private Context(int contextId)
        {
            m_context_id = contextId;
        }

        ~Context()
        {
            delayAddedAgents.Clear();
            delayRemovedAgents.Clear();

            this.CleanupStaticVariables();
            this.CleanupInstances();
        }

        private int GetContextId()
        {
            return this.m_context_id;
        }

        public static Context GetContext(int contextId)
        {
            Debug.Check(contextId >= 0);

            if (ms_contexts.ContainsKey(contextId))
            {
                Context pContext = ms_contexts[contextId];
                return pContext;
            }

            Context pC = new Context(contextId);
            ms_contexts[contextId] = pC;

            return pC;
        }

        public static void Cleanup(int contextId)
        {
            if (ms_contexts != null)
            {
                if (contextId == -1)
                {
                    ms_contexts.Clear();
                    //ms_contexts = null;
                }
                else
                {
                    if (ms_contexts.ContainsKey(contextId))
                    {
                        ms_contexts.Remove(contextId);
                    }
                    else
                    {
                        Debug.Check(false, "unused context id");
                    }
                }
            }
        }

        /**
        log changed static variables(propery) for the specified agent class or all agent classes

        @param agentClassName
        if null, it logs for all the agent class
        */

        private void LogStaticVariables(string agentClassName)
        {
            if (!string.IsNullOrEmpty(agentClassName))
            {
                if (m_static_variables.ContainsKey(agentClassName))
                {
                    Variables variables = m_static_variables[agentClassName];

                    variables.Log(null, false);
                }
            }
            else
            {
                var e = m_static_variables.Values.GetEnumerator();
                while (e.MoveNext())
                {
                    e.Current.Log(null, false);
                }
            }
        }

        private List<Agent> delayAddedAgents = new List<Agent>();
        private List<Agent> delayRemovedAgents = new List<Agent>();

        public static void AddAgent(Agent pAgent)
        {
            if (pAgent != null)
            {
                Context c = Context.GetContext(pAgent.GetContextId());

                c.delayAddedAgents.Add(pAgent);
            }
        }

        public static void RemoveAgent(Agent pAgent)
        {
            if (pAgent != null)
            {
                Context c = Context.GetContext(pAgent.GetContextId());

                c.delayRemovedAgents.Add(pAgent);
            }
        }

        private void DelayProcessingAgents()
        {
            for (int i = 0; i < delayAddedAgents.Count; ++i)
            {
                addAgent_(delayAddedAgents[i]);
            }

            for (int i = 0; i < delayRemovedAgents.Count; ++i)
            {
                removeAgent_(delayRemovedAgents[i]);
            }

            delayAddedAgents.Clear();
            delayRemovedAgents.Clear();
        }

        private void addAgent_(Agent pAgent)
        {
            //ASSERT_MAIN_THREAD();
            int agentId = pAgent.GetId();
            int priority = pAgent.GetPriority();
            int itemIndex = this.Agents.FindIndex(delegate(HeapItem_t h)
            {
                return h.priority == priority;
            });

            if (itemIndex == -1)
            {
                HeapItem_t pa = new HeapItem_t();
                pa.agents = new Dictionary<int, Agent>();
                pa.priority = priority;
                pa.agents[agentId] = pAgent;
                this.Agents.Add(pa);
            }
            else
            {
                this.Agents[itemIndex].agents[agentId] = pAgent;
            }
        }

        private void removeAgent_(Agent pAgent)
        {
            //ASSERT_MAIN_THREAD();

            int agentId = pAgent.GetId();
            int priority = pAgent.GetPriority();
            int itemIndex = this.Agents.FindIndex(delegate(HeapItem_t h)
            {
                return h.priority == priority;
            });

            if (itemIndex != -1)
            {
                if (this.Agents[itemIndex].agents.ContainsKey(agentId))
                {
                    this.Agents[itemIndex].agents.Remove(agentId);
                }
            }
        }

        public static void execAgents(int contextId)
        {
            if (contextId >= 0)
            {
                Context pContext = Context.GetContext(contextId);

                pContext.execAgents_();
            }
            else
            {
                var e = ms_contexts.Values.GetEnumerator();
                while (e.MoveNext())
                {
                    Context pContext = e.Current;

                    pContext.execAgents_();
                }
            }
        }

        private void execAgents_()
        {
            this.DelayProcessingAgents();

            this.Agents.Sort();

            for (int i = 0; i < this.Agents.Count; ++i)
            {
                HeapItem_t pa = this.Agents[i];
                var e = pa.agents.Values.GetEnumerator();

                while (e.MoveNext())
                {
                    if (e.Current.IsActive())
                    {
                        e.Current.btexec();

                        //in case IsExecAgents was set to false by pA's bt
                        if (!Workspace.Instance.IsExecAgents)
                        {
                            break;
                        }
                    }
                }
            }

            if (Agent.IdMask() != 0)
            {
                this.LogStaticVariables(null);
            }
        }

        private void LogCurrentState()
        {
            string msg = string.Format("LogCurrentStates {0} {1}", this.m_context_id, this.Agents.Count);
            behaviac.Debug.Log(msg);

            //force to log vars
            for (int i = 0; i < this.Agents.Count; ++i)
            {
                HeapItem_t pa = this.Agents[i];
                var e = pa.agents.Values.GetEnumerator();
                while (e.MoveNext())
                {
                    if (e.Current.IsMasked())
                    {
                        e.Current.LogVariables(true);
                    }
                }
            }

            this.LogStaticVariables(null);
        }

        public static void LogCurrentStates(int contextId)
        {
            Debug.Check(ms_contexts != null);

            if (contextId >= 0)
            {
                Context pContext = Context.GetContext(contextId);
                pContext.LogCurrentState();
            }
            else
            {
                var e = ms_contexts.Values.GetEnumerator();
                while (e.MoveNext())
                {
                    e.Current.LogCurrentState();
                }
            }
        }

        private void CleanupStaticVariables()
        {
            var e = m_static_variables.Values.GetEnumerator();
            while (e.MoveNext())
            {
                e.Current.Clear();
            }

            m_static_variables.Clear();
        }

        public void ResetChangedVariables()
        {
            var e = m_static_variables.Values.GetEnumerator();
            while (e.MoveNext())
            {
                e.Current.Reset();
            }
        }

        private void CleanupInstances()
        {
            //foreach (KeyValuePair<string, Agent> p in m_namedAgents)
            //{
            //    string msg = string.Format("{0}:{1}", p.Key,p.Value.GetName());
            //    behaviac.Debug.Log(msg);
            //}

            //Debug.Check(m_namedAgents.Count == 0, "you need to call DestroyInstance or UnbindInstance");

            m_namedAgents.Clear();
        }

        public object GetStaticVariable(string staticClassName, uint variableId)
        {
            Debug.Check(!string.IsNullOrEmpty(staticClassName));

            if (!m_static_variables.ContainsKey(staticClassName))
            {
                m_static_variables[staticClassName] = new Variables();
            }

            Variables variables = m_static_variables[staticClassName];
            return variables.GetObject(null, false, null, variableId);
        }

        /**
        if staticClassName is no null, it is for static variable
        */
        /// <summary>
        /// if the caller's third parameter's type is object
        /// </summary>
        /// <param name="pMember"></param>
        /// <param name="variableName"></param>
        /// <param name="value"></param>
        /// <param name="staticClassName"></param>
        /// <param name="variableId"></param>
        public void SetStaticVariableObject(CMemberBase pMember, string variableName, object value, string staticClassName, uint variableId)
        {
            Debug.Check(!string.IsNullOrEmpty(variableName));
            Debug.Check(!string.IsNullOrEmpty(staticClassName));

            if (!m_static_variables.ContainsKey(staticClassName))
            {
                m_static_variables[staticClassName] = new Variables();
            }

            Variables variables = m_static_variables[staticClassName];
            //TODO: ture and false add by notice by below
            variables.SetObject(true, null, false, pMember, variableName, value, variableId);
        }

        public void SetStaticVariable<VariableType>(CMemberBase pMember, string variableName, VariableType value, string staticClassName, uint variableId)
        {
            Debug.Check(!string.IsNullOrEmpty(variableName));
            Debug.Check(!string.IsNullOrEmpty(staticClassName));

            if (!m_static_variables.ContainsKey(staticClassName))
            {
                m_static_variables[staticClassName] = new Variables();
            }

            Variables variables = m_static_variables[staticClassName];
            variables.Set(true, null, false, pMember, variableName, value, variableId);
        }

        private static bool GetClassNameString(string variableName, ref string className)
        {
            Debug.Check(!string.IsNullOrEmpty(variableName));

            int pSep = variableName.LastIndexOf(':');

            if (pSep > 0)
            {
                Debug.Check(variableName[pSep - 1] == ':');
                className = variableName.Substring(0, pSep - 1);

                return true;
            }
            else
            {
                className = variableName;
                return true;
            }

            //return false;
        }

        public CNamedEvent FindNamedEventTemplate(List<CMethodBase> methods, string eventName)
        {
            CStringID eventID = new CStringID(eventName);

            //reverse, so the event in the derived class can override the one in the base class
            for (int i = methods.Count - 1; i >= 0; --i)
            {
                CMethodBase pMethod = methods[i];
                string methodName = pMethod.Name;
                CStringID methodID = new CStringID(methodName);

                if (methodID == eventID && pMethod.IsNamedEvent())
                {
                    Debug.Check(pMethod is CNamedEvent);
                    CNamedEvent pNamedMethod = (CNamedEvent)pMethod;

                    return pNamedMethod;
                }
            }

            return null;
        }

        /**
        bind 'agentInstanceName' to 'pAgentInstance'.
        'agentInstanceName' should have been registered to the class of 'pAgentInstance' or its parent class.

        @sa RegisterInstanceName
        */

        public bool BindInstance(Agent pAgentInstance, string agentInstanceName)
        {
            if (string.IsNullOrEmpty(agentInstanceName))
            {
                agentInstanceName = pAgentInstance.GetType().FullName;
            }

            if (Agent.IsNameRegistered(agentInstanceName))
            {
                Debug.Check(GetInstance(agentInstanceName) == null, "the name has been bound to an instance already!");

                string className = Agent.GetRegisteredClassName(agentInstanceName);

                if (Agent.IsDerived(pAgentInstance, className))
                {
                    m_namedAgents[agentInstanceName] = pAgentInstance;

                    return true;
                }
            }
            else
            {
                Debug.Check(false);
            }

            return false;
        }

        public bool BindInstance(Agent pAgentInstance)
        {
            return BindInstance(pAgentInstance, null);
        }

        /**
        unbind 'agentInstanceName' from 'pAgentInstance'.
        'agentInstanceName' should have been bound to 'pAgentInstance'.

        @sa RegisterInstanceName, BindInstance, CreateInstance
        */

        public bool UnbindInstance(string agentInstanceName)
        {
            Debug.Check(!string.IsNullOrEmpty(agentInstanceName));

            if (Agent.IsNameRegistered(agentInstanceName))
            {
                if (m_namedAgents.ContainsKey(agentInstanceName))
                {
                    m_namedAgents.Remove(agentInstanceName);

                    return true;
                }
            }
            else
            {
                Debug.Check(false);
            }

            return false;
        }

        public bool UnbindInstance<T>()
        {
            string agentInstanceName = typeof(T).FullName;
            return UnbindInstance(agentInstanceName);
        }

        public Agent GetInstance(string agentInstanceName)
        {
            bool bValidName = !string.IsNullOrEmpty(agentInstanceName);

            if (bValidName)
            {
                string className = null;
                GetClassNameString(agentInstanceName, ref className);

                if (m_namedAgents.ContainsKey(className))
                {
                    Agent pA = m_namedAgents[className];

                    return pA;
                }

                return null;
            }

            return null;
        }

        public bool Save(Dictionary<string, Agent.State_t> states)
        {
            var e = m_static_variables.GetEnumerator();
            while (e.MoveNext())
            {
                string className = e.Current.Key;
                Variables variables = e.Current.Value;

                //states.insert(std::pair<const string, State_t>(className, State_t()));
                states[className] = new Agent.State_t();

                variables.CopyTo(null, states[className].Vars);
            }

            return true;
        }

        public bool Load(Dictionary<string, Agent.State_t> states)
        {
            var e = states.GetEnumerator();
            while (e.MoveNext())
            {
                if (m_static_variables.ContainsKey(e.Current.Key))
                {
                    Variables variables_f = m_static_variables[e.Current.Key];

                    e.Current.Value.Vars.CopyTo(null, variables_f);
                }
            }

            return true;
        }
    }
}
