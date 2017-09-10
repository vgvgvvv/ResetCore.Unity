using ResetCore.Event;
using ResetCore.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ResetCore.Event
{

    public class EventProptyGetEvent<T> : UnityEvent<T> { }
    public class EventProptySetEvent<T> : UnityEvent<T> { }

    public class EventProperty<T>
    {
        /// <summary>
        /// 获取事件
        /// </summary>
        private UnityEvent<T> get = new EventProptyGetEvent<T>();
        /// <summary>
        /// 改变事件
        /// </summary>
        private UnityEvent<T> set = new EventProptySetEvent<T>();

        /// <summary>
        /// 获取器
        /// </summary>
        private Func<T> getter = null;

        /// <summary>
        /// 设置器
        /// </summary>
        private Action<T> setter = null;

        private T _value;
        /// <summary>
        /// 值
        /// </summary>
        public T propValue
        {
            get
            {
                if (getter == null)
                {
                    if (get != null)
                        get.Invoke(_value);
                    return _value;
                }
                else
                {
                    if (get != null)
                        get.Invoke(getter());
                    return getter();
                }

            }
            set
            {
                if (setter == null)
                {
                    if (value == null)
                    {
                        _value = default(T);
                    }
                    else if (value.Equals(_value))
                    {
                        return;
                    }
                    else
                    {
                        _value = value;
                    }

                    if (set != null)
                        set.Invoke(_value);
                }
                else
                {

                    if (value == null)
                    {
                        setter(default(T));
                    }
                    else if (value.Equals(getter()))
                    {
                        return;
                    }
                    else
                    {
                        setter(value);
                    }
                    if (set != null)
                        set.Invoke(getter());
                }

            }
        }

        /// <summary>
        /// 初始化值
        /// </summary>
        public void Init()
        {
            if (set != null)
            {
                if (getter != null && setter != null)
                {
                    set.Invoke(getter());
                }
                else
                {
                    set.Invoke(_value);
                }
            }
        }

        public EventProperty() { }

        public EventProperty(T value)
        {
            this._value = value;
        }

        public EventProperty(Func<T> getter, Action<T> setter, 
            UnityEvent<T> overrideGetEvent = null, UnityEvent<T> overrideSetEvent = null)
        {
            if (overrideGetEvent != null)
                get = overrideGetEvent;
            if (overrideSetEvent != null)
                set = overrideSetEvent;

            this.getter = getter;
            this.setter = setter;
        }

        /// <summary>
        /// 作为被监听者获取GetListenable
        /// </summary>
        /// <returns></returns>
        public Listenable<T> GetGetListenable()
        {
            return get.GetListenable<T>();
        }

        /// <summary>
        /// 作为被监听者获取SetListenable
        /// </summary>
        /// <returns></returns>
        public Listenable<T> GetSetListenable()
        {
            return set.GetListenable<T>();
        }
        
        #region 监听事件
        /// <summary>
        /// 作为监听者监听事件
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public BaseCommand ListenEvent(string eventName, Func<T> func)
        {
            return EventDispatcher.AddEventListener(eventName, () => this.propValue = func());
        }

        /// <summary>
        /// 作为监听者监听事件
        /// </summary>
        public BaseCommand ListenEvent<A1>(string eventName, Func<A1, T> func)
        {
            return EventDispatcher.AddEventListener<A1>(eventName, (arg1) => this.propValue = func(arg1));
        }

        /// <summary>
        /// 作为监听者监听事件
        /// </summary>
        public BaseCommand ListenEvent<A1, A2>(string eventName, Func<A1, A2, T> func)
        {
            return EventDispatcher.AddEventListener<A1, A2>(eventName, (arg1, arg2) => this.propValue = func(arg1, arg2));
        }

        /// <summary>
        /// 作为监听者监听事件
        /// </summary>
        public BaseCommand ListenEvent<A1, A2, A3>(string eventName, Func<A1, A2, A3, T> func)
        {
            return EventDispatcher.AddEventListener<A1, A2, A3>(eventName, (arg1, arg2, arg3) => this.propValue = func(arg1, arg2, arg3));
        }
        #endregion 监听事件


        #region 绑定属性
        /// <summary>
        /// 将两个属性之间进行绑定
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="prop"></param>
        /// <returns></returns>
        public Binding<T, V> Bind<V>(EventProperty<V> prop)
        {
            return Binding<T, V>.Bind(this, prop);
        }

        /// <summary>
        /// 将两个属性之间进行绑定
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="prop"></param>
        /// <returns></returns>
        public Binding<T, V> Bind<V>(EventProperty<V> prop, IValueConverter<T, V> converter)
        {
            return Binding<T, V>.Bind(this, prop, converter);
        }
        #endregion 绑定属性
    }

}
