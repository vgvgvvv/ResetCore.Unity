using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using ResetCore.Util;

namespace ResetCore.Event
{
    public class EventController
    {
        /// <summary>
        /// 不变事件不会被清除
        /// </summary>
        private List<string> m_permanentEvents = new List<string>();
        private Dictionary<string, List<BaseCommand>> m_theRouter = new Dictionary<string, List<BaseCommand>>();
        public Dictionary<string, List<BaseCommand>> TheRouter
        {
            get
            {
                return this.m_theRouter;
            }
        }

        #region 添加监听器
        //添加监听器
        public BaseCommand AddEventListener(string eventType, Action handler, object bindObject = null)
        {
            var command = new EventCommand(eventType, handler, 0, bindObject);
            this.OnListenerAdding(eventType, handler);
            this.m_theRouter[eventType].Add(command);
            return command;
        }
        public BaseCommand AddEventListener<T>(string eventType, Action<T> handler, object bindObject = null)
        {
            var command = new EventCommand<T>(eventType, handler, 1, bindObject);
            this.OnListenerAdding(eventType, handler);
            this.m_theRouter[eventType].Add(command);
            return command;
        }
        public BaseCommand AddEventListener<T, U>(string eventType, Action<T, U> handler, object bindObject = null)
        {
            var command = new EventCommand<T, U>(eventType, handler, 2, bindObject);
            this.OnListenerAdding(eventType, handler);
            this.m_theRouter[eventType].Add(command);
            return command;
        }
        public BaseCommand AddEventListener<T, U, V>(string eventType, Action<T, U, V> handler, object bindObject = null)
        {
            var command = new EventCommand<T, U, V>(eventType, handler, 3, bindObject);
            this.OnListenerAdding(eventType, handler);
            this.m_theRouter[eventType].Add(command);
            return command;
        }
        public BaseCommand AddEventListener<T, U, V, W>(string eventType, Action<T, U, V, W> handler, object bindObject = null)
        {
            var command = new EventCommand<T, U, V, W>(eventType, handler, 4, bindObject);
            this.OnListenerAdding(eventType, handler);
            this.m_theRouter[eventType].Add(command);
            return command;
        }
        
        //添加提供器
        public BaseCommand AddSingleProvider<Res>(string eventType, Func<Res> provider, object bindObject = null)
        {
            if (OnSingleProviderAdding(eventType, provider))
            {
                var command = new SingleProvidCommand<Res>(eventType, provider, 0, bindObject);
                this.m_theRouter[eventType].Add(command);
                return command;
            }
            return null;
        }
        public BaseCommand AddSingleProvider<A1, Res>(string eventType, Func<A1, Res> provider, object bindObject = null)
        {
            if (OnSingleProviderAdding(eventType, provider))
            {
                var command = new SingleProvidCommand<A1, Res>(eventType, provider, 1, bindObject);
                this.m_theRouter[eventType].Add(command);
                return command;
            }
            return null;
        }
        public BaseCommand AddSingleProvider<A1, A2, Res>(string eventType, Func<A1, A2, Res> provider, object bindObject = null)
        {
            if (OnSingleProviderAdding(eventType, provider))
            {
                var command = new SingleProvidCommand<A1, A2, Res>(eventType, provider, 2, bindObject);
                this.m_theRouter[eventType].Add(command);
                return command;
            }
            return null;
        }
        public BaseCommand AddSingleProvider<A1, A2, A3, Res>(string eventType, Func<A1, A2, A3, Res> provider, object bindObject = null)
        {
            if (OnSingleProviderAdding(eventType, provider))
            {
                var command = new SingleProvidCommand<A1, A2, A3, Res>(eventType, provider, 3, bindObject);
                this.m_theRouter[eventType].Add(command);
                return command;
            }
            return null;
        }
        public BaseCommand AddSingleProvider<A1, A2, A3, A4, Res>(string eventType, Func<A1, A2, A3, A4, Res> provider, object bindObject = null)
        {
            if (OnSingleProviderAdding(eventType, provider))
            {
                var command = new SingleProvidCommand<A1, A2, A3, A4, Res>(eventType, provider, 4, bindObject);
                this.m_theRouter[eventType].Add(command);
                return command;
            }
            return null;
        }

