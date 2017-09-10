using UnityEngine;
using System.Collections;


namespace ResetCore.BehaviorTree
{
    public abstract class CompositeNode : BaseBehaviorNode
    {

        public sealed override bool DoBehavior()
        {
            if (CheckConditionNode() == false)
            {
                return false;
            }
            else
            {
                return DoComposite();
            }
        }

        protected abstract bool DoComposite();

        protected bool CheckConditionNode()
        {
            foreach (BaseBehaviorNode node in childBehaviorList)
            {
                if (!(node is ActionNode) && node.DoBehavior() == false)
                {
                    return false;
                }
            }
            return true;
        }
    }
}

