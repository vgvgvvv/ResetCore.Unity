using UnityEngine;
using System.Collections;
using ResetCore.Util;


namespace ResetCore.BehaviorTree
{
    public enum RunStatus
    {
        Completed,
        Failure,
        Running,
    }


    public abstract class ActionNode : BaseBehaviorNode
    {
        public RunStatus runState { get; protected set; }
        public CoroutineTaskManager.CoroutineTask task;

        protected abstract IEnumerator DoAction(out RunStatus status);
        public abstract bool CanDoBehavior();

        public sealed override bool DoBehavior()
        {
            bool flag = CanDoBehavior();
            if (flag)
            {
                root.currentRunningNode = this;

                runState = RunStatus.Running;

                //加入行为队列，如果完成工作就进入下一个节点
                actionQueue.AddAction((act) =>
                {
                    task = new CoroutineTaskManager.CoroutineTask(root.GetHashCode() + GetType().Name,
                        DoActionWithChangeState(),
                        (comp) =>
                        {
                            if (comp)
                            {
                                if (act != null)
                                {
                                    act();
                                }
                                else
                                {
                                    root.Tick();
                                }

                            }

                        });
                    CoroutineTaskManager.Instance.AddTask(task);
                });

                return true;
            }
            else
            {
                return false;
            }
        }

        public sealed override void AddChild(BaseBehaviorNode behavior)
        {
            Debug.unityLogger.LogError("添加子节点", "行为节点不能有子节点");
        }

        public sealed override void DeleteChild(BaseBehaviorNode behavior)
        {
            Debug.unityLogger.LogError("添加子节点", "行为节点不能有子节点");
        }

        protected IEnumerator DoActionWithChangeState()
        {
            RunStatus statu;
            yield return DoAction(out statu);
            runState = statu;
        }

        public void StopBehavior()
        {
            if (task != null)
            {
                task.Stop();
            }
        }
    }

}