        //添加提供器
        public BaseCommand AddMultProvider<Res>(string eventType, Func<Res> provider, object bindObject = null)
        {
            var command = new MultProvidCommand<Res>(eventType, provider, 0, bindObject);
            OnMultProviderAdding(eventType, provider);
            this.m_theRouter[eventType].Add(command);
            return command;
        }
        public BaseCommand AddMultProvider<A1, Res>(string eventType, Func<A1, Res> provider, object bindObject = null)
        {
            var command = new MultProvidCommand<A1, Res>(eventType, provider, 1, bindObject);
            OnMultProviderAdding(eventType, provider);
            this.m_theRouter[eventType].Add(command);
            return command;
        }
        public BaseCommand AddMultProvider<A1, A2, Res>(string eventType, Func<A1, A2, Res> provider, object bindObject = null)
        {
            var command = new MultProvidCommand<A1, A2, Res>(eventType, provider, 2, bindObject);
            OnMultProviderAdding(eventType, provider);
            this.m_theRouter[eventType].Add(command);
            return command;
        }
        public BaseCommand AddMultProvider<A1, A2, A3, Res>(string eventType, Func<A1, A2, A3, Res> provider, object bindObject = null)
        {
            var command = new MultProvidCommand<A1, A2, A3, Res>(eventType, provider, 3, bindObject);
            OnMultProviderAdding(eventType, provider);
            this.m_theRouter[eventType].Add(command);
            return command;
        }
        public BaseCommand AddMultProvider<A1, A2, A3, A4, Res>(string eventType, Func<A1, A2, A3, A4, Res> provider, object bindObject = null)
        {
            var command = new MultProvidCommand<A1, A2, A3, A4, Res>(eventType, provider, 4, bindObject);
            OnMultProviderAdding(eventType, provider);
            this.m_theRouter[eventType].Add(command);
            return command;
        }
        #endregion 添加监听器

        #region 移除监听器
        public void RemoveEventListener<T>(string eventType, Action<T> handler)
        {
            if (this.OnListenerRemoving(eventType, handler))
            {
                this.m_theRouter[eventType].Remove(m_theRouter[eventType]
                    .Where((cmd)=> { return ((EventCommand<T>)cmd).act == handler; }).First());
                this.OnListenerRemoved(eventType);
            }
        }
        public void RemoveEventListener(string eventType, Action handler)
        {
            if (this.OnListenerRemoving(eventType, handler))
            {
                this.m_theRouter[eventType].Remove(m_theRouter[eventType]
                    .Where((cmd) => { return ((EventCommand)cmd).act == handler; }).First());
                this.OnListenerRemoved(eventType);
            }
        }
        public void RemoveEventListener<T, U>(string eventType, Action<T, U> handler)
        {
            if (this.OnListenerRemoving(eventType, handler))
            {
                this.m_theRouter[eventType].Remove(m_theRouter[eventType]
                    .Where((cmd) => { return ((EventCommand<T, U>)cmd).act == handler; }).First());
                this.OnListenerRemoved(eventType);
            }
        }
        public void RemoveEventListener<T, U, V>(string eventType, Action<T, U, V> handler)
        {
            if (this.OnListenerRemoving(eventType, handler))
            {
                this.m_theRouter[eventType].Remove(m_theRouter[eventType]
                    .Where((cmd) => { return ((EventCommand<T, U, V>)cmd).act == handler; }).First());
                this.OnListenerRemoved(eventType);
            }
        }
        public void RemoveEventListener<T, U, V, W>(string eventType, Action<T, U, V, W> handler)
        {
            if (this.OnListenerRemoving(eventType, handler))
            {
                this.m_theRouter[eventType].Remove(m_theRouter[eventType]
                    .Where((cmd) => { return ((EventCommand<T, U, V, W>)cmd).act == handler; }).First());
                this.OnListenerRemoved(eventType);
            }
        }

        //移除单返回提供器
        public void RemoveSingleProvider(string providerType)
        {
            if (OnSingleProviderRemoving(providerType))
            {
                m_theRouter.Remove(providerType);
                OnSingleProviderRemoved(providerType);
            }
        }

