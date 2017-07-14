#if DATA_GENER
using UnityEngine;
using System.Collections;
using ResetCore.Data.GameDatas.Xml;
using System.Collections.Generic;
using System;
using ResetCore.Util;

namespace ResetCore.GameSystem
{
    public enum BuffType
    {
        Add,
        Mult,
        Other
    }

    public abstract class BaseBuff<T>
    {


        public abstract BuffType type { get; }
        public BuffManager<T> manager { protected get; set; }

        protected BaseBuff() { }

        public float buffTime { get; set; }
        public Action removeCallback { get; set; }
        public CoroutineTaskManager.CoroutineTask task { get; set; }

        public abstract void BuffFunc(T effectObject);
        public virtual void RemoveBuffFunc(T effectObject) { }
    }
}

#endif