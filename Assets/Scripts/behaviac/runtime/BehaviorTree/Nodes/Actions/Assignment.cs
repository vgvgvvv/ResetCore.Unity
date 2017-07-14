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
    public class Assignment : BehaviorNode
    {
        public Assignment()
        {
        }

        ~Assignment()
        {
            m_opl = null;
            m_opr = null;
            m_opr_m = null;
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);

            for (int i = 0; i < properties.Count; ++i)
            {
                property_t p = properties[i];
                if (p.name == "Opl")
                {
                    this.m_opl = Condition.LoadLeft(p.value);
                }
                else if (p.name == "Opr")
                {
                    int pParenthesis = p.value.IndexOf('(');

                    if (pParenthesis == -1)
                    {
                        string typeName = null;
                        this.m_opr = Condition.LoadRight(p.value, ref typeName);
                    }
                    else
                    {
                        //method
                        this.m_opr_m = Action.LoadMethod(p.value);
                    }
                }
                else
                {
                    //Debug.Check(0, "unrecognised property %s", p.name);
                }
            }
        }

        public static bool EvaluteAssignment(Agent pAgent, Property opl, Property opr, behaviac.CMethodBase opr_m)
        {
            bool bValid = false;

            if (opl != null)
            {
                if (opr_m != null)
                {
                    object returnValue = opr_m.Invoke(pAgent);

                    Agent pParentOpl = opl.GetParentAgent(pAgent);
                    opl.SetValue(pParentOpl, returnValue);

                    bValid = true;
                }
                else if (opr != null)
                {
                    Agent pParentL = opl.GetParentAgent(pAgent);
                    Agent pParentR = opr.GetParentAgent(pAgent);

                    opl.SetFrom(pParentR, opr, pParentL);

                    bValid = true;
                }
            }

            return bValid;
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is Assignment))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        protected override BehaviorTask createTask()
        {
            return new AssignmentTask();
        }

        protected Property m_opl;
        protected Property m_opr;
        protected CMethodBase m_opr_m;

        private class AssignmentTask : LeafTask
        {
            public AssignmentTask()
            { }

            ~AssignmentTask()
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

                Debug.Check(this.GetNode() is Assignment);
                Assignment pAssignmentNode = (Assignment)(this.GetNode());

                EBTStatus result = EBTStatus.BT_SUCCESS;
                bool bValid = Assignment.EvaluteAssignment(pAgent, pAssignmentNode.m_opl, pAssignmentNode.m_opr, pAssignmentNode.m_opr_m);

                if (!bValid)
                {
                    result = pAssignmentNode.update_impl(pAgent, childStatus);
                }

                return result;
            }
        }
    }
}