        //移除多返回提供器
        public void RemoveMultProvider<Res>(string providerType, Func<Res> provider)
        {
            if (this.OnMultProviderRemoving(providerType, provider))
            {
                this.m_theRouter[providerType].Remove(m_theRouter[providerType]
                    .Where((cmd) => { return ((MultProvidCommand<Res>)cmd).act == provider; }).First());
                this.OnMultProviderRemoved(providerType);
            }
        }
        public void RemoveMultProvider<A1, Res>(string providerType, Func<A1, Res> provider)
        {
            if (this.OnMultProviderRemoving(providerType, provider))
            {
                this.m_theRouter[providerType].Remove(m_theRouter[providerType]
                    .Where((cmd) => { return ((MultProvidCommand<A1, Res>)cmd).act == provider; }).First());
                this.OnMultProviderRemoved(providerType);
            }
        }
        public void RemoveMultProvider<A1, A2, Res>(string providerType, Func<A1, A2, Res> provider)
        {
            if (this.OnMultProviderRemoving(providerType, provider))
            {
                this.m_theRouter[providerType].Remove(m_theRouter[providerType]
                    .Where((cmd) => { return ((MultProvidCommand<A1, A2, Res>)cmd).act == provider; }).First());
                this.OnMultProviderRemoved(providerType);
            }
        }
        public void RemoveMultProvider<A1, A2, A3, Res>(string providerType, Func<A1, A2, A3, Res> provider)
        {
            if (this.OnMultProviderRemoving(providerType, provider))
            {
                this.m_theRouter[providerType].Remove(m_theRouter[providerType]
                    .Where((cmd) => { return ((MultProvidCommand<A1, A2, A3, Res>)cmd).act == provider; }).First());
                this.OnMultProviderRemoved(providerType);
            }
        }
        public void RemoveMultProvider<A1, A2, A3, A4, Res>(string providerType, Func<A1, A2, A3, A4, Res> provider)
        {
            if (this.OnMultProviderRemoving(providerType, provider))
            {
                this.m_theRouter[providerType].Remove(m_theRouter[providerType]
                    .Where((cmd) => { return ((MultProvidCommand<A1, A2, A3, A4, Res>)cmd).act == provider; }).First());
                this.OnMultProviderRemoved(providerType);
            }
        }
        #endregion

        #region 触发事件
        public void TriggerEvent(string eventType)
        {
            List<BaseCommand> delegate2;
            if (this.m_theRouter.TryGetValue(eventType, out delegate2))
            {
                for (int i = 0; i < delegate2.Count; i++)
                {
                    try
                    {
                        if (delegate2[i] is EventCommand)
                        {
                            ((EventCommand)delegate2[i]).CallAct();
                        }
                        else
                        {
                            throw new EventException(string.Format("TriggerEvent {0} error: types of parameters are not match.", eventType));
                        }
                    }
                    catch (Exception exception)
                    {
                        Debug.unityLogger.LogException(exception);
                    }
                }
            }
        }
        public void TriggerEvent<T>(string eventType, T arg1)
        {
            List<BaseCommand> delegate2;
            if (this.m_theRouter.TryGetValue(eventType, out delegate2))
            {
                for (int i = 0; i < delegate2.Count; i++)
                {
                    try
                    {
                        if (delegate2[i] is EventCommand<T>)
                        {
                            ((EventCommand<T>)delegate2[i]).CallAct(arg1);
                        }
                        else
                        {
                            throw new EventException(string.Format("TriggerEvent {0} error: types of parameters are not match.", eventType));
                        }
                    }
                    catch (Exception exception)
                    {
                        Debug.unityLogger.LogException(exception);
                    }
                }
            }
        }
        public void TriggerEvent<T, U>(string eventType, T arg1, U arg2)
        {
            List<BaseCommand> delegate2;
            if (this.m_theRouter.TryGetValue(eventType, out delegate2))
            {
                for (int i = 0; i < delegate2.Count; i++)
                {
                    try
                    {
                        if (delegate2[i] is EventCommand<T, U>)
                        {
                            ((EventCommand<T, U>)delegate2[i]).CallAct(arg1, arg2);
                        }
                        else
                        {
                            throw new EventException(string.Format("TriggerEvent {0} error: types of parameters are not match.", eventType));
                        }
                    }
                    catch (Exception exception)
                    {
                        Debug.unityLogger.LogException(exception);
                    }
                }
            }
        }
        public void TriggerEvent<T, U, V>(string eventType, T arg1, U arg2, V arg3)
        {
            List<BaseCommand> delegate2;
            if (this.m_theRouter.TryGetValue(eventType, out delegate2))
            {
                for (int i = 0; i < delegate2.Count; i++)
                {
                    try
                    {
                        if (delegate2[i] is EventCommand<T, U, V>)
                        {
                            ((EventCommand<T, U, V>)delegate2[i]).CallAct(arg1, arg2, arg3);
                        }
                        else
                        {
                            throw new EventException(string.Format("TriggerEvent {0} error: types of parameters are not match.", eventType));
                        }
                    }
                    catch (Exception exception)
                    {
                        Debug.unityLogger.LogException(exception);
                    }
                }
            }
        }
        public void TriggerEvent<T, U, V, W>(string eventType, T arg1, U arg2, V arg3, W arg4)
        {
            List<BaseCommand> delegate2;
            if (this.m_theRouter.TryGetValue(eventType, out delegate2))
            {
                for (int i = 0; i < delegate2.Count; i++)
                {
                    try
                    {
                        if (delegate2[i] is EventCommand<T, U, V, W>)
                        {
                            ((EventCommand<T, U, V, W>)delegate2[i]).CallAct(arg1, arg2, arg3, arg4);
                        }
                        else
                        {
                            throw new EventException(string.Format("TriggerEvent {0} error: types of parameters are not match.", eventType));
                        }
                    }
                    catch (Exception exception)
                    {
                        Debug.unityLogger.LogException(exception);
                    }
                }
            }
        }

