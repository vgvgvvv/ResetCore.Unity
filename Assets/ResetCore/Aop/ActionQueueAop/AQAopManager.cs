using UnityEngine;
using System.Collections;
using ResetCore.Util;
using System;
using System.Collections.Generic;

namespace ResetCore.AOP
{
    /// <summary>
    /// 非即时触发式行为队列，需要Submit之后才进行调用
    /// </summary>
    public class AQAopManager
    {

        private Queue<Action> queue;

        private AQAopManager()
        {
            queue = new Queue<Action>();
        }

        public static AQAopManager Aop { get { return new AQAopManager(); } }

        public AQAopManager Work(Action<Action> act)
        {
            Action callback = () =>
            {
                if (queue.Count > 0)
                {
                    queue.Dequeue().Invoke();
                }
            };
            queue.Enqueue(() => { act(callback); });
            return this;
        }

        public AQAopManager Work(Action act)
        {
            Action res = () =>
            {
                act();
                if (queue.Count > 0)
                {
                    queue.Dequeue().Invoke();
                }
            };
            queue.Enqueue(res);

            return this;
        }

        public AQAopManager WorkAfterTimes(Action act, float second)
        {
            Work(() =>
            {
                CoroutineTaskManager.Instance.WaitSecondTodo(act, second);
            });
            return this;
        }

        public AQAopManager WorkAfterTimes(Action<Action> act, float second)
        {
            Work((aftAct) =>
            {

                CoroutineTaskManager.Instance.WaitSecondTodo(() => { act(aftAct); }, second);
            });
            return this;
        }

        public void Submit()
        {
            queue.Dequeue().Invoke();
        }
    }
}

