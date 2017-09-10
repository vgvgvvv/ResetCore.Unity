using UnityEngine;
using System.Collections;
using ResetCore.Util;

namespace ResetCore.GameSystem
{
    public class BaseSkill<T>
    {

        public class SkillArg
        {
            public Vector3 skillPoint;
            public GameObject targetObj;
        }

        private bool isColdingDown = false;
        private float coldDownTime;
        public SkillSystem<T> system { get; protected set; }

        public BaseSkill() { }
        public BaseSkill(SkillSystem<T> system)
        {
            this.system = system;
        }

        public virtual void Learn()
        {

        }

        public virtual void Use()
        {
            if (isColdingDown)
            {
                return;
            }
            isColdingDown = true;
            CoroutineTaskManager.Instance.WaitSecondTodo(() =>
            {
                isColdingDown = false;
            }, coldDownTime);
        }
    }

}