        //请求单返回请求器
        public Res RequestSingleProvider<Res>(string providerType)
        {
            List<BaseCommand> delegate2;
            if (this.m_theRouter.TryGetValue(providerType, out delegate2))
            {
                try
                {
                    if (delegate2[0] is Func<Res>)
                    {
                        return (Res)((SingleProvidCommand<Res>)delegate2[0]).CallAct();
                    }
                    else
                    {
                        throw new EventException(string.Format("TriggerEvent {0} error: types of parameters are not match.", providerType));
                    }
                }
                catch (Exception exception)
                {
                    Debug.unityLogger.LogException(exception);
                }
            }
            else
            {
                throw new EventException("未找到对应的提供器");
            }
            return default(Res);
        }
        public Res RequestSingleProvider<A1, Res>(string providerType, A1 arg1)
        {
            List<BaseCommand> delegate2;
            if (this.m_theRouter.TryGetValue(providerType, out delegate2))
            {
                try
                {
                    if (delegate2[0] is Func<A1, Res>)
                    {
                        return (Res)((SingleProvidCommand<A1, Res>)delegate2[0]).CallAct(arg1);
                    }
                    else
                    {
                        throw new EventException(string.Format("TriggerEvent {0} error: types of parameters are not match.", providerType));
                    }
                }
                catch (Exception exception)
                {
                    Debug.unityLogger.LogException(exception);
                }
            }
            else
            {
                throw new EventException("未找到对应的提供器");
            }
            return default(Res);
        }
        public Res RequestSingleProvider<A1, A2, Res>(string providerType, A1 arg1, A2 arg2)
        {
            List<BaseCommand> delegate2;
            if (this.m_theRouter.TryGetValue(providerType, out delegate2))
            {
                try
                {
                    if (delegate2[0] is Func<A1, A2, Res>)
                    {
                        return (Res)((SingleProvidCommand<A1, A2, Res>)delegate2[0]).CallAct(arg1, arg2);
                    }
                    else
                    {
                        throw new EventException(string.Format("TriggerEvent {0} error: types of parameters are not match.", providerType));
                    }
                }
                catch (Exception exception)
                {
                    Debug.unityLogger.LogException(exception);
                }
            }
            else
            {
                throw new EventException("未找到对应的提供器");
            }
            return default(Res);
        }
        public Res RequestSingleProvider<A1, A2, A3, Res>(string providerType, A1 arg1, A2 arg2, A3 arg3)
        {
            List<BaseCommand> delegate2;
            if (this.m_theRouter.TryGetValue(providerType, out delegate2))
            {
                try
                {
                    if (delegate2[0] is Func<A1, A2, A3, Res>)
                    {
                        return (Res)((SingleProvidCommand<A1, A2, A3, Res>)delegate2[0]).CallAct(arg1, arg2, arg3);
                    }
                    else
                    {
                        throw new EventException(string.Format("TriggerEvent {0} error: types of parameters are not match.", providerType));
                    }
                }
                catch (Exception exception)
                {
                    Debug.unityLogger.LogException(exception);
                }
            }
            else
            {
                throw new EventException("未找到对应的提供器");
            }
            return default(Res);
        }
        public Res RequestSingleProvider<A1, A2, A3, A4, Res>(string providerType, A1 arg1, A2 arg2, A3 arg3, A4 arg4)
        {
            List<BaseCommand> delegate2;
            if (this.m_theRouter.TryGetValue(providerType, out delegate2))
            {
                try
                {
                    if (delegate2[0] is Func<A1, A2, A3, A4, Res>)
                    {
                        return (Res)((SingleProvidCommand<A1, A2, A3, A4, Res>)delegate2[0]).CallAct(arg1, arg2, arg3, arg4);
                    }
                    else
                    {
                        throw new EventException(string.Format("TriggerEvent {0} error: types of parameters are not match.", providerType));
                    }
                }
                catch (Exception exception)
                {
                    Debug.unityLogger.LogException(exception);
                }
            }
            else
            {
                throw new EventException("未找到对应的提供器");
            }
            return default(Res);
        }

