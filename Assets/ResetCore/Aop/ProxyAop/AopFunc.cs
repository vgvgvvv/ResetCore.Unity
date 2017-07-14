using UnityEngine;
using System.Collections;
using System.Runtime.Remoting.Messaging;
using System;

public abstract class AopFunc {

    public abstract bool CanExecute(IMethodCallMessage msg);

    public virtual object Execute(Func<object> act)
    {
        return act();
    }
}
