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

using System.Collections.Generic;

namespace behaviac
{
    // ============================================================================
    public class Action : BehaviorNode
    {
        public Action()
        {
            m_resultOption = EBTStatus.BT_INVALID;
        }

        ~Action()
        {
            m_method = null;
            m_resultFunctor = null;
        }

        private static string ParseInstanceName(string fullName, ref string instanceName)
        {
            //Self.AgentActionTest::Action2(0)
            int pClassBegin = fullName.IndexOf('.');
            Debug.Check(pClassBegin != -1);

            instanceName = fullName.Substring(0, pClassBegin);

            string propertyName = fullName.Substring(pClassBegin + 1);
            return propertyName;
        }

        public static int ParseMethodNames(string fullName, ref string agentIntanceName, ref string agentClassName, ref string methodName)
        {
            //Self.test_ns::AgentActionTest::Action2(0)
            int pClassBegin = fullName.IndexOf('.');
            Debug.Check(pClassBegin != -1);

            agentIntanceName = fullName.Substring(0, pClassBegin);

            int pBeginAgentClass = pClassBegin + 1;

            int pBeginP = fullName.IndexOf('(', pBeginAgentClass);
            Debug.Check(pBeginP != -1);

            //test_ns::AgentActionTest::Action2(0)
            int pBeginMethod = fullName.LastIndexOf(':', pBeginP);
            Debug.Check(pBeginMethod != -1);
            //skip '::'
            Debug.Check(fullName[pBeginMethod] == ':' && fullName[pBeginMethod - 1] == ':');
            pBeginMethod += 1;

            int pos1 = pBeginP - pBeginMethod;

            methodName = fullName.Substring(pBeginMethod, pos1);

            int pos = pBeginMethod - 2 - pBeginAgentClass;

            agentClassName = fullName.Substring(pBeginAgentClass, pos).Replace("::", ".");

            return pBeginP;
        }

        //suppose params are seprated by ','
        public static List<string> ParseForParams(string tsrc)
        {
            int tsrcLen = tsrc.Length;
            int startIndex = 0;
            int index = 0;
            int quoteDepth = 0;

            List<string> params_ = new List<string>();

            for (; index < tsrcLen; ++index)
            {
                if (tsrc[index] == '"')
                {
                    quoteDepth++;

                    //if (quoteDepth == 1)
                    //{
                    //	startIndex = index;
                    //}

                    if ((quoteDepth & 0x1) == 0)
                    {
                        //closing quote
                        quoteDepth -= 2;
                        Debug.Check(quoteDepth >= 0);
                    }
                }
                else if (quoteDepth == 0 && tsrc[index] == ',')
                {
                    //skip ',' inside quotes, like "count, count"
                    int lengthTemp = index - startIndex;
                    string strTemp = tsrc.Substring(startIndex, lengthTemp);
                    params_.Add(strTemp);
                    startIndex = index + 1;
                }
            }//end for

            // the last param
            int lengthTemp0 = index - startIndex;

            if (lengthTemp0 > 0)
            {
                string strTemp = tsrc.Substring(startIndex, lengthTemp0);
                params_.Add(strTemp);

                //params_.Add(strTemp);
            }

            return params_;
        }

        public static CMethodBase LoadMethod(string value_)
        {
            //Self.test_ns::AgentActionTest::Action2(0)
            if (string.IsNullOrEmpty(value_) || (value_[0] == '\"' && value_[1] == '\"'))
            {
                //empty value
                return null;
            }

            string agentIntanceName = null;
            string agentClassName = null;
            string methodName = null;
            int pBeginP = ParseMethodNames(value_, ref agentIntanceName, ref agentClassName, ref methodName);

            //propertyName = FormatString("%s::%s", agentClassName, methodName);
            CStringID agentClassId = new CStringID(agentClassName);
            CStringID methodId = new CStringID(methodName);

            CMethodBase method = Agent.CreateMethod(agentClassId, methodId);

            if (method == null)
            {
                behaviac.Debug.LogWarning(string.Format("No Method {0}::{1} registered\n", agentClassName, methodName));
                Debug.Check(false, string.Format("No Method {0}::{1} registered\n", agentClassName, methodName));
            }
            else
            {
                method.InstanceName = agentIntanceName;

                Debug.Check(method != null, string.Format("No Method {0}::{1} registered", agentClassName, methodName));
                string params_ = value_.Substring(pBeginP);

                Debug.Check(params_[0] == '(');

                List<string> paramsTokens = null;

                {
                    int len = params_.Length;

                    Debug.Check(params_[len - 1] == ')');

                    string text = params_.Substring(1, len - 2);
                    //StringUtils::SplitIntoArray(text, ",", tokens);
                    paramsTokens = ParseForParams(text);
                }

                if (paramsTokens != null)
                {
                    method.Load(paramsTokens);
                }
            }

            return method;
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);

