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

    }
}
