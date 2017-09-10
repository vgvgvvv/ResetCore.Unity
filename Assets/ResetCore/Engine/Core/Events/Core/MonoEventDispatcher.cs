using UnityEngine;
using System.Collections;
using ResetCore.Event;
using System.Collections.Generic;
using System;
using ResetCore.Util;

namespace ResetCore.Event
{
    public class MonoEventDispatcher
    {

        public static Dictionary<object, EventController> monoEventControllerDict = new Dictionary<object, EventController>();
        /// <summary>
        /// 获得监听物体
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static EventController GetMonoController(object gameObject)
        {
            if (gameObject == null || gameObject.Equals(null))
            {
                Debug.LogError("未找到MonoController");
                RemoveMonoController(gameObject);
                return null;
            }

            if (!monoEventControllerDict.ContainsKey(gameObject))
            {
                monoEventControllerDict.Add(gameObject, new EventController());
            }
            return monoEventControllerDict[gameObject];
        }

        /// <summary>
        /// 移除特定监听物体
        /// </summary>
        /// <param name="gameObject"></param>
        public static void RemoveMonoController(object gameObject)
        {
            monoEventControllerDict.Remove(gameObject);
        }

        private static List<object> keyToRemove = new List<object>();
        private static List<object> dictKeys = new List<object>();
        /// <summary>
        /// 对所有的MonoController
        /// </summary>
        /// <param name="act"></param>
        public static void DoToAllMonoContorller(Action<EventController> act)
        {
            keyToRemove.Clear();
            dictKeys.Clear();
            foreach (var kvp in monoEventControllerDict)
            {
                if (kvp.Key == null || kvp.Key.Equals(null))
                {
                    keyToRemove.Add(kvp.Key);
                    continue;
                }
                else
                {
                    dictKeys.Add(kvp.Key);
                }
            }

            for (int i = 0; i < dictKeys.Count; i++)
            {
                act(monoEventControllerDict[dictKeys[i]]);
            }

            foreach (var key in keyToRemove)
            {
                RemoveMonoController(key);
            }
        }
    }

    public static class MonoEventEx
    {
        /// <summary>
        /// 添加监听器
        /// </summary>
        /// <param name="bindObject"></param>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public static void AddEventListener(this GameObject bindObject, string eventType, Action handler)
        {
            if (bindObject == null) return;
            MonoEventDispatcher.GetMonoController(bindObject).AddEventListener(eventType, handler, bindObject);
            bindObject.GetOrCreateComponent<MonoEventCleanUp>();
        }
        /// <summary>
        /// 添加监听器
        /// </summary>
        /// <param name="bindObject"></param>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public static void AddEventListener<T>(this GameObject bindObject, string eventType, Action<T> handler)
        {
            if (bindObject == null) return;
            MonoEventDispatcher.GetMonoController(bindObject).AddEventListener<T>(eventType, handler, bindObject);
            bindObject.GetOrCreateComponent<MonoEventCleanUp>();
        }
        /// <summary>
        /// 添加监听器
        /// </summary>
        /// <param name="bindObject"></param>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public static void AddEventListener<T, U>(this GameObject bindObject, string eventType, Action<T, U> handler)
        {
            if (bindObject == null) return;
            MonoEventDispatcher.GetMonoController(bindObject).AddEventListener<T, U>(eventType, handler, bindObject);
            bindObject.GetOrCreateComponent<MonoEventCleanUp>();
        }
        /// <summary>
        /// 添加监听器
        /// </summary>
        /// <param name="bindObject"></param>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public static void AddEventListener<T, U, V>(this GameObject bindObject, string eventType, Action<T, U, V> handler)
        {
            if (bindObject == null) return;
            MonoEventDispatcher.GetMonoController(bindObject).AddEventListener<T, U, V>(eventType, handler, bindObject);
            bindObject.GetOrCreateComponent<MonoEventCleanUp>();
        }
        /// <summary>
        /// 添加监听器
        /// </summary>
        /// <param name="bindObject"></param>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public static void AddEventListener<T, U, V, W>(this GameObject bindObject, string eventType, Action<T, U, V, W> handler)
        {
            if (bindObject == null) return;
            MonoEventDispatcher.GetMonoController(bindObject).AddEventListener<T, U, V, W>(eventType, handler, bindObject);
            bindObject.GetOrCreateComponent<MonoEventCleanUp>();
        }
    }

}

