using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResetCore.Rx
{
    public interface IObservable<T>
    {
        IDisposable Subscribe(IObserver<T> observer);
    }

}
