using ResetCore.Event;
using ResetCore.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ResetCore.Event
{
    public class Listenable
    {
        /// <summary>
        /// 总表
        /// </summary>
        private static Dictionary<UnityEvent, Listenable> listenableDict = new Dictionary<UnityEvent, Listenable>();

        /// <summary>
        /// 获得Listenable
        /// </summary>
        /// <param name="eventToListen"></param>
        /// <returns></returns>
        public static Listenable GetListenable(UnityEvent eventToListen)
        {
            if (!listenableDict.ContainsKey(eventToListen))
            {
                listenableDict.Add(eventToListen, new Listenable(eventToListen));
            }
            return listenableDict[eventToListen];
        }

        //被监听物标识
        private string _listenableId;
        public string listenableId
        {
            get
            {
                if(string.IsNullOrEmpty(_listenableId))
                    _listenableId = "Listenable" + eventToListen.GetHashCode().ToString();
                return _listenableId;
            }
        }

        UnityEvent eventToListen;

        public Listenable(UnityEvent eventToListen)
        {
            this.eventToListen = eventToListen;

            eventToListen.AddListener(() =>
            {
                EventDispatcher.TriggerEvent(listenableId);
            });
        }

        /// <summary>
        /// 开始监听
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public BaseCommand Listen(Action callback)
        {
            return EventDispatcher.AddEventListener(listenableId, callback);
        }

        /// <summary>
        /// 转换为普通的字符串命令
        /// </summary>
        /// <param name="commandName"></param>
        /// <param name="bindObject"></param>
        /// <returns></returns>
        public BaseCommand Convert2StringCommand(string commandName, object bindObject = null)
        {
            return EventDispatcher.AddEventListener(listenableId, () =>
            {
                EventDispatcher.TriggerEventWithTag(commandName, bindObject);
            });
        }

        /// <summary>
        /// 移除指定监听者
        /// </summary>
        /// <param name="callback"></param>
        public void RemoveListen(Action callback)
        {
            EventDispatcher.RemoveEventListener(listenableId, callback);
            ///依旧还有别的地方在进行监听
            if (!EventDispatcher.ContainEvent(listenableId))
                listenableDict.Remove(eventToListen);
        }

        /// <summary>
        /// 移除所有监听者
        /// </summary>
        public void RemoveAllListen()
        {
            EventDispatcher.RemoveEvent(listenableId);
            if (!EventDispatcher.ContainEvent(listenableId))
                listenableDict.Remove(eventToListen);
        }

        /// <summary>
        /// 当销毁时卸载Listener
        /// </summary>
        /// <param name="go"></param>
        public void RemoveAllListenWhenDestroy(GameObject go)
        {
            go.GetCallbacks().onDestroy += RemoveAllListen;
        }
    }

    public class Listenable<T>
    {
        /// <summary>
        /// 总表
        /// </summary>
        private static Dictionary<UnityEvent<T>, Listenable<T>> listenableDict = new Dictionary<UnityEvent<T>, Listenable<T>>();

        /// <summary>
        /// 获得Listenable
        /// </summary>
        /// <param name="eventToListen"></param>
        /// <returns></returns>
        public static Listenable<T> GetListenable(UnityEvent<T> eventToListen)
        {
            if (!listenableDict.ContainsKey(eventToListen))
            {
                listenableDict.Add(eventToListen, new Listenable<T>(eventToListen));
            }
            return listenableDict[eventToListen];
        }


        //被监听物标识
        private string _listenableId;
        public string listenableId
        {
            get
            {
                if (string.IsNullOrEmpty(_listenableId))
                    _listenableId = "Listenable" + eventToListen.GetHashCode().ToString();
                return _listenableId;
            }
        }

        UnityEvent<T> eventToListen;

        public Listenable(UnityEvent<T> eventToListen)
        {
            this.eventToListen = eventToListen;

            eventToListen.AddListener((arg1) =>
            {
                EventDispatcher.TriggerEvent(listenableId, arg1);
            });
        }

        /// <summary>
        /// 开始监听
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public BaseCommand Listen(Action<T> callback, object bindObject = null)
        {
            return EventDispatcher.AddEventListener<T>(listenableId, callback, bindObject);
        }

        /// <summary>
        /// 转换为普通的字符串命令
        /// </summary>
        /// <param name="commandName"></param>
        /// <param name="bindObject"></param>
        /// <returns></returns>
        public BaseCommand Convert2StringCommand(string commandName, object bindObject = null)
        {
            return EventDispatcher.AddEventListener<T>(listenableId, (arg) =>
            {
                EventDispatcher.TriggerEventWithTag<T>(commandName, arg, bindObject);
            });
        }

        /// <summary>
        /// 移除指定监听者
        /// </summary>
        /// <param name="callback"></param>
        public void RemoveListen(Action<T> callback)
        {
            EventDispatcher.RemoveEventListener<T>(listenableId, callback);
            if (!EventDispatcher.ContainEvent(listenableId))
                listenableDict.Remove(eventToListen);
        }

        /// <summary>
        /// 移除所有监听者
        /// </summary>
        public void RemoveAllListen()
        {
            EventDispatcher.RemoveEvent(listenableId);
            if (!EventDispatcher.ContainEvent(listenableId))
                listenableDict.Remove(eventToListen);
        }
    }


    public static class ListenableEx
    {
        /// <summary>
        /// 转换为可监听物体
        /// </summary>
        /// <param name="eventToListen"></param>
        /// <returns></returns>
        public static Listenable GetListenable(this UnityEvent eventToListen)
        {
            return Listenable.GetListenable(eventToListen);
        }

        /// <summary>
        /// 转换为可监听物体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventToListen"></param>
        /// <returns></returns>
        public static Listenable<T> GetListenable<T>(this UnityEvent<T> eventToListen)
        {
            return Listenable<T>.GetListenable(eventToListen);
        }

        /// <summary>
        /// 转换为可监听对象
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static Listenable GetListenable(this ReCoroutineTaskManager.CoroutineTask task)
        {
            UnityEvent finishEvent = new UnityEvent();
            task.callBack += (bo) =>
            {
                finishEvent.Invoke();
            };
            return Listenable.GetListenable(finishEvent);
        }

    }
}

