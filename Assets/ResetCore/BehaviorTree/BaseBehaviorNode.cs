using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using ResetCore.Util;

namespace ResetCore.BehaviorTree
{
    public abstract class BaseBehaviorNode
    {


        protected BaseBehaviorNode parent;
        protected List<BaseBehaviorNode> childBehaviorList = new List<BaseBehaviorNode>();

        protected ActionQueue actionQueue { get { return root.actionQueue; } }

        public BehaviorRoot root { get; set; }


        public virtual void AddChild(BaseBehaviorNode behavior)
        {
            behavior.parent = this;
            childBehaviorList.Add(behavior);
            behavior.root = root;
        }

        public virtual void DeleteChild(BaseBehaviorNode behavior)
        {
            behavior.parent = null;
            childBehaviorList.Remove(behavior);
        }

        public abstract bool DoBehavior();

        public static BaseBehaviorNode Getbehavior(string behaviorName)
        {
            Type rootBehaviorType = Type.GetType(behaviorName);

            ConstructorInfo constructor = rootBehaviorType.GetConstructor(new Type[] { });
            BaseBehaviorNode finalBehavior = constructor.Invoke(new object[] { }) as BaseBehaviorNode;
            return finalBehavior;
        }

    }

}
