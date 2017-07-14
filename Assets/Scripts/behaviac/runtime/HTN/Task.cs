using System.Collections.Generic;

namespace behaviac
{
    public class Task : BehaviorNode
    {
        public const string LOCAL_TASK_PARAM_PRE = "_$local_task_param_$_";

        protected bool m_bHTN;

        protected CTaskMethod m_task;

        public Task()
        {
        }

        ~Task()
        {
        }

        public bool IsHTN
        {
            get
            {
                return this.m_bHTN;
            }
        }

        public int FindMethodIndex(Method method)
        {
            for (int i = 0; i < this.GetChildrenCount(); ++i)
            {
                BehaviorNode child = this.GetChild(i);

                if (child == method)
                {
                    return i;
                }
            }

            return -1;
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is Task))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        protected override BehaviorTask createTask()
        {
            TaskTask pTask = new TaskTask();

            return pTask;
        }
        /// <summary>
        /// implement the decompose
        /// </summary>
        /// <param name="task"></param>
        /// <param name="seqTask"></param>
        /// <param name="depth"></param>
        /// <param name="planner"></param>
        /// <returns></returns>
        public override bool decompose(BehaviorNode node, PlannerTaskComplex seqTask, int depth, Planner planner)
        {
            bool bOk = false;
            Task task = (Task)node;
            PlannerTask childTask = planner.decomposeTask((Task)task, depth);

            if (childTask != null)
            {
                seqTask.AddChild(childTask);
                bOk = true;
            }

            return bOk;
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);

            for (int i = 0; i < properties.Count; ++i)
            {
                property_t p = properties[i];
                if (p.name == "Prototype")
                {
                    if (!string.IsNullOrEmpty(p.value))
                    {
                        CMethodBase m = Action.LoadMethod(p.value);
                        this.m_task = m as CTaskMethod;
                    }//if (p.value[0] != '\0')
                }
                else if (p.name == "IsHTN")
                {
                    if (p.value == "true")
                    {
                        this.m_bHTN = true;
                    }
                }
            }
        }
    }

    internal class TaskTask : Sequence.SequenceTask
    {
        private Planner _planner = new Planner();

        public TaskTask()
            : base()
        {
        }

        public override void copyto(BehaviorTask target)
        {
            base.copyto(target);
        }

        public override void Init(BehaviorNode node)
        {
            Debug.Check(node is Task, "node is not an Method");
            Task pTaskNode = (Task)(node);

            if (pTaskNode.IsHTN)
            {
                this.m_bIgnoreChildren = true;
            }

            base.Init(node);
        }

        public override void load(ISerializableNode node)
        {
            base.load(node);
        }

        public override void save(ISerializableNode node)
        {
            base.save(node);
        }

        protected override void addChild(BehaviorTask pBehavior)
        {
            base.addChild(pBehavior);
        }

        protected override bool onenter(Agent pAgent)
        {
            //reset the action child as it will be checked in the update
            this.m_activeChildIndex = CompositeTask.InvalidChildIndex;
            Debug.Check(this.m_activeChildIndex == CompositeTask.InvalidChildIndex);
            Task pMethodNode = (Task)(this.GetNode());

            _planner.Init(pAgent, pMethodNode);

            return base.onenter(pAgent);
        }

        protected override void onexit(Agent pAgent, EBTStatus s)
        {
            _planner.Uninit();
            base.onexit(pAgent, s);
        }

        protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
        {
            Debug.Check(this.GetNode() is Task, "node is not an Method");
            Task pTaskNode = (Task)(this.GetNode());

            if (pTaskNode.IsHTN)
            {
                EBTStatus status = _planner.Update();

                return status;
            }
            else
            {
                Debug.Check(this.m_children.Count == 1);
                BehaviorTask c = this.m_children[0];
                EBTStatus status = c.exec(pAgent);

                return status;
            }
        }
    }
}
