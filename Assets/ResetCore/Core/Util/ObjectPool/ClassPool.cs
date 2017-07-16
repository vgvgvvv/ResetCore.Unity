using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResetCore.Util
{
    public class ClassPool
    {

        //private Dictionary<Type, Stack<IStorable>> stackMap = new 
        
        

    }

    /// <summary>
    /// 可储存的对象
    /// </summary>
    public interface IStorable
    {
        bool isDirty { get; set; }
        void Reset();
        void StoreToPool();
    }
}
