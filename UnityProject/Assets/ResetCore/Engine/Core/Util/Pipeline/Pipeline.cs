using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResetCore.Util
{
    public class Pipeline<I, O>
    {
        private List<BasePass> passList = new List<BasePass>();

        /// <summary>
        /// 处理同步任务
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public O SyncProcess(I input)
        {
            object temp = input;
            foreach(var pass in passList)
            {
                if (pass is BaseAysnPass)
                {
                    Debug.unityLogger.LogError("Pipeline", "无法同步处理异步Pass：" + pass.GetType().Name);
                    break;
                }
                temp = pass.Handle(temp);
            }
            return (O)temp;
        }

        /// <summary>
        /// 处理异步任务
        /// </summary>
        /// <param name="input"></param>
        /// <param name="outputHandler"></param>
        public void AsynProcess(I input, Action<O> outputHandler)
        {
            ActionQueueWithArg actionQueue = new ActionQueueWithArg();

            for(int i = 0; i < passList.Count; i ++)
            {
                var tempPass = passList[i];
                if (tempPass is BaseAysnPass)
                {
                    BaseAysnPass aysnPass = (BaseAysnPass)tempPass;
                    actionQueue.AddAction(aysnPass.Handle);
                }
                else
                {
                    actionQueue.AddAction((objs, act) =>
                    {
                        act(tempPass.Handle(objs));
                    });
                }
            }
            actionQueue.AddAction((obj, act) =>
            {
                outputHandler((O)obj);
            });
            actionQueue.Start(input);
        }

        /// <summary>
        /// 添加处理器
        /// </summary>
        /// <param name="pass"></param>
        public Pipeline<I, O> AddPass(BasePass pass)
        {
            passList.Add(pass);
            return this;
        }

        /// <summary>
        /// 添加同步处理器行为
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public Pipeline<I, O> AddPass(Func<I, O> func)
        {
            passList.Add(new CommonPass<I,O>(func));
            return this;
        }

        /// <summary>
        /// 添加异步处理器行为
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        public Pipeline<I, O> AddPass(Action<I, Action<object>> act)
        {
            passList.Add(new CommonAysnPass<I, O>(act));
            return this;
        }
    }
}

