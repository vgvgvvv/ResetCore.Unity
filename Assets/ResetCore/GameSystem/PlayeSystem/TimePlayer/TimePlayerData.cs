using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResetCore.GameSystem
{
    public class TimePlayerData : BasePlayerData
    {

        public float duration;

        public TimePlayerData(float speed, float delayTime, float duration) : base(speed, delayTime)
        {
            this.duration = duration;
        }
    }
}
