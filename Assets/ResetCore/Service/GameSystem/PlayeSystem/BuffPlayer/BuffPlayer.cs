using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResetCore.GameSystem
{
    public class BuffPlayer : BasePlayer<BasePlayerData>
    {
        protected override void OnStart()
        {
            Debug.Log("Start");
        }

        protected override void OnUpdate()
        {
            Debug.Log("pastTime:" + pastTime);
        }

        protected override void OnEnd()
        {
            Debug.Log("End");
        }
    }
}
