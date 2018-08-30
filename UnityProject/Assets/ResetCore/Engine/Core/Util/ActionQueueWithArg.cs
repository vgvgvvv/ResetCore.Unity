using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResetCore.Util
{

    public class ActionQueueWithArg
    {
        //行动队列
        private Queue<Action<object, Action<object>>> m_queue = 
            new Queue<Action<object, Action<object>>>();

        /// <summary>
        /// 添加行为
        /// </summary>
        /// <param name="actionCB"></param>
        /// <returns></returns>
        public ActionQueueWithArg AddAction(Action<object, Action<object>> actionCB)
        {
            m_queue.Enqueue(actionCB);
            return this;
        }

        /// <summary>
        /// 开始执行行为队列
        /// </summary>
        /// <param name="input"></param>
        public void Start(object input)
        {
            object temp = input;
            Action<object, Action<object>> currentAction = m_queue.Dequeue();
            currentAction.Invoke(temp, DequeueAndInvoke);
        }

        private void DequeueAndInvoke(object objs)
        {
            if (m_queue.Count <= 0)
                return;

            m_queue.Dequeue().Invoke(objs, DequeueAndInvoke);
        }

        /// <summary>
        /// 等待一些时间
        /// </summary>
        /// <param name="second"></param>
        /// <returns></returns>
        public ActionQueueWithArg Wait(float second)
        {
            return AddAction((obj, act) =>
            {
                CoroutineTaskManager.Instance.WaitSecondTodo(() =>
                {
                    act(obj);
                }, second);
            });
        }


        /// <summary>
        /// 清理行为队列
        /// </summary>
        public void Clean()
        {
            m_queue.Clear();
            m_queue = new Queue<Action<object, Action<object>>>();
        }

    }
}
