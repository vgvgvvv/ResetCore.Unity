using UnityEngine;
using System.Collections;


namespace ResetCore.BehaviorTree
{
    public class CompositeSequence : CompositeNode
    {


        protected override bool DoComposite()
        {
            //遇到一个false就立马返回
            foreach (BaseBehaviorNode node in childBehaviorList)
            {
                if (node is ActionNode && node.DoBehavior() == false)
                {
                    break;
                }
            }
            return true;
        }
    }

}