        //请求多返回提供器
        public void RequestMultProvider<Res>(string providerType, Action<Res> handler)
        {
            List<BaseCommand> delegate2;
            if (this.m_theRouter.TryGetValue(providerType, out delegate2))
            {
                for (int i = 0; i < delegate2.Count; i++)
                {
                    try
                    {
                        if (delegate2[i] is MultProvidCommand<Res>)
                        {
                            var res = ((MultProvidCommand<Res>)delegate2[i]).act();
                            if(res != null)
                                handler(res);
                        }
                        else
                        {
                            throw new EventException(string.Format("请求多值提供器 {0} 错误: 参数错误.", providerType));
                        }
                    }
                    catch (Exception exception)
                    {
                        Debug.unityLogger.LogException(exception);
                    }
                }
            }
        }
        public void RequestMultProvider<A1, Res>(string providerType, Action<Res> handler, A1 arg1)
        {
            List<BaseCommand> delegate2;
            if (this.m_theRouter.TryGetValue(providerType, out delegate2))
            {
                for (int i = 0; i < delegate2.Count; i++)
                {
                    try
                    {
                        if (delegate2[i] is MultProvidCommand<A1, Res>)
                        {
                            var res = ((MultProvidCommand<A1, Res>)delegate2[i]).act(arg1);
                            if (res != null)
                                handler(res);
                        }
                        else
                        {
                            throw new EventException(string.Format("请求多值提供器 {0} 错误: 参数错误.", providerType));
                        }
                    }
                    catch (Exception exception)
                    {
                        Debug.unityLogger.LogException(exception);
                    }
                }
            }
        }
        public void RequestMultProvider<A1, A2, Res>(string providerType, Action<Res> handler, A1 arg1, A2 arg2)
        {
            List<BaseCommand> delegate2;
            if (this.m_theRouter.TryGetValue(providerType, out delegate2))
            {
                for (int i = 0; i < delegate2.Count; i++)
                {
                    try
                    {
                        if (delegate2[i] is MultProvidCommand<A1, A2, Res>)
                        {
                            var res = ((MultProvidCommand<A1, A2, Res>)delegate2[i]).act(arg1, arg2);
                            if (res != null)
                                handler(res);
                        }
                        else
                        {
                            throw new EventException(string.Format("请求多值提供器 {0} 错误: 参数错误.", providerType));
                        }
                    }
                    catch (Exception exception)
                    {
                        Debug.unityLogger.LogException(exception);
                    }
                }
            }
        }
        public void RequestMultProvider<A1, A2, A3, Res>(string providerType, Action<Res> handler, A1 arg1, A2 arg2, A3 arg3)
        {
            List<BaseCommand> delegate2;
            if (this.m_theRouter.TryGetValue(providerType, out delegate2))
            {
                for (int i = 0; i < delegate2.Count; i++)
                {
                    try
                    {
                        if (delegate2[i] is MultProvidCommand<A1, A2, A3, Res>)
                        {
                            var res = ((MultProvidCommand<A1, A2, A3, Res>)delegate2[i]).act(arg1, arg2, arg3);
                            if (res != null)
                                handler(res);
                        }
                        else
                        {
                            throw new EventException(string.Format("请求多值提供器 {0} 错误: 参数错误.", providerType));
                        }
                    }
                    catch (Exception exception)
                    {
                        Debug.unityLogger.LogException(exception);
                    }
                }
            }
        }
        public void RequestMultProvider<A1, A2, A3, A4, Res>(string providerType, Action<Res> handler, A1 arg1, A2 arg2, A3 arg3, A4 arg4)
        {
            List<BaseCommand> delegate2;
            if (this.m_theRouter.TryGetValue(providerType, out delegate2))
            {
                for (int i = 0; i < delegate2.Count; i++)
                {
                    try
                    {
                        if (delegate2[i] is MultProvidCommand<A1, A2, A3, A4, Res>)
                        {
                            var res = ((MultProvidCommand<A1, A2, A3, A4, Res>)delegate2[i]).act(arg1, arg2, arg3, arg4);
                            if (res != null)
                                handler(res);
                        }
                        else
                        {
                            throw new EventException(string.Format("请求多值提供器 {0} 错误: 参数错误.", providerType));
                        }
                    }
                    catch (Exception exception)
                    {
                        Debug.unityLogger.LogException(exception);
                    }
                }
            }
        }
        #endregion

