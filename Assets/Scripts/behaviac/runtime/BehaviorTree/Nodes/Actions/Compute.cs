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
    public enum EComputeOperator
    {
        E_INVALID,
        E_ADD,
        E_SUB,
        E_MUL,
        E_DIV
    }

public class Compute : BehaviorNode
    {
        public Compute()
    {
    }

    ~Compute()
    {
        m_opl = null;
        m_opr1 = null;
        m_opr1_m = null;
        m_opr2 = null;
        m_opr2_m = null;
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
            else if (p.name == "Operator")
            {
                if (p.value == "Add")
                {
                    this.m_operator = EComputeOperator.E_ADD;
                }
                else if (p.value == "Sub")
                {
                    this.m_operator = EComputeOperator.E_SUB;
                }
                else if (p.value == "Mul")
                {
                    this.m_operator = EComputeOperator.E_MUL;
                }
                else if (p.value == "Div")
                {
                    this.m_operator = EComputeOperator.E_DIV;
                }
                else
                {
                    Debug.Check(false);
                }
            }
            else if (p.name == "Opr1")
            {
                int pParenthesis = p.value.IndexOf('(');

                if (pParenthesis == -1)
                {
                    string typeName = null;
                    this.m_opr1 = Condition.LoadRight(p.value, ref typeName);
                }
                else
                {
                    //method
                    this.m_opr1_m = Action.LoadMethod(p.value);
                }
            }
            else if (p.name == "Opr2")
            {
                int pParenthesis = p.value.IndexOf('(');

                if (pParenthesis == -1)
                {
                    string typeName = null;
                    this.m_opr2 = Condition.LoadRight(p.value, ref typeName);
                }
                else
                {
                    //method
                    this.m_opr2_m = Action.LoadMethod(p.value);
                }
            }
            else
            {
                //Debug.Check(0, "unrecognised property %s", p.name);
            }
        }
    }

    public static bool EvaluteCompute(Agent pAgent, Property opl, Property opr1, CMethodBase opr1_m, EComputeOperator opr, Property opr2, CMethodBase opr2_m)
    {
        bool bValid = false;
        object value1 = null;

        if (opl != null)
        {
            if (opr1_m != null)
            {
                bValid = true;
                value1 = opr1_m.Invoke(pAgent);
            }
            else if (opr1 != null)
            {
                bValid = true;
                Agent pParentR = opr1.GetParentAgent(pAgent);

                value1 = opr1.GetValue(pParentR);
            }

            if (opr2_m != null)
            {
                bValid = true;
                object value2 = opr2_m.Invoke(pAgent);

                Agent pParentOpl = opl.GetParentAgent(pAgent);
                object returnValue = Details.ComputeValue(value1, value2, opr);

                opl.SetValue(pParentOpl, returnValue);
            }
            else if (opr2 != null)
            {
                bValid = true;
                Agent pParentL = opl.GetParentAgent(pAgent);
                Agent pParentR = opr2.GetParentAgent(pAgent);

                object value2 = opr2.GetValue(pParentR);

                object returnValue = Details.ComputeValue(value1, value2, opr);

                opl.SetValue(pParentL, returnValue);
            }
        }

        return bValid;
    }

    public override bool IsValid(Agent pAgent, BehaviorTask pTask)
    {
        if (!(pTask.GetNode() is Compute))
        {
            return false;
        }

        return base.IsValid(pAgent, pTask);
    }

    protected override BehaviorTask createTask()
    {
        return new ComputeTask();
    }

    protected Property m_opl;
    protected Property m_opr1;
    protected CMethodBase m_opr1_m;
    protected Property m_opr2;
    protected CMethodBase m_opr2_m;
    protected EComputeOperator m_operator = EComputeOperator.E_INVALID;

private class ComputeTask : LeafTask
    {
        public ComputeTask()
    {
    }

    ~ComputeTask()
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

        EBTStatus result = EBTStatus.BT_SUCCESS;

        Debug.Check(this.GetNode() is Compute);
        Compute pComputeNode = (Compute)(this.GetNode());

        bool bValid = Compute.EvaluteCompute(pAgent, pComputeNode.m_opl, pComputeNode.m_opr1, pComputeNode.m_opr1_m,
                                             pComputeNode.m_operator, pComputeNode.m_opr2, pComputeNode.m_opr2_m);

        if (!bValid)
        {
            result = pComputeNode.update_impl(pAgent, childStatus);
        }

        return result;
    }
    }
    }
}
