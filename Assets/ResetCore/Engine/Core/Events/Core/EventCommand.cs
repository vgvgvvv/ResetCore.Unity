using ResetCore.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResetCore.Event
{
    public enum CommandType
    {
        Event,
        SingleProvider,
        MultProvider
    }

    public abstract class BaseCommand
    {
        private static int latestId = 0;
        /// <summary>
        /// 默认Id
        /// </summary>
        public int uid { get; private set; }
        /// <summary>
        /// 事件类型
        /// </summary>
        public string eventType { get; private set; }
        /// <summary>
        /// 参数数量
        /// </summary>
        public int argNumber { get; private set; }

        /// <summary>
        /// 绑定物体
        /// </summary>
        public object bindObject { get; private set; }

        private  EventController controller
        {
            get
            {
                if (bindObject == null)
                {
                    return EventDispatcher.globelEventController;
                }
                else
                {
                    return MonoEventDispatcher.GetMonoController(bindObject);
                }
            }
        }

        /// <summary>
        /// 是否已经停止监听
        /// </summary>
        public bool listening
        {
            get
            {
                var router = controller.TheRouter;
                return router.ContainsKey(eventType) &&
                            router[eventType].Contains(this);
            }
        }

        /// <summary>
        /// 命令类型
        /// </summary>
        public abstract CommandType commandType { get; }

        /// <summary>
        /// 是否只执行一次
        /// </summary>
        public bool isOnce { get; set; }

        /// <summary>
        /// 是否在一段时间内只取最后一条消息
        /// *该功能不支持所有的提供器*
        /// </summary>
        public float poolEventTime { get; set; }
        /// <summary>
        /// 重置参数池
        /// </summary>
        public float resetPoolTime { get; set; }
        /// <summary>
        /// 距离第一次点击过去的时间
        /// </summary>
        public float poolTime { get; private set; }
        //池子中的第一个消息过来的时候重启计时器
        private ReCoroutineTaskManager.CoroutineTask poolTimerAfterFirstClick;

        /// <summary>
        /// 需要储存的数量,达到该数量之后才能完成调用
        /// *该功能不支持同步的单提供器*
        /// </summary>
        public int poolNumber { get; set; }

        /// <summary>
        /// 忽略重复且相同的消息
        /// *该功能不支持同步的单提供器*
        /// </summary>
        public bool ignoreSameEvent { get; set; }

        /// <summary>
        /// 上一次传入的参数
        /// </summary>
        public object[] lastCommand { get; private set; }

        /// <summary>
        /// 上一个被采纳的参数（通过筛选的）
        /// </summary>
        public object[] lastValidCommand { get; private set; }

        /// <summary>
        /// 每次调用的时候都会将命令加入池中，当完成命令调用时清空池
        /// *该功能不支持同步的单提供器*
        /// </summary>
        public List<object[]> callPool { get; private set; }

        /// <summary>
        /// 任何参数传入时进行调用
        /// </summary>
        public List<Action> onEveryArgIn { get; set;}

        /// <summary>
        /// 开始调用Act的时候进行调用
        /// </summary>
        public List<Action> onStartCall { get; set; }

        /// <summary>
        /// 结束Act的时候调用
        /// </summary>
        public List<Action> onEndCall { get; set; }

        /// <summary>
        /// 出现错误时进行的操作
        /// </summary>
        public Action<Exception> onError { get; set; }

        /// <summary>
        /// 在点击后计时器计时的时候进行计时器轮询
        /// </summary>
        public List<Action> poolTimerAfterFirstClickLoopCall { get; set; }

        /// <summary>
        /// 判断列表，所有参数首次进入，不会触发轮询器
        /// </summary>
        public List<Func<object[], bool>> conditionListWhenEveryArgIn { get; set; }

        /// <summary>
        /// 判断列表(通过第一次参数筛选后的，触发轮询器之后的)
        /// </summary>
        public List<Func<object[], bool>> conditionList { get; set; }

        /// <summary>
        /// 是否使用池
        /// </summary>
        public bool usePool { get; set; }

        /// <summary>
        /// 通用行为
        /// </summary>
        public abstract Delegate commonAct { get; }

        public abstract object DynamicCall(object[] args);

        public BaseCommand(string eventType, int argNumber, object bindObject = null)
        {

            this.ignoreSameEvent = false;
            this.isOnce = false;
            this.poolEventTime = 0;
            this.resetPoolTime = 0;
            this.poolNumber = 0;
            this.callPool = new List<object[]>();
            this.eventType = eventType;
            this.uid = latestId;
            this.argNumber = argNumber;
            this.onEveryArgIn = new List<Action>();
            this.onStartCall = new List<Action>();
            this.onEndCall = new List<Action>();
            this.conditionList = new List<Func<object[], bool>>();
            this.poolTimerAfterFirstClickLoopCall = new List<Action>();
            this.conditionListWhenEveryArgIn = new List<Func<object[], bool>>();
            this.usePool = false;
            this.bindObject = bindObject;
            latestId++;
            ResetPool();

        }

        /// <summary>
        /// 调用事件处理函数
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public object CallAct(params object[] args)
        {
            try
            {
                if (args == null)
                    args = new object[0];

                if (args.Length != argNumber)
                {
                    throw new Exception("参数数量错误,传入的数量为 " + args.Length + "个 而正确的为 " + argNumber + "个");
                }

                if (!CheckCondition(conditionListWhenEveryArgIn, args))
                    return null;

                for (int i = 0; i < onEveryArgIn.Count; i++)
                {
                    onEveryArgIn[i]();
                }

                if (callPool.Count == 0 && usePool)
                {
                    if (poolTimerAfterFirstClick != null)
                        poolTimerAfterFirstClick.Stop();

                    //重新时间计数
                    poolTimerAfterFirstClick = ReCoroutineTaskManager.Instance.LoopByEveryFrame(() =>
                    {
                        if (!listening)
                        {
                            poolTimerAfterFirstClick.Stop();
                        }
                        poolTime += Time.deltaTime;

                        for (int i = 0; i < poolTimerAfterFirstClickLoopCall.Count; i++)
                            poolTimerAfterFirstClickLoopCall[i]();

                    }, -1);
                }

                callPool.Add(args);

                lastCommand = args;

                return CallActDirectly(args);
                
            }catch(Exception e)
            {
                Debug.LogException(e);
                if(onError != null)
                    onError(e);
            }
            return null;
        }

        /// <summary>
        /// 直接调用行为
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public object CallActDirectly(object[] args)
        {
            if (!CheckCondition(conditionList, args))
                return null;

            lastValidCommand = args;

            //单次调用
            if (isOnce)
                StopListen();

            for (int i = 0; i < onStartCall.Count; i++)
                onStartCall[i]();

            var res = DynamicCall(args);

            for(int i = 0; i < onEndCall.Count; i++)
                onEndCall[i]();


            ResetPool();
            return res;
        }

        /// <summary>
        /// 停止监听
        /// </summary>
        public void StopListen()
        {
            var router = controller.TheRouter;
            if (router.ContainsKey(eventType))
                router[eventType].Remove(this);
        }

        //重置池子
        public void ResetPool()
        {
            callPool.Clear();
            poolTime = 0;
            //停止计时器
            if (poolTimerAfterFirstClick != null)
                poolTimerAfterFirstClick.Stop();
        }

        /// <summary>
        /// 与上一次命令进行比较
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool CompareWithLastEventArgs(object[] args)
        {
            if (lastCommand == null)
                return false;

            //对于数量为0的命令来说不存在重复的概念
            if (lastCommand.Length == 0)
                return false;

            if (args.Length != lastCommand.Length)
                return false;

            for (int i = 0; i < lastCommand.Length; i++)
            {
                if (!args[i].Equals(lastCommand[i]))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 与上一次的有效命令进行比较
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool CompareWithLastValidEventArgs(object[] args)
        {
            if (lastValidCommand == null)
                return false;

            //对于数量为0的命令来说不存在重复的概念
            if (lastValidCommand.Length == 0)
                return false;

            if (args.Length != lastValidCommand.Length)
                return false;

            for (int i = 0; i < lastValidCommand.Length; i++)
            {
                if (!args[i].Equals(lastValidCommand[i]))
                    return false;
            }
            return true;
        }
        #region 私有方法


        //检查条件
        private bool CheckCondition(List<Func<object[], bool>> list, object[] args)
        {
            if (list.Count == 0)
                return true;

            for (int i = 0; i < list.Count; i++)
            {
                if (!list[i](args))
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

    }

    #region 请求部分的Command

    public abstract class BaseEventCommand : BaseCommand
    {
        public override CommandType commandType
        {
            get
            {
                return CommandType.Event;
            }
        }

        public BaseEventCommand(string eventType, int argNumber, object bindObject = null) 
            : base(eventType, argNumber, bindObject)
        {
        }
    }

    public class EventCommand : BaseEventCommand
    {
        public Action act { get; private set; }

        public override Delegate commonAct { get{ return act; } }

        public EventCommand(string eventType, Action act, int argNumber, object bindObject = null) 
            : base(eventType, argNumber, bindObject) { this.act = act; }

        public override object DynamicCall(object[] args)
        {
            act();
            return null;
        }
    }

    public class EventCommand<T1> : BaseEventCommand
    {
        public Action<T1> act { get; private set; }
         public override Delegate commonAct { get { return act; } }
        public EventCommand(string eventType, Action<T1> act, int argNumber, object bindObject = null) 
            : base(eventType, argNumber, bindObject) { this.act = act; }

        public override object DynamicCall(object[] args)
        {
            act((T1)args[0]);
            return null;
        }
    }

    public class EventCommand<T1, T2> : BaseEventCommand
    {
        public Action<T1, T2> act { get; private set; }
        public override Delegate commonAct { get { return act; } }
        public EventCommand(string eventType, Action<T1, T2> act, int argNumber, object bindObject = null) 
            : base(eventType, argNumber, bindObject) { this.act = act; }
        public override object DynamicCall(object[] args)
        {
            act((T1)args[0], (T2)args[1]);
            return null;
        }
    }

    public class EventCommand<T1, T2, T3> : BaseEventCommand
    {
        public Action<T1, T2, T3> act { get; private set; }
        public override Delegate commonAct { get { return act; } }
        public EventCommand(string eventType, Action<T1, T2, T3> act, int argNumber, object bindObject = null) 
            : base(eventType, argNumber, bindObject) { this.act = act; }
        public override object DynamicCall(object[] args)
        {
            act((T1)args[0], (T2)args[1], (T3)args[2]);
            return null;
        }
    }

    public class EventCommand<T1, T2, T3, T4> : BaseEventCommand
    {
        public Action<T1, T2, T3, T4> act { get; private set; }
        public override Delegate commonAct { get { return act; } }
        public EventCommand(string eventType, Action<T1, T2, T3, T4> act, int argNumber, object bindObject = null) 
            : base(eventType, argNumber, bindObject) { this.act = act; }
        public override object DynamicCall(object[] args)
        {
            act((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3]);
            return null;
        }
    }
    #endregion 请求部分

    #region 单提供器部分
    public abstract class BaseSingleProvidCommand : BaseCommand
    {
        public override CommandType commandType
        {
            get
            {
                return CommandType.SingleProvider;
            }
        }
        public BaseSingleProvidCommand(string eventType, int argNumber, object bindObject = null) 
            : base(eventType, argNumber, bindObject)
        {
        }
    }

    public class SingleProvidCommand<Res> : BaseSingleProvidCommand
    {
        public Func<Res> act { get; private set; }
        public override Delegate commonAct { get { return act; } }
        public SingleProvidCommand(string eventType, Func<Res> act, int argNumber, object bindObject = null) 
            : base(eventType, argNumber, bindObject) { this.act = act; }
        public override object DynamicCall(object[] args)
        {
            return act();
        }
    }

    public class SingleProvidCommand<A1, Res> : BaseSingleProvidCommand
    {
        public Func<A1, Res> act { get; private set; }
        public override Delegate commonAct { get { return act; } }
        public SingleProvidCommand(string eventType, Func<A1, Res> act, int argNumber, object bindObject = null) 
            : base(eventType, argNumber, bindObject) { this.act = act; }
        public override object DynamicCall(object[] args)
        {
            return act((A1)args[0]);
        }
    }

    public class SingleProvidCommand<A1, A2, Res> : BaseSingleProvidCommand
    {
        public Func<A1, A2, Res> act { get; private set; }
        public override Delegate commonAct { get { return act; } }
        public SingleProvidCommand(string eventType, Func<A1, A2, Res> act, int argNumber, object bindObject = null) 
            : base(eventType, argNumber, bindObject) { this.act = act; }
        public override object DynamicCall(object[] args)
        {
            return act((A1)args[0], (A2)args[1]);
        }
    }

    public class SingleProvidCommand<A1, A2, A3, Res> : BaseSingleProvidCommand
    {
        public Func<A1, A2, A3, Res> act { get; private set; }
        public override Delegate commonAct { get { return act; } }
        public SingleProvidCommand(string eventType, Func<A1, A2, A3, Res> act, int argNumber, object bindObject = null) 
            : base(eventType, argNumber, bindObject) { this.act = act; }
        public override object DynamicCall(object[] args)
        {
            return act((A1)args[0], (A2)args[1], (A3)args[2]);
        }
    }

    public class SingleProvidCommand<A1, A2, A3, A4, Res> : BaseSingleProvidCommand
    {
        public Func<A1, A2, A3, A4, Res> act { get; private set; }
        public override Delegate commonAct { get { return act; } }
        public SingleProvidCommand(string eventType, Func<A1, A2, A3, A4, Res> act, int argNumber, object bindObject = null) 
            : base(eventType, argNumber, bindObject) { this.act = act; }
        public override object DynamicCall(object[] args)
        {
            return act((A1)args[0], (A2)args[1], (A3)args[2], (A4)args[3]);
        }
    }
    #endregion

    #region 多提供器
    public abstract class BaseMultProvidCommand : BaseCommand
    {
        public override CommandType commandType
        {
            get
            {
                return CommandType.MultProvider;
            }
        }
        public BaseMultProvidCommand(string eventType, int argNumber, object bindObject = null) 
            : base(eventType, argNumber, bindObject)
        {
        }
    }
    public class MultProvidCommand<Res> : BaseMultProvidCommand
    {
        public Func<Res> act { get; private set; }
        public override Delegate commonAct { get { return act; } }
        public MultProvidCommand(string eventType, Func<Res> act, int argNumber, object bindObject = null) 
            : base(eventType, argNumber, bindObject) { this.act = act; }
        public override object DynamicCall(object[] args)
        {
            return act();
        }
    }

    public class MultProvidCommand<A1, Res> : BaseMultProvidCommand
    {
        public Func<A1, Res> act { get; private set; }
        public override Delegate commonAct { get { return act; } }
        public MultProvidCommand(string eventType, Func<A1, Res> act, int argNumber, object bindObject = null) 
            : base(eventType, argNumber, bindObject) { this.act = act; }
        public override object DynamicCall(object[] args)
        {
            return act((A1)args[0]);
        }
    }

    public class MultProvidCommand<A1, A2, Res> : BaseMultProvidCommand
    {
        public Func<A1, A2, Res> act { get; private set; }
        public override Delegate commonAct { get { return act; } }
        public MultProvidCommand(string eventType, Func<A1, A2, Res> act, int argNumber, object bindObject = null) 
            : base(eventType, argNumber, bindObject) { this.act = act; }
        public override object DynamicCall(object[] args)
        {
            return act((A1)args[0], (A2)args[1]);
        }
    }

    public class MultProvidCommand<A1, A2, A3, Res> : BaseMultProvidCommand
    {
        public Func<A1, A2, A3, Res> act { get; private set; }
        public override Delegate commonAct { get { return act; } }
        public MultProvidCommand(string eventType, Func<A1, A2, A3, Res> act, int argNumber, object bindObject = null) 
            : base(eventType, argNumber, bindObject) { this.act = act; }
        public override object DynamicCall(object[] args)
        {
            return act((A1)args[0], (A2)args[1], (A3)args[2]);
        }
    }

    public class MultProvidCommand<A1, A2, A3, A4, Res> : BaseMultProvidCommand
    {
        public Func<A1, A2, A3, A4, Res> act { get; private set; }
        public override Delegate commonAct { get { return act; } }
        public MultProvidCommand(string eventType, Func<A1, A2, A3, A4, Res> act, int argNumber, object bindObject = null) 
            : base(eventType, argNumber, bindObject) { this.act = act; }
        public override object DynamicCall(object[] args)
        {
            return act((A1)args[0], (A2)args[1], (A3)args[2], (A4)args[3]);
        }
    }

    #endregion 多提供器

}