        #region 公开方法
        //移除某个委托队列
        public void RemoveEvent(string eventType)
        {
            if (m_theRouter.ContainsKey(eventType))
            {
                var temp = m_theRouter[eventType];
                m_theRouter.Remove(eventType);
                temp = null;
            }
        }
        public void CleanUp()
        {
            List<string> list = new List<string>();
            foreach (var pair in this.m_theRouter)
            {
                bool flag = false;
                foreach (string str in this.m_permanentEvents)
                {
                    if (pair.Key == str)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    list.Add(pair.Key);
                }
            }
            for (int i = 0; i < list.Count; i++)
            {
                this.m_theRouter.Remove(list[i]);
            }
        }
        public void CleanUp(string eventName)
        {
            List<string> list = new List<string>();
            bool flag = false;
            foreach (string str in this.m_permanentEvents)
            {
                if (eventName == str)
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                list.Add(eventName);
            }
            for (int i = 0; i < list.Count; i++ )
            {
                this.m_theRouter.Remove(list[i]);
            }
        }
        public bool ContainsEvent(string eventType)
        {
            return this.m_theRouter.ContainsKey(eventType);
        }
        //设为常驻事件
        public void MarkAsPermanent(string eventType)
        {
            this.m_permanentEvents.Add(eventType);
        }
        #endregion

        #region 私有方法
        //移除普通事件
        private void OnListenerAdding(string eventType, Delegate listenerBeingAdded)
        {
            if (!this.m_theRouter.ContainsKey(eventType))
            {
                this.m_theRouter.Add(eventType, new List<BaseCommand>());
            }
        }
        private void OnListenerRemoved(string eventType)
        {
            if (m_theRouter.ContainsKey(eventType) && (m_theRouter[eventType] == null || m_theRouter[eventType].Count == 0))
            {
                this.m_theRouter.Remove(eventType);
            }
        }
        private bool OnListenerRemoving(string eventType, Delegate listenerBeingRemoved)
        {
            if (!this.m_theRouter.ContainsKey(eventType))
            {
                return false;
            }
            return true;
        }
        //移除单返回提供器
        private bool OnSingleProviderAdding(string providerType, Delegate providerBeingAdded)
        {
            if (!this.m_theRouter.ContainsKey(providerType))
            {
                this.m_theRouter.Add(providerType, null);
                var delegate2 = this.m_theRouter[providerType];
                return true;
            }
            else
            {
                throw new EventException("一个SingleProvider只能绑定一个方法，当前方法名为" + providerType);
            }
        }
        private bool OnSingleProviderRemoving(string providerType)
        {
            if (!this.m_theRouter.ContainsKey(providerType))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private void OnSingleProviderRemoved(string providerType)
        {
            OnListenerRemoved(providerType);
        }
        //移除多返回提供器
        private void OnMultProviderAdding(string providerType, Delegate providerBeingAdded)
        {
            //实现与Listener一致
            OnListenerAdding(providerType, providerBeingAdded);
        }
        private bool OnMultProviderRemoving(string providerType, Delegate providerBeingAdded)
        {
            if (!this.m_theRouter.ContainsKey(providerType))
            {
                return false;
            }
            return true;
        }
        private void OnMultProviderRemoved(string providerType)
        {
            OnListenerRemoved(providerType);
        }
        #endregion

    }

    public class EventException : Exception
    {

        public EventException(string message)
            : base(message)
        {
        }

        public EventException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
