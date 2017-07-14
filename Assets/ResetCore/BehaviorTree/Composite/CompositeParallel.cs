using UnityEngine;
using System.Collections;

namespace ResetCore.BehaviorTree
{
    public class CompositeParallel : CompositeNode
    {
        protected override bool DoComposite()
        {

            foreach (BaseBehaviorNode node in childBehaviorList)
            {
                if (node is ActionNode)
                {
                    node.DoBehavior();
                }
            }

            return true;
        }
    }

}
