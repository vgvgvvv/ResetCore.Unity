using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ResetCore.GameSystem
{
    public class SkillSystem<T>
    {

        public HashSet<BaseSkill<T>> skillSet { get; protected set; }
        public T skillObject { get; protected set; }

        public static SkillSystem<T> Create(T skillObject)
        {
            return new SkillSystem<T>(skillObject);
        }
        private SkillSystem() { }
        private SkillSystem(T skillObject)
        {
            this.skillObject = skillObject;
            skillSet = new HashSet<BaseSkill<T>>();
        }

        public void LearnSkill(BaseSkill<T> skill)
        {
            skillSet.Add(skill);
            skill.Learn();
        }

        public void FindAndUse<U>()
        {
            foreach (BaseSkill<T> skill in skillSet)
            {
                if (skill is U)
                {
                    skill.Use();
                    return;
                }
            }
            Debug.unityLogger.LogError("使用技能", skillObject.ToString() + "不存在该技能" + typeof(U).Name);
        }
    }

}
