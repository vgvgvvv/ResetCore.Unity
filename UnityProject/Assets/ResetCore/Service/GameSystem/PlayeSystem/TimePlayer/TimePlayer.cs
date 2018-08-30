using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResetCore.GameSystem
{
    public abstract class TimePlayer : BasePlayer<TimePlayerData>
    {
        /// <summary>
        /// 持续的时间
        /// </summary>
        public float duration { get; private set; }

        public override void Init(TimePlayerData playerData, BasePlayer[] playerList = null)
        {
            base.Init(playerData, playerList);
            duration = playerData.duration;
        }

        protected override bool CheckEnd()
        {
            return pastTime >= duration;
        }
    }
}
