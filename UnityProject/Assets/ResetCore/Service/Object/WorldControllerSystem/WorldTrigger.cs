using ResetCore.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ResetCore.WorldController
{
    public class WorldTrigger
    {
        /// <summary>
        /// 脚本
        /// </summary>
        public string script { get; private set; }

        /// <summary>
        /// 触发事件
        /// </summary>
        public List<BaseCommand> evnetList = new List<BaseCommand>();

        /// <summary>
        /// 条件
        /// </summary>
        public List<Func<bool>> conditionList = new List<Func<bool>>();

        /// <summary>
        /// 行为
        /// </summary>
        public List<Action> actionList = new List<Action>();
    }
}

