#define BEHAVIAC_ENABLE_PUSH_OPT

using System;
using System.Collections.Generic;

namespace behaviac
{
    public class AgentState : Variables, IDisposable
    {
        private List<AgentState> state_stack = null;

        public AgentState()
        {
        }

        public AgentState(AgentState parent)
        {
            this.parent = parent;
        }

        public void Dispose()
        {
            this.Pop();
        }

        private static Stack<AgentState> pool = new Stack<AgentState>();

        private AgentState parent = null;

#if BEHAVIAC_ENABLE_PUSH_OPT
        private bool m_forced;
        private int m_pushed;
#endif

        public int Depth
        {
            get
            {
                int d = 1;

                if (this.state_stack != null && this.state_stack.Count > 0)
                {
                    for (int i = this.state_stack.Count - 1; i >= 0; --i)
                    {
                        AgentState t = this.state_stack[i];
                        d += 1 + t.m_pushed;
                    }
                }

                return d;
            }
        }

        public int Top
        {
            get
            {
                if (this.state_stack != null && this.state_stack.Count > 0)
                {
                    return this.state_stack.Count - 1;
                }

                return -1;
            }
        }

        public AgentState Push(bool bForcePush)
        {
#if BEHAVIAC_ENABLE_PUSH_OPT

            if (!bForcePush)
            {
                //if the top has nothing new added, to use it again
                if (this.state_stack != null && this.state_stack.Count > 0)
                {
                    AgentState t = this.state_stack[this.state_stack.Count - 1];

                    if (!t.m_forced && t.m_variables.Count == 0)
                    {
                        t.m_pushed++;
                        return t;
                    }
                }
            }

#endif

            AgentState newly = null;
            lock(pool)
            {
                if (pool.Count > 0)
                {
                    newly = pool.Pop();
                    //set the parent
                    newly.parent = this;
                }
                else
                {
                    newly = new AgentState(this);
                }

                newly.m_forced = bForcePush;

                if (bForcePush)
                {
                    base.CopyTo(null, newly);
                }
            }

            if (this.state_stack == null)
            {
                this.state_stack = new List<AgentState>();
            }

            //add the newly one at the end of the list as the top
            this.state_stack.Add(newly);

            return newly;
        }

        public void Pop()
        {
#if BEHAVIAC_ENABLE_PUSH_OPT

            if (this.m_pushed > 0)
            {
                this.m_pushed--;

                if (this.m_variables.Count > 0)
                {
                    this.m_variables.Clear();
                    return;
                }

                return;
            }

#endif

            if (this.state_stack != null && this.state_stack.Count > 0)
            {
                AgentState top = this.state_stack[this.state_stack.Count - 1];
                top.Pop();
                return;
            }

            this.Clear();
            Debug.Check(this.state_stack == null);
            Debug.Check(this.parent != null);

            this.parent.PopTop();
            this.parent = null;

            lock(pool)
            {
                Debug.Check(!pool.Contains(this));
                pool.Push(this);
            }
        }

        private void PopTop()
        {
            Debug.Check(this.state_stack != null);
            Debug.Check(this.state_stack.Count > 0);
            //remove the last one
            this.state_stack.RemoveAt(this.state_stack.Count - 1);
        }

        public override void Log(Agent pAgent, bool bForce)
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsLoggingOrSocketing)
            {
                if (this.state_stack != null && this.state_stack.Count > 0)
                {
                    Dictionary<string, bool> logged = new Dictionary<string, bool>();

                    for (int i = this.state_stack.Count - 1; i >= 0; --i)
                    {
                        AgentState t = this.state_stack[i];

                        var e = t.Vars.Values.GetEnumerator();
                        while (e.MoveNext())
                        {
                            if (bForce || e.Current.IsChanged())
                            {
                                if (!logged.ContainsKey(e.Current.Name))
                                {
                                    e.Current.Log(pAgent);
                                    logged.Add(e.Current.Name, true);
                                }
                            }
                        }
                    }//end of for
                }
                else
                {
                    base.Log(pAgent, bForce);
                }
            }

#endif
        }
        public override void SetObject(bool bMemberSet, Agent pAgent, bool bLocal, CMemberBase pMember, string variableName, object value, uint varId)
        {
            //if (variableName == "DirtyRooms")
            //{
            //    Debug.Check(true);
            //}

            // not in planning
            if (pAgent.PlanningTop == -1 && !bLocal)
            {
                base.SetObject(bMemberSet, pAgent, bLocal, pMember, variableName, value, varId);
                return;
            }

            if (this.state_stack != null && this.state_stack.Count > 0)
            {
                int stackIndex = 0;

                if (bLocal)
                {
                    //top
                    stackIndex = this.state_stack.Count - 1;
                }
                else
                {
                    //bottom
                    stackIndex = pAgent.PlanningTop;
                }

                AgentState t = this.state_stack[stackIndex];

                //if there are something in the state stack, it is used for planning, so, don't really set member
                t.SetObject(false, pAgent, bLocal, null, variableName, value, varId);
            }
            else
            {
                base.SetObject(bMemberSet, pAgent, bLocal, pMember, variableName, value, varId);
            }
        }

        public override object GetObject(Agent pAgent, bool bMemberGet, CMemberBase pMember, uint varId)
        {
            if (this.state_stack != null && this.state_stack.Count > 0)
            {
                for (int i = this.state_stack.Count - 1; i >= 0; --i)
                {
                    AgentState t = this.state_stack[i];
                    object result = t.GetObject(pAgent, false, pMember, varId);

                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            object result1 = base.GetObject(pAgent, bMemberGet, pMember, varId);

            return result1;
        }
    }
}
