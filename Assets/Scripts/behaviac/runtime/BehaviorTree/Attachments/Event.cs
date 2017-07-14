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
    public class Event : ConditionBase
    {
        public Event()
        {
            m_bTriggeredOnce = false;
            m_triggerMode = TriggerMode.TM_Transfer;
        }

        ~Event()
        {
            m_event = null;
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);

            for (int i = 0; i < properties.Count; ++i)
            {
                property_t p = properties[i];
                if (p.name == "Task")
                {
                    //method
                    this.m_event = Action.LoadMethod(p.value);
                }
                else if (p.name == "ReferenceFilename")
                {
                    this.m_referencedBehaviorPath = p.value;
                }
                else if (p.name == "TriggeredOnce")
                {
                    if (p.value == "true")
                    {
                        this.m_bTriggeredOnce = true;
                    }
                }
                else if (p.name == "TriggerMode")
                {
                    if (p.value == "Transfer")
                    {
                        this.m_triggerMode = TriggerMode.TM_Transfer;
                    }
                    else if (p.value == "Return")
                    {
                        this.m_triggerMode = TriggerMode.TM_Return;
                    }
                    else
                    {
                        Debug.Check(false, string.Format("unrecognised trigger mode {0}", p.value));
                    }
                }
                else
                {
                    //Debug.Check(0, "unrecognised property %s", p.name);
                }
            }
        }

        public string GetEventName()
        {
            if (this.m_event != null)
            {
                return this.m_event.Name;
            }

            return null;
        }

        public bool TriggeredOnce()
        {
            return this.m_bTriggeredOnce;
        }

        public TriggerMode GetTriggerMode()
        {
            return this.m_triggerMode;
        }

        public void switchTo(Agent pAgent)
        {
            if (!string.IsNullOrEmpty(this.m_referencedBehaviorPath))
            {
                if (pAgent != null)
                {
                    TriggerMode tm = this.GetTriggerMode();

                    pAgent.bteventtree(pAgent, this.m_referencedBehaviorPath, tm);
                    pAgent.btexec();
                }
            }
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is Event))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        protected override BehaviorTask createTask()
        {
            Debug.Check(false);
            return null;
        }

        protected CMethodBase m_event;

        protected string m_referencedBehaviorPath = null;

        protected TriggerMode m_triggerMode;

        //an event can be configured to stop being checked if triggered
        protected bool m_bTriggeredOnce;

        // ============================================================================
    }
}
