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
    public class Condition : ConditionBase
    {
        public Condition()
        {
        }

        ~Condition()
        {
            m_opl = null;
            m_opr = null;
            m_opl_m = null;
            m_opr_m = null;
            m_comparator = null;
        }

        public static VariableComparator Create(string comparionOperator, Property lhs, CMethodBase lhs_m, Property rhs, CMethodBase rhs_m)
        {
            E_VariableComparisonType comparisonType = VariableComparator.ParseComparisonType(comparionOperator);

            VariableComparator pComparator = VariableComparator.Create(lhs, lhs_m, rhs, rhs_m);
            pComparator.SetComparisonType(comparisonType);

            return pComparator;
        }

        public static Property LoadLeft(string value)
        {
            Property opl = null;

            if (!string.IsNullOrEmpty(value))
            {
                string typeName = null;
                opl = ParseProperty(value, ref typeName);
            }

            return opl;
        }

        public static Property LoadRight(string value, ref string typeName)
        {
            Property opr = null;

            if (!string.IsNullOrEmpty(value))
            {
                if (value.StartsWith("const"))
                {
                    //const Int32 0
                    const int kConstLength = 5;
                    string strRemaining = value.Substring(kConstLength + 1);
                    int p = StringUtils.FirstToken(strRemaining, ' ', ref typeName);

                    typeName = typeName.Replace("::", ".");

                    string strVale = strRemaining.Substring(p + 1);
                    opr = Property.Create(typeName, strVale);
                }
                else
                {
                    opr = ParseProperty(value, ref typeName);
                }
            }

            return opr;
        }

        public static Property ParseProperty(string value, ref string typeName)
        {
            try
            {
                Property opr = null;
                List<string> tokens = StringUtils.SplitTokens(value);

                if (tokens[0] == "static")
                {
                    //static int Property1
                    typeName = tokens[1].Replace("::", ".");

                    if (tokens.Count == 3)
                    {
                        opr = Property.Create(typeName, tokens[2], true, null);
                    }
                    else
                    {
                        Debug.Check(tokens.Count == 4);
                        opr = Property.Create(typeName, tokens[2], true, tokens[3]);
                    }
                }
                else
                {
                    //int Property1
                    typeName = tokens[0].Replace("::", ".");

                    if (tokens.Count == 2)
                    {
                        opr = Property.Create(typeName, tokens[1], false, null);
                    }
                    else
                    {
                        opr = Property.Create(typeName, tokens[1], false, tokens[2]);
                    }
                }

                return opr;
            }
            catch (System.Exception e)
            {
                Debug.Check(false, e.Message);
            }

            return null;
        }

        public static Property LoadProperty(string value)
        {
            string typeName = null;
            return LoadRight(value, ref typeName);
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);

            string typeName = null;
            string comparatorName = null;

            for (int i = 0; i < properties.Count; ++i)
            {
                property_t p = properties[i];
                if (p.name == "Operator")
                {
                    comparatorName = p.value;
                }
                else if (p.name == "Opl")
                {
                    int pParenthesis = p.value.IndexOf('(');

                    if (pParenthesis == -1)
                    {
                        this.m_opl = LoadLeft(p.value);
                    }
                    else
                    {
                        this.m_opl_m = Action.LoadMethod(p.value);
                    }
                }
                else if (p.name == "Opr")
                {
                    int pParenthesis = p.value.IndexOf('(');

                    if (pParenthesis == -1)
                    {
                        this.m_opr = LoadRight(p.value, ref typeName);
                    }
                    else
                    {
                        this.m_opr_m = Action.LoadMethod(p.value);
                    }
                }
                else
                {
                    //Debug.Check(0, "unrecognised property %s", p.name);
                }
            }

            if (!string.IsNullOrEmpty(comparatorName) && (this.m_opl != null || this.m_opl_m != null) && (this.m_opr != null || this.m_opr_m != null))
            {
                this.m_comparator = Condition.Create(comparatorName, this.m_opl, this.m_opl_m, this.m_opr, this.m_opr_m);
            }
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is Condition))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        public override bool Evaluate(Agent pAgent)
        {
            if (this.m_comparator != null)
            {
                return this.m_comparator.Execute(pAgent);
            }
            else
            {
                EBTStatus childStatus = EBTStatus.BT_INVALID;
                EBTStatus result = this.update_impl(pAgent, childStatus);
                return result == EBTStatus.BT_SUCCESS;
            }
        }

        protected override BehaviorTask createTask()
        {
            ConditionTask pTask = new ConditionTask();

            return pTask;
        }

        protected Property m_opl;
        private Property m_opr;
        private CMethodBase m_opl_m;
        private CMethodBase m_opr_m;
        private VariableComparator m_comparator;

        private class ConditionTask : ConditionBaseTask
        {
            public ConditionTask()
            {
            }

            ~ConditionTask()
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

                Debug.Check(this.GetNode() is Condition);
                Condition pConditionNode = (Condition)(this.GetNode());

                bool ret = pConditionNode.Evaluate(pAgent);

                return ret ? EBTStatus.BT_SUCCESS : EBTStatus.BT_FAILURE;
            }
        }
    }
}