            for (int i = 0; i < properties.Count; ++i)
            {
                property_t p = properties[i];
                if (p.name == "Method")
                {
                    if (!string.IsNullOrEmpty(p.value))
                    {
                        this.m_method = Action.LoadMethod(p.value);
                    }//if (p.value[0] != '\0')
                }
                else if (p.name == "ResultOption")
                {
                    if (p.value == "BT_INVALID")
                    {
                        m_resultOption = EBTStatus.BT_INVALID;
                    }
                    else if (p.value == "BT_FAILURE")
                    {
                        m_resultOption = EBTStatus.BT_FAILURE;
                    }
                    else if (p.value == "BT_RUNNING")
                    {
                        m_resultOption = EBTStatus.BT_RUNNING;
                    }
                    else
                    {
                        m_resultOption = EBTStatus.BT_SUCCESS;
                    }
                }
                else if (p.name == "ResultFunctor")
                {
                    if (p.value[0] != '\0')
                    {
                        this.m_resultFunctor = Action.LoadMethod(p.value);
                    }
                }
                else
                {
                    //Debug.Check(0, "unrecognised property %s", p.name);
                }
            }
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is Action))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        public EBTStatus Execute(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus result = EBTStatus.BT_SUCCESS;

            if (this.m_method != null)
            {
                object returnValue = this.m_method.Invoke(pAgent);

                if (this.m_resultOption != EBTStatus.BT_INVALID)
                {
                    result = this.m_resultOption;
                }
                else if (this.m_resultFunctor != null)
                {
                    result = (EBTStatus)this.m_resultFunctor.Invoke(pAgent, returnValue);
                }
                else
                {
                    Debug.Check(returnValue is EBTStatus, "method's return type is not EBTStatus");
                    result = (EBTStatus)returnValue;
                }
            }
            else
            {
                result = this.update_impl(pAgent, childStatus);
            }

            return result;
        }

        protected override BehaviorTask createTask()
        {
            ActionTask pTask = new ActionTask();

            return pTask;
        }

        protected CMethodBase m_method;

        public EBTStatus Execute(Agent pAgent, object[] paramsValue)
        {
            EBTStatus result = EBTStatus.BT_SUCCESS;

            if (this.m_method != null)
            {
                object returnValue = this.m_method.Invoke(pAgent, paramsValue);

                if (this.m_resultOption != EBTStatus.BT_INVALID)
                {
                    result = this.m_resultOption;
                }
                else if (this.m_resultFunctor != null)
                {
                    result = (EBTStatus)this.m_resultFunctor.Invoke(pAgent, returnValue);
                }
                else
                {
                    Debug.Check(returnValue is EBTStatus, "method's return type is not EBTStatus");
                    result = (EBTStatus)returnValue;
                }
            }

            return result;
        }

        protected EBTStatus m_resultOption;
        private CMethodBase m_resultFunctor;

        private class ActionTask : LeafTask
        {
            public ActionTask()
            {
            }

            ~ActionTask()
            {
            }

            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);
            }

            public override void save(ISerializableNode node)
            {
                base.save(node);
            }

            public override void load(ISerializableNode node)
            {
                base.load(node);
            }

            protected override bool onenter(Agent pAgent)
            {
                return true;
            }

            protected override void onexit(Agent pAgent, EBTStatus s)
            {
            }

            protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
            {
                Debug.Check(childStatus == EBTStatus.BT_RUNNING);

                Debug.Check(this.GetNode() is Action, "node is not an Action");
                Action pActionNode = (Action)(this.GetNode());

                EBTStatus result = pActionNode.Execute(pAgent, childStatus);

                return result;
            }
        }
    }
}
