using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResetCore.Data.GameDatas.Obj
{
    [Serializable]

    public class ObjDataBundle<T> : ScriptableObject where T : ObjData
    {
        public List<T> dataArray = new List<T>();
    }

   
}