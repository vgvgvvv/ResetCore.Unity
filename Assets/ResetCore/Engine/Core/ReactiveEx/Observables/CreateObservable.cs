using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResetCore.Rx
{
    public class CreateObservable<T> : IObservable<T>
    {
        readonly Func<IObserver<T>, IDisposable> subscribe;

        public CreateObservable(Func<IObserver<T>, IDisposable> subscribe)
        {
            this.subscribe = subscribe;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return subscribe(observer);
        }

    }

}
