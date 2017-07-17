using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResetCore.Util
{
    public class ThreadManager : MonoSingleton<ThreadManager>
    {
        //public readonly int MaxThread = 8;
        //private int numThreads;

        //private List<Action> _actions = new List<Action>();
        //public struct DelayedQueueItem
        //{
        //    public float time;
        //    public Action action;
        //}

        //private List<DelayedQueueItem> _delayed = new List<DelayedQueueItem>();

        //List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem>();

        //public void Init()
        //{
            
        //}

        //public void RunOnMainThread(Action action)
        //{
        //    RunOnMainThread(action, 0);
        //}

        //public void RunOnMainThread(Action action, float time)
        //{
        //    if (time != 0)
        //    {
        //        lock (_delayed)
        //        {
        //            _delayed.Add(
        //                new DelayedQueueItem
        //                {
        //                    time = Time.realtimeSinceStartup + time,
        //                    action = action
        //                });
        //        }
        //    }
        //    else
        //    {
        //        lock (_actions)
        //        {
        //            _actions.Add(action);
        //        }
        //    }
        //}

        //void Update()
        //{
            
        //}
    }
}

