using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Linq;

public class TheardHelper : MonoBehaviour
{
    public static int maxThreads = 8;
    static int numThreads;

    private static TheardHelper _current;
    public static TheardHelper Current
    {
        get
        {
            Initialize();
            return _current;
        }
    }

    void Awake()
    {
        _current = this;
        initialized = true;
    }

    static bool initialized;

    static void Initialize()
    {
        if (!initialized)
        {

            if (!Application.isPlaying)
                return;
            initialized = true;
            var g = new GameObject("TheardHelper");
            _current = g.AddComponent<TheardHelper>();
        }

    }

    private List<Action> _actions = new List<Action>();
    public struct DelayedQueueItem
    {
        public float time;
        public Action action;
    }
    private List<DelayedQueueItem> _delayed = new List<DelayedQueueItem>();

    List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem>();

    //在主线程上执行
    public static void QueueOnMainThread(Action action)
    {
        QueueOnMainThread(action, 0f);
    }
    public static void QueueOnMainThread(Action action, float time)
    {
        if (time != 0)
        {
            lock (Current._delayed)
            {
                Current._delayed.Add(new DelayedQueueItem { time = Time.time + time, action = action });
            }
        }
        else
        {
            lock (Current._actions)
            {
                Current._actions.Add(action);
            }
        }
    }
    
    //在子线程中执行
    public static Thread RunAsync(Action a)
    {
        Initialize();
        //线程过多
        while (numThreads >= maxThreads)
        {
            Thread.Sleep(1);
        }
        //以原子操作的模式递增：numThreads++
        Interlocked.Increment(ref numThreads);
        ThreadPool.QueueUserWorkItem(RunAction, a);
        return null;
    }

    private static void RunAction(object action)
    {
        try
        {
            ((Action)action)();
        }
        catch
        {
        }
        finally
        {
            //以原子操作的模式递减：numThreads--
            Interlocked.Decrement(ref numThreads);
        }

    }


    void OnDisable()
    {
        if (_current == this)
        {

            _current = null;
        }
    }

    List<Action> _currentActions = new List<Action>();

    // 在主线程上执行
    void Update()
    {
        //加入动作
        lock (_actions)
        {
            _currentActions.Clear();
            _currentActions.AddRange(_actions);
            _actions.Clear();
        }
        //遍历动作并且执行
        foreach (var a in _currentActions)
        {
            a();
        }

        //加入延时动作
        lock (_delayed)
        {
            _currentDelayed.Clear();
            _currentDelayed.AddRange(_delayed.Where(d => d.time <= Time.time));
            foreach (var item in _currentDelayed)
                _delayed.Remove(item);
        }
        //遍历并且执行
        foreach (var delayed in _currentDelayed)
        {
            delayed.action();
        }
    }
}