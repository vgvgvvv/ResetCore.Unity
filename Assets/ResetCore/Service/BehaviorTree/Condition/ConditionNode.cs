using UnityEngine;
using System.Collections;

namespace ResetCore.BehaviorTree
{
    public abstract class ConditionNode : BaseBehaviorNode
    {

        public BaseBehaviorNode childNode { get { return childBehaviorList[0]; } }

        public sealed override bool DoBehavior()
        {
            return Handle();
        }

        protected abstract bool Handle();

        public override void AddChild(BaseBehaviorNode behavior)
        {
            if (childBehaviorList.Count == 0)
            {
                base.AddChild(behavior);
            }
            else
            {
                Debug.unityLogger.LogError("ConditionNode", "ConditionNode只允许有一个子节点");
            }

        }
    }

}
