using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResetCore.SAsset
{
    public abstract class RefObject
    {
        /// <summary>
        /// 引用计数
        /// </summary>
        public int refCount { get; private set; }

        public void Retain()
        {
            refCount++;
        }

        public void Release()
        {
            refCount--;
        }

        public abstract void Dispose();

    }

}
