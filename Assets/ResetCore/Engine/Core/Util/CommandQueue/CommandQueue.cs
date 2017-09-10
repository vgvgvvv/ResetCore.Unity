using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ResetCore.Util.CommandQueue
{
    public class CommandQueue
    {
        private ActionQueue queue = new ActionQueue();

        private List<BaseCommand> commandList = new List<BaseCommand>();
        
        /// <summary>
        /// 添加指令
        /// </summary>
        /// <returns></returns>
        public CommandQueue AddCommand(BaseCommand command)
        {
            command.queue = this;
            commandList.Add(command);
            return this;
        }

        /// <summary>
        /// 移除指令
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public CommandQueue RemoveCommand(BaseCommand command)
        {
            command.queue = null;
            commandList.Remove(command);
            return this;
        }

        /// <summary>
        /// 执行指令队列
        /// </summary>
        public void Execute()
        {
            foreach(var cmd in commandList)
            {
                cmd.AddToQueue(queue);
            }
        }

    }

}
