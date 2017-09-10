using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResetCore.Util.CommandQueue
{
    public abstract class BaseCommand
    {
        /// <summary>
        /// 所在的CommandQueue
        /// </summary>
        public CommandQueue queue { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        /// <returns></returns>
        public abstract string Description();

        /// <summary>
        /// 执行行为
        /// </summary>
        /// <param name="act"></param>
        public abstract void Execute(Action act);

        /// <summary>
        /// 加入到队列
        /// </summary>
        /// <param name="queue"></param>
        public abstract void AddToQueue(ActionQueue queue);

    }
}
