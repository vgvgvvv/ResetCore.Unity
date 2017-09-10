using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace ResetCore.GameSystem
{
    public class BasePlayerData
    {
        public float speed = 1;
        public float delayTime = 0;

        public BasePlayerData(float speed, float delayTime)
        {
            this.speed = speed;
            this.delayTime = delayTime;
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="TPlayer"></typeparam>
        /// <param name="player"></param>
        /// <returns></returns>
        public string ToJson()
        {
            return JsonMapper.ToJson(this);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public TData FromJson<TData>(string json) where TData : BasePlayerData
        {
            return JsonMapper.ToObject<TData>(json);
        }
    }
}
