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
    public class Query : BehaviorNode
    {
        public Query()
        {
        }

        ~Query()
        {
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);

            if (properties.Count > 0)
            {
                for (int i = 0; i < properties.Count; ++i)
                {
                    property_t p = properties[i];
                    if (p.name == "Domain")
                    {
                        m_domain = p.value;
                    }
                    else if (p.name == "Descriptors")
                    {
                        SetDescriptors(p.value);
                    }
                    else
                    {
                        //Debug.Check(0, "unrecognised property %s", p.name);
                    }
                }
            }
        }

        public bool CheckReQuerying(Agent pAgent)
        {
            bool bTriggered = this.EvaluteCustomCondition(pAgent);

            return bTriggered;
        }

        protected override BehaviorTask createTask()
        {
            QueryTask pTask = new QueryTask();

            return pTask;
        }

        protected string m_domain;

        private class Descriptor_t
        {
            public Property Attribute;
            public Property Reference;
            public float Weight;

            public Descriptor_t()
            {
                Attribute = null;
                Reference = null;
                Weight = 0.0f;
            }

            public Descriptor_t(Descriptor_t copy)
            {
                Attribute = copy.Attribute;
                Reference = copy.Reference;
                Weight = copy.Weight;
            }

            ~Descriptor_t()
            {
                Attribute = null;
                Reference = null;
            }
        };

        private List<Descriptor_t> m_descriptors;

        private static Property FindProperty(Descriptor_t q, List<BehaviorTree.Descriptor_t> c)
        {
            //BehaviorTree.Descriptor_t descriptor = c.Find(delegate (BehaviorTree.Descriptor_t it) { return it.Descriptor.GetVariableId() == q.Attribute.GetVariableId(); });
            for (int i = 0; i < c.Count; ++i)
            {
                BehaviorTree.Descriptor_t descriptor = c[i];

                if (descriptor.Descriptor.VariableId == q.Attribute.VariableId)
                {
                    return descriptor.Reference;
                }
            }

            return null;
        }

        private List<Descriptor_t> GetDescriptors()
        {
            return this.m_descriptors;
        }

        protected void SetDescriptors(string descriptors)
        {
            this.m_descriptors = (List<Descriptor_t>)StringUtils.FromString(typeof(List<Descriptor_t>), descriptors, false);

            for (int i = 0; i < this.m_descriptors.Count; ++i)
            {
                Descriptor_t d = this.m_descriptors[i];
                d.Attribute.SetDefaultValue(d.Reference);
            }
        }

        private float ComputeSimilarity(List<Descriptor_t> q, List<BehaviorTree.Descriptor_t> c)
        {
            float similarity = 0.0f;

            for (int i = 0; i < q.Count; ++i)
            {
                Descriptor_t qi = q[i];

                Property ci = FindProperty(qi, c);

                if (ci != null)
                {
                    float range = qi.Attribute.GetRange();
                    float dp = qi.Reference.DifferencePercentage(ci, range);
                    Debug.Check(dp >= 0.0f && dp <= 1.0f, "dp should be normalized to [0, 1], please check its scale");

                    similarity += (1.0f - dp) * qi.Weight;
                }
            }

            return similarity;
        }

        private class QueryTask : SingeChildTask
        {
            private AgentState currentState;

            public QueryTask()
            {
            }

            public override void Init(BehaviorNode node)
            {
                base.Init(node);
            }

            ~QueryTask()
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
                //this.m_root = null;
                this.currentState = pAgent.Variables.Push(false);
                Debug.Check(currentState != null);

                if (this.ReQuery(pAgent))
                {
                    return true;
                }

                return false;
            }

            protected override void onexit(Agent pAgent, EBTStatus s)
            {
                this.currentState = pAgent.Variables.Push(false);
                Debug.Check(currentState != null);
                base.onexit(pAgent, s);
            }

            private bool ReQuery(Agent pAgent)
            {
                Query pQueryNode = this.GetNode() as Query;

                if (pQueryNode != null)
                {
                    List<Query.Descriptor_t> qd = pQueryNode.GetDescriptors();

                    if (qd.Count > 0)
                    {
                        Dictionary<string, BehaviorTree> bs = Workspace.Instance.GetBehaviorTrees();

                        BehaviorTree btFound = null;
                        float similarityMax = -1.0f;

                        var e = bs.Values.GetEnumerator();
                        while (e.MoveNext())
                        {
                            string domains = e.Current.GetDomains();

                            if (string.IsNullOrEmpty(pQueryNode.m_domain) || (!string.IsNullOrEmpty(domains) && domains.IndexOf(pQueryNode.m_domain) != -1))
                            {
                                List<BehaviorTree.Descriptor_t> bd = e.Current.GetDescriptors();
                                float similarity = pQueryNode.ComputeSimilarity(qd, bd);

                                if (similarity > similarityMax)
                                {
                                    similarityMax = similarity;
                                    btFound = e.Current;
                                }
                            }
                        }

                        if (btFound != null)
                        {
                            pAgent.btreferencetree(btFound.GetName());
                            //pAgent.btexec();

                            return true;
                        }
                    }
                }

                return false;
            }

            protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
            {
                Debug.Check(childStatus == EBTStatus.BT_RUNNING);

                Query node = this.m_node as Query;

                if (node.CheckReQuerying(pAgent))
                {
                    this.ReQuery(pAgent);
                }

                //EBTStatus status = base.update(pAgent, childStatus);

                //return status;
                return EBTStatus.BT_RUNNING;
            }
        }
    }
}
