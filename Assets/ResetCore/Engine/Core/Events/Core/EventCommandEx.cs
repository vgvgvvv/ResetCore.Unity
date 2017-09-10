using ResetCore.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResetCore.Event
{
    /// <summary>
    /// 事件命令扩展
    /// </summary>
    public static class EventCommandEx
    {
        /// <summary>
        /// 设置为一次性消息
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static BaseCommand Once(this BaseCommand command)
        {
            command.isOnce = true;
            return command;
        }

        /// <summary>
        /// 同一帧中只取最后一条消息
        /// *该功能不支持同步的单提供器*
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static BaseCommand PoolInOneFrame(this BaseCommand command)
        {
            command.usePool = true;
            return command.PoolInTime(Time.deltaTime);
        }

        /// <summary>
        /// 在几秒内只取最后一条消息
        /// *该功能不支持同步的单提供器*
        /// </summary>
        /// <param name="command"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static BaseCommand PoolInTime(this BaseCommand command, float time)
        {
            command.usePool = true;
            command.poolEventTime = time;
            command.conditionList.Add((args) =>
            {
                if (command is BaseSingleProvidCommand || command is BaseMultProvidCommand)
                {
                    throw new Exception("所有提供器都提供器不支持池功能");
                }
                return !(command.poolEventTime > 0 && command.poolTime < command.poolEventTime);
            });
            command.AddEveryFrameCall(() =>
            {
                if (command.poolEventTime > 0 && command.poolTime > command.poolEventTime)
                {
                    if (command.callPool.Count != 0)
                    {
                        command.CallActDirectly(command.lastCommand);
                    }
                    else
                    {
                        command.ResetPool();
                    }
                }
            });
            //池时间调用
           
            return command;
        }

        /// <summary>
        /// 在监听到消息多次之后才进行调用，使用最后一次的参数
        /// </summary>
        /// <param name="command"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public static BaseCommand PoolByNum(this BaseCommand command, int number)
        {
            command.usePool = true;
            command.poolNumber = number;
            command.conditionList.Add((args) =>
            {
                if (command is BaseSingleProvidCommand)
                {
                    throw new Exception("同步的单提供器不支持池功能");
                }
                return !(command.poolNumber > command.callPool.Count);
            });

            return command;
        }

        /// <summary>
        /// 在一段时间之后重置池
        /// </summary>
        /// <param name="command"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static BaseCommand ResetPoolByTime(this BaseCommand command, float time)
        {
            command.usePool = true;
            command.resetPoolTime = time;
            command.AddEveryFrameCall(() => {
                //重置池
                if (command.resetPoolTime > 0 && command.poolTime > command.resetPoolTime)
                {
                    command.ResetPool();
                }
            });
            return command;
        }

        /// <summary>
        /// 忽略相同的消息,该属性对无参事件无效
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static BaseCommand IgnoreSameEvent(this BaseCommand command)
        {
            command.usePool = true;
            command.ignoreSameEvent = true;
            Func<object[], bool> ignoreCondition = (args) =>
            {
                if (command is BaseSingleProvidCommand)
                {
                    throw new Exception("同步的单提供器不支持池功能");
                }
                //忽略重复
                return !(command.argNumber != 0 && command.ignoreSameEvent && command.CompareWithLastEventArgs(args));
            };

            Func<object[], bool> afterIgnoreCondition = (args) =>
            {
                if (command is BaseSingleProvidCommand)
                {
                    throw new Exception("同步的单提供器不支持池功能");
                }
                //忽略重复
                return !(command.argNumber != 0 && command.ignoreSameEvent && command.CompareWithLastValidEventArgs(args));
            };
            command.conditionListWhenEveryArgIn.Add(ignoreCondition);
            command.conditionList.Add(afterIgnoreCondition);
            return command;
        }

        /// <summary>
        /// 添加预行为
        /// </summary>
        /// <param name="command"></param>
        /// <param name="startcall"></param>
        /// <returns></returns>
        public static BaseCommand AddStartCall(this BaseCommand command, Action startcall)
        {
            command.onStartCall.Add(startcall);
            return command;
        }

        /// <summary>
        /// 添加回调
        /// </summary>
        /// <param name="command"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static BaseCommand AddCallBack(this BaseCommand command, Action callback)
        {
            command.onEndCall.Add(callback);
            return command;
        }

        /// <summary>
        /// 添加计时器轮询回调
        /// </summary>
        /// <param name="command"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static BaseCommand AddEveryFrameCall(this BaseCommand command, Action everyFrameCall)
        {
            command.usePool = true;
            command.poolTimerAfterFirstClickLoopCall.Add(everyFrameCall);
            return command;
        }

        /// <summary>
        /// 设置超时
        /// </summary>
        /// <param name="command"></param>
        /// <param name="time"></param>
        /// <param name="onTimeOut"></param>
        /// <returns></returns>
        public static BaseCommand SetTimeOutOnce(this BaseCommand command, float time, Action onTimeOut)
        {
            command.usePool = true;
            command.isOnce = true;
            ReCoroutineTaskManager.CoroutineTask task = null;
            task = ReCoroutineTaskManager.Instance.WaitSecondTodo(() =>
            {
                command.StopListen();
                if (onTimeOut != null)
                    onTimeOut();
            }, time);
            Action startCall = () =>
            {
                if (task != null)
                    task.Stop();
            };

            return command.AddStartCall(startCall);
        }

        /// <summary>
        /// 当物体销毁时移除监听器
        /// </summary>
        /// <param name="command"></param>
        /// <param name="mono"></param>
        /// <returns></returns>
        public static BaseCommand RemoveWhenDestroy(this BaseCommand command, GameObject go)
        {
            go.GetCallbacks().onDestroy += 
                ()=> command.StopListen();
            return command;
        }

        /// <summary>
        /// 当销毁的时候执行
        /// </summary>
        /// <param name="command"></param>
        /// <param name="go"></param>
        /// <param name="act"></param>
        /// <returns></returns>
        public static BaseCommand WhenMonobehaviorDestory(this BaseCommand command, GameObject go, Action<BaseCommand> act)
        {
            go.GetCallbacks().onDestroy += 
                () => act(command);
            return command;
        }

        /// <summary>
        /// 出错时进行恢复操作
        /// </summary>
        /// <param name="command"></param>
        /// <param name="onError"></param>
        /// <returns></returns>
        public static BaseCommand OnError(this BaseCommand command, Action<Exception> onError)
        {
            command.onError = onError;
            return command;
        }

        /// <summary>
        /// 当任意事件传入（未剔除前的）
        /// </summary>
        /// <param name="command"></param>
        /// <param name="onArgIn"></param>
        /// <returns></returns>
        public static BaseCommand OnEveryArgIn(this BaseCommand command, Action onArgIn)
        {
            command.usePool = true;
            command.onEveryArgIn.Add(onArgIn);
            return command;
        }

        /// <summary>
        /// 直到几秒内没有信息流进入则使用该消息
        /// *该功能不支持同步的单提供器*
        /// </summary>
        /// <param name="command"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static BaseCommand TakeUntil(this BaseCommand command, float time)
        {
            command.usePool = true;
            command.OnEveryArgIn(() =>
            {
                if (command.callPool.Count != 0)
                    command.ResetPool();
            });
            command.PoolInTime(time);
            command.IgnoreSameEvent();
            return command;
        }

        /// <summary>
        /// 添加筛选器
        /// </summary>
        /// <param name="command"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static BaseCommand Fliter(this BaseCommand command, Func<object[], bool> condition)
        {
            command.usePool = true;
            command.conditionListWhenEveryArgIn.Add(condition);
            return command;
        }

        /// <summary>
        /// 获取一个监听下一次事件响应的协程
        /// </summary>
        /// <returns></returns>
        public static ReCoroutineTaskManager.CoroutineTask GetListenLatestCommandCoroutine(this BaseCommand command, Action<bool> callback = null, object bindObject = null)
        {
            return ReCoroutineTaskManager.Instance.AddTask(DoWaitLatestCommand(command), callback, bindObject);
        }

        private static IEnumerator<float> DoWaitLatestCommand(BaseCommand command)
        {
            bool finish = false;
            command.AddCallBack(() => finish = true);
            while (!finish)
            {
                yield return 0;
            }
        } 
    } 

}
