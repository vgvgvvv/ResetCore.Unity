using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public abstract class IRefrenceObject<T> : IDisposable where T: IRefrenceObject<T>{

    /// <summary>
    /// 引用数量
    /// </summary>
    public int refNum
    {
        get
        {
            return referenceList.Count;
        }
    }

    private System.Object lockObject = new object();

    /// <summary>
    /// 依赖列表
    /// </summary>
    public HashSet<T> referenceList
    {
        get;
        private set;
    }

    /// <summary>
    /// 添加引用
    /// </summary>
	public void AddRef(T obj)
    {
        lock (lockObject)
        {
            referenceList.Add(obj);
        }
    }

    /// <summary>
    /// 移除引用
    /// </summary>
    public void RemoveRef(T obj)
    {
        lock (lockObject)
        {
            referenceList.Remove(obj);
        }

    }

    /// <summary>
    /// 如果不被任何对象引用则销毁该对象
    /// </summary>
    public void Dispose()
    {
        if(referenceList.Count == 0)
        {
            DoDispose();
        }
    }

    public abstract void DoDispose();
}
