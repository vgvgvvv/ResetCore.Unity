using UnityEngine;
using System.Collections;


namespace ResetCore.BehaviorTree
{
    public class CompositeSelector : CompositeNode
    {

        protected override bool DoComposite()
        {
            //遇到第一个对的就立马返回
            foreach (BaseBehaviorNode node in childBehaviorList)
            {
                if (node is ActionNode && node.DoBehavior() == true)
                {
                    break;
                }
            }
            return true;
        }

    }

}
