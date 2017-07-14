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
    public class AttachAction : BehaviorNode
    {
        public enum EOperatorType
        {
            E_INVALID,
            E_ASSIGN,        // =
            E_ADD,           // +
            E_SUB,           // -
            E_MUL,           // *
            E_DIV,           // /
            E_EQUAL,         // ==
            E_NOTEQUAL,      // !=
            E_GREATER,       // >
            E_LESS,          // <
            E_GREATEREQUAL,  // >=
            E_LESSEQUAL      // <=
        }

        public enum TransitionMode
        {
            Condition,
            Success,
            Failure,
            End
        }

        public class ActionConfig
        {
            public TransitionMode m_mode = TransitionMode.Condition;
            public Property m_opl;
            public CMethodBase m_opl_m;
            public Property m_opr1;
            public CMethodBase m_opr1_m;
            public EOperatorType m_operator = EOperatorType.E_INVALID;
            public Property m_opr2;
            public CMethodBase m_opr2_m;

            private VariableComparator m_comparator;

            protected ActionConfig()
        {
        }

        public virtual bool load(List<property_t> properties)
        {
            string opr2TypeName = null;
            string comparatorName = null;

            for (int i = 0; i < properties.Count; ++i)
            {
                property_t p = properties[i];
                if (p.name == "Mode")
                {
                    switch (p.value)
                    {
                        case "Condition":
                            this.m_mode = TransitionMode.Condition;
                            break;

                        case "Success":
                            this.m_mode = TransitionMode.Success;
                            break;

                        case "Failure":
                            this.m_mode = TransitionMode.Failure;
                            break;

                        case "End":
                            this.m_mode = TransitionMode.End;
                            break;
                    }
                }
                else if (p.name == "Opl")
                {
                    if (StringUtils.IsValidString(p.value))
                    {
                        int pParenthesis = p.value.IndexOf('(');

                        if (pParenthesis == -1)
                        {
                            string typeName = null;
                            this.m_opl = Condition.LoadRight(p.value, ref typeName);
                        }
                        else
                        {
                            //method
                            this.m_opl_m = Action.LoadMethod(p.value);
                        }
                    }
                }
                else if (p.name == "Opr1")
                {
                    if (StringUtils.IsValidString(p.value))
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
                }
                else if (p.name == "Operator")
                {
                    comparatorName = p.value;

                    switch (p.value)
                    {
                        case "Invalid":
                            this.m_operator = EOperatorType.E_INVALID;
                            break;

                        case "Assign":
                            this.m_operator = EOperatorType.E_ASSIGN;
                            break;

                        case "Add":
                            this.m_operator = EOperatorType.E_ADD;
                            break;

                        case "Sub":
                            this.m_operator = EOperatorType.E_SUB;
                            break;

                        case "Mul":
                            this.m_operator = EOperatorType.E_MUL;
                            break;

                        case "Div":
                            this.m_operator = EOperatorType.E_DIV;
                            break;

                        case "Equal":
                            this.m_operator = EOperatorType.E_EQUAL;
                            break;

                        case "NotEqual":
                            this.m_operator = EOperatorType.E_NOTEQUAL;
                            break;

                        case "Greater":
                            this.m_operator = EOperatorType.E_GREATER;
                            break;

                        case "Less":
                            this.m_operator = EOperatorType.E_LESS;
                            break;

                        case "GreaterEqual":
                            this.m_operator = EOperatorType.E_GREATEREQUAL;
                            break;

                        case "LessEqual":
                            this.m_operator = EOperatorType.E_LESSEQUAL;
                            break;
                    }
                }
                else if (p.name == "Opr2")
                {
                    if (StringUtils.IsValidString(p.value))
                    {
                        int pParenthesis = p.value.IndexOf('(');

                        if (pParenthesis == -1)
                        {
                            this.m_opr2 = Condition.LoadRight(p.value, ref opr2TypeName);
                        }
                        else
                        {
                            //method
                            this.m_opr2_m = Action.LoadMethod(p.value);
                        }
                    }
                }
                else
                {
                    //Debug.Check(0, "unrecognised property %s", p.name);
                }
            }

            // compare
            if (this.m_operator >= EOperatorType.E_EQUAL && this.m_operator <= EOperatorType.E_LESSEQUAL)
            {
                if (!string.IsNullOrEmpty(comparatorName) && (this.m_opl != null || this.m_opl_m != null) &&
                    (this.m_opr2 != null || this.m_opr2_m != null))
                {
                    this.m_comparator = Condition.Create(comparatorName, this.m_opl, this.m_opl_m, this.m_opr2, this.m_opr2_m);
                }
            }

            return this.m_opl != null;
        }

        public bool Execute(Agent pAgent)
        {
            bool bValid = false;

            // action
            if (this.m_opl_m != null && this.m_operator == EOperatorType.E_INVALID)
            {
                bValid = true;
                this.m_opl_m.Invoke(pAgent);
            }

            // assign
            else if (this.m_operator == EOperatorType.E_ASSIGN)
            {
                bValid = Assignment.EvaluteAssignment(pAgent, this.m_opl, this.m_opr2, this.m_opr2_m);
            }

            // compute
            else if (this.m_operator >= EOperatorType.E_ADD && this.m_operator <= EOperatorType.E_DIV)
            {
                EComputeOperator computeOperator = (EComputeOperator)(EComputeOperator.E_ADD + (this.m_operator - EOperatorType.E_ADD));
                bValid = Compute.EvaluteCompute(pAgent, this.m_opl, this.m_opr1, this.m_opr1_m, computeOperator, this.m_opr2, this.m_opr2_m);
            }

            // compare
            else if (this.m_operator >= EOperatorType.E_EQUAL && this.m_operator <= EOperatorType.E_LESSEQUAL)
            {
                if (this.m_comparator != null)
                {
                    bValid = this.m_comparator.Execute(pAgent);
                }
            }

            return bValid;
        }

        public bool Execute(Agent pAgent, EBTStatus methodResult)
        {
            if (this.m_mode == TransitionMode.Condition)
            {
                return this.Execute(pAgent);
            }
            else if (this.m_mode == TransitionMode.Success && methodResult == EBTStatus.BT_SUCCESS)
            {
                return true;
            }
            else if (this.m_mode == TransitionMode.Failure && methodResult == EBTStatus.BT_FAILURE)
            {
                return true;
            }
            else if (this.m_mode == TransitionMode.End && (methodResult == EBTStatus.BT_SUCCESS || methodResult == EBTStatus.BT_FAILURE))
            {
                return true;
            }

            return false;
        }
        }

        protected ActionConfig m_ActionConfig;

        public AttachAction()
        {
        }

        ~AttachAction()
        {
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);

            this.m_ActionConfig.load(properties);
        }

        public override bool Evaluate(Agent pAgent)
        {
            bool bValid = this.m_ActionConfig.Execute(pAgent);

            if (!bValid)
            {
                EBTStatus childStatus = EBTStatus.BT_INVALID;
                bValid = (EBTStatus.BT_SUCCESS == this.update_impl(pAgent, childStatus));
            }

            return bValid;
        }

        protected override BehaviorTask createTask()
        {
            Debug.Check(false);
            return null;
        }
    }
}
