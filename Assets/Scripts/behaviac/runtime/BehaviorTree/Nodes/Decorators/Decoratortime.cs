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
    public class DecoratorTime : DecoratorNode
    {
        private Property m_time_var;
        protected CMethodBase m_time_m;

        public DecoratorTime()
        {
            this.m_time_var = null;
            this.m_time_m = null;
        }

        ~DecoratorTime()
        {
            this.m_time_var = null;
            this.m_time_m = null;
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);

            for (int i = 0; i < properties.Count; ++i)
            {
                property_t p = properties[i];
                if (p.name == "Time")
                {
                    int pParenthesis = p.value.IndexOf('(');

                    if (pParenthesis == -1)
                    {
                        string typeName = null;
                        this.m_time_var = Condition.LoadRight(p.value, ref typeName);
                    }
                    else
                    {
                        //method
                        this.m_time_m = Action.LoadMethod(p.value);
                    }
                }
            }
        }

        protected virtual double GetTime(Agent pAgent)
        {
            object timeObj = null;

            if (this.m_time_var != null)
            {
                timeObj = this.m_time_var.GetValue(pAgent);
            }
            else
            {
                Debug.Check(this.m_time_m != null);
                if (this.m_time_m != null)
                {
                    timeObj = this.m_time_m.Invoke(pAgent);
                }
            }

            if (timeObj != null)
            {
                return Convert.ToDouble(timeObj);
            }

            return 0;
        }

        protected override BehaviorTask createTask()
        {
            DecoratorTimeTask pTask = new DecoratorTimeTask();

            return pTask;
        }

        private class DecoratorTimeTask : DecoratorTask
        {
            public DecoratorTimeTask()
            {
            }

            ~DecoratorTimeTask()
            {
            }

            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);

                Debug.Check(target is DecoratorTimeTask);
                DecoratorTimeTask ttask = (DecoratorTimeTask)target;

                ttask.m_start = this.m_start;
                ttask.m_time = this.m_time;
            }

            public override void save(ISerializableNode node)
            {
                base.save(node);

                CSerializationID startId = new CSerializationID("start");
                node.setAttr(startId, this.m_start);

                CSerializationID timeId = new CSerializationID("time");
                node.setAttr(timeId, this.m_time);
            }

            public override void load(ISerializableNode node)
            {
                base.load(node);
            }

            protected override bool onenter(Agent pAgent)
            {
                base.onenter(pAgent);

                this.m_start = Workspace.Instance.TimeSinceStartup * 1000.0;
                this.m_time = this.GetTime(pAgent);

                return (this.m_time >= 0);
            }

            protected override EBTStatus decorate(EBTStatus status)
            {
                if (Workspace.Instance.TimeSinceStartup * 1000.0 - this.m_start >= this.m_time)
                {
                    return EBTStatus.BT_SUCCESS;
                }

                return EBTStatus.BT_RUNNING;
            }

            private double GetTime(Agent pAgent)
            {
                Debug.Check(this.GetNode() is DecoratorTime);
                DecoratorTime pNode = (DecoratorTime)(this.GetNode());

                return pNode != null ? pNode.GetTime(pAgent) : 0;
            }

            private double m_start;
            private double m_time;
        }
    }
}
