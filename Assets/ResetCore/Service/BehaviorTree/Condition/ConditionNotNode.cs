using UnityEngine;
using System.Collections;

namespace ResetCore.BehaviorTree
{
    public class ConditionNotNode : ConditionNode
    {

        protected override bool Handle()
        {
            return !childNode.DoBehavior();
        }
    }

}
