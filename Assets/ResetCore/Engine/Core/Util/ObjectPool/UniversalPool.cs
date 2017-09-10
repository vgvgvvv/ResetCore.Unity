using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResetCore.Util
{
    public class UniversalPool<T>
    {
        private UniversalPool() { }

        List<T> pool = new List<T>();

        Func<T> factory;
        Action<T> destroyAct;
        Action<T> onGet;
        Action<T> onReturn;

        /// <summary>
        /// 创建池
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="destroyAct"></param>
        /// <param name="onGet"></param>
        /// <param name="onReturn"></param>
        /// <param name="startSize"></param>
        /// <returns></returns>
        public static UniversalPool<T> Create(Func<T> factory, Action<T> destroyAct
            , Action<T> onGet = null, Action<T> onReturn = null, int startSize = 0)
        {
            var uniPool = new UniversalPool<T>();
            uniPool.factory = factory;
            uniPool.destroyAct = destroyAct;
            uniPool.onGet = onGet;
            uniPool.onReturn = onReturn;
            uniPool.Init(startSize);
            return uniPool;
        }

        private void Init(int startSize)
        {
            for(int i = 0; i < startSize; i++)
            {
                var obj = factory();
                pool.Add(obj);
                if (onReturn != null)
                    onReturn(obj);
            }
        }

        /// <summary>
        /// 从池中获取
        /// </summary>
        /// <returns></returns>
        public T Get()
        {
            T res;
            if (pool.Count > 0)
            {
                res = pool[pool.Count - 1];
                pool.Remove(res);
            }
            else
            {
                res = factory();
            }
            if(onGet != null)
                onGet(res);
            return res;

        }

        /// <summary>
        /// 归还池
        /// </summary>
        /// <param name="obj"></param>
        public void Return(T obj)
        {
            if (onReturn != null)
                onReturn(obj);
            pool.Add(obj);
        }

        /// <summary>
        /// 销毁池
        /// </summary>
        public void Destroy()
        {
            for(int i = 0; i < pool.Count; i++)
            {
                destroyAct(pool[i]);
            }
        }
        
    }

}
