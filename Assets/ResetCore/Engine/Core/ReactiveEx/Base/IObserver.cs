using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResetCore.Rx
{
    public interface IObserver<T>
    {
        void OnCompleted();
        void OnError(Exception error);
        void OnNext(T value);
    }
}
