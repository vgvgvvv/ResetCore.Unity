using System.Collections;
using System.Collections.Generic;
using ResetCore.Util;
using UnityEngine;
using LitJson;
using ResetCore.Data;
using ResetCore.Json;

namespace ResetCore.GameSystem
{
    public enum PlayerState
    {
        NotStarted,
        Running,
        Finished
    }

    public abstract class BasePlayer
    {
        /// <summary>
        /// Player运行状态
        /// </summary>
        [SerializeIgnore]
        public PlayerState state { get; protected set; }

        /// <summary>
        /// 经过的时间
        /// </summary>
        [SerializeIgnore]
        public float pastTime { get; protected set; }

        /// <summary>
        /// 作为子播放器时的开始时间
        /// </summary>
        public float delayTime { get; protected set; }

        /// <summary>
        /// 播放的速度
        /// </summary>
        public float speed { get; protected set; }

        /// <summary>
        /// 子播放器
        /// </summary>
        public BasePlayer[] childPlayerList { get; protected set; }

        private ReCoroutineTaskManager.CoroutineTask coroutine;


        #region 公开函数
        /// <summary>
        /// 开始
        /// </summary>
        public virtual void Start()
        {
            state = PlayerState.Running;
            pastTime = 0;
            OnStart();
            coroutine = ReCoroutineTaskManager.Instance.AddTask(Update());
        }

        /// <summary>
        /// 停止
        /// </summary>
        public virtual void Stop()
        {
            End();
            coroutine.Stop();
        }
        #endregion 公开函数


        #region 私有函数
        protected IEnumerator<float> Update()
        {
            while (true)
            {
                if (state == PlayerState.NotStarted)
                {

                }
                else if (state == PlayerState.Finished)
                {
                    break;
                }
                else
                {
                    pastTime += Time.deltaTime * speed;

                    for (int i = 0; i < childPlayerList.Length; i++)
                    {
                        if (childPlayerList[i].delayTime <= pastTime && childPlayerList[i].state == PlayerState.NotStarted)
                        {
                            childPlayerList[i].Start();
                        }
                    }

                    OnUpdate();
                    if (CheckEnd())
                    {
                        End();
                    }
                }
                yield return 0;
            }
        }

        private void End()
        {
            if (state == PlayerState.Running)
            {
                for (int i = 0; i < childPlayerList.Length; i++)
                {
                    childPlayerList[i].Stop();
                }
                OnEnd();
            }
            state = PlayerState.Finished;
        }
        #endregion 私有函数


        #region 可重写函数

        /// <summary>
        /// 检查是否已经结束
        /// </summary>
        /// <returns></returns>
        protected virtual bool CheckEnd()
        {
            for (int i = 0; i < childPlayerList.Length; i++)
            {
                if (childPlayerList[i].state != PlayerState.Finished)
                    return false;
            }
            return true;
        }

        protected abstract void OnStart();

        protected abstract void OnUpdate();

        protected abstract void OnEnd();

        #endregion 可重写函数
    }

    public abstract class BasePlayer<T> : BasePlayer where T : BasePlayerData
    {

        /// <summary>
        /// Player相关数据
        /// </summary>
        public T data { get; private set; }


        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Init(T playerData, BasePlayer[] playerList = null)
        {
            state = PlayerState.NotStarted;
            data = playerData;
            speed = data.speed;
            delayTime = data.delayTime;
            childPlayerList = playerList??new BasePlayer[0];
        }

    }
}
