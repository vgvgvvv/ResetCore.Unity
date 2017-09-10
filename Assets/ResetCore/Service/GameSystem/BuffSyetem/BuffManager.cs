#if DATA_GENER
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ResetCore.Util;
using System;
using ResetCore.Event;
using ResetCore.Data.GameDatas.Xml;

namespace ResetCore.GameSystem
{
    public class BuffManager<T>
    {
        private Dictionary<BuffType, List<BaseBuff<T>>> buffListDict;

        private List<BaseBuff<T>> addBuffList = new List<BaseBuff<T>>();
        private List<BaseBuff<T>> multBuffList = new List<BaseBuff<T>>();
        private List<BaseBuff<T>> otherBuffList = new List<BaseBuff<T>>();

        private Dictionary<System.Type, BaseBuff<T>> buffInstanceDict = new Dictionary<System.Type, BaseBuff<T>>();
        private V GetBuff<V>(BuffManager<T> manager) where V : BaseBuff<T>
        {
            Type buffType = typeof(V);
            if (buffInstanceDict.ContainsKey(buffType))
            {
                return buffInstanceDict[buffType] as V;
            }
            else
            {
                V buff = Activator.CreateInstance<V>();
                buff.manager = manager;
                //buff.buffTime = BuffData.Select((data) =>
                //{
                //    return data.BuffName == buffType.Name;
                //}).BuffTime;
                buffInstanceDict.Add(buffType, buff);
                return buffInstanceDict[buffType] as V;
            }
        }

        public T effectedObject { get; private set; }
        private System.Action initAct;

        public BuffManager(T effectedObject, System.Action initAct)
        {
            buffListDict = new Dictionary<BuffType, List<BaseBuff<T>>>()
        {
            {BuffType.Add, addBuffList },
            {BuffType.Mult, multBuffList },
            {BuffType.Other, otherBuffList },
        };
            this.effectedObject = effectedObject;
            this.initAct = initAct;
        }

        public void InitProperty()
        {
            initAct();
        }

        //添加Buff
        public void AddBuff<V>(Action callBack = null) where V : BaseBuff<T>
        {
            V buff = GetBuff<V>(this);
            List<BaseBuff<T>> buffList = buffListDict[buff.type];
            //添加buff
            if (buffList.Contains(buff))
            {
                RemoveBuff<V>();
            }
            buffList.Add(buff);
            //添加时间
            if (buff.buffTime <= 0) return;

            buff.removeCallback = () =>
            {

                if (!buffList.Contains(buff)) return;
                if (effectedObject == null) return;

                buff.RemoveBuffFunc(effectedObject);
                buffList.Remove(buff);

                if (callBack != null)
                    callBack();
                Recalculate();

                if (buff.task != null)
                    buff.task.Stop();
            };

            buff.task = CoroutineTaskManager.Instance.WaitSecondTodo(buff.removeCallback, buff.buffTime);

            Recalculate();

        }
        //删除Buff
        public void RemoveBuff<V>() where V : BaseBuff<T>
        {
            V buff = GetBuff<V>(this);
            List<BaseBuff<T>> buffList = buffListDict[buff.type];
            if (buffList.Contains(buff))
            {
                buff.removeCallback();
            }
            Recalculate();
        }

        public void Recalculate()
        {
            if (effectedObject == null) return;
            InitProperty();
            foreach (BaseBuff<T> buff in addBuffList)
            {
                buff.BuffFunc(effectedObject);
            }
            foreach (BaseBuff<T> buff in multBuffList)
            {
                buff.BuffFunc(effectedObject);
            }
            foreach (BaseBuff<T> buff in otherBuffList)
            {
                buff.BuffFunc(effectedObject);
            }
        }



        public void ClearAllBuff()
        {
            InitProperty();
        }

    }
}
#endif