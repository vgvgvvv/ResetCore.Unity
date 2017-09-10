using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResetCore.Util
{
    /// <summary>
    /// 同步Pass
    /// </summary>
    public abstract class BasePass
    {

        public abstract object Handle(object input);
        
    }

    /// <summary>
    /// 同步泛型Pass
    /// </summary>
    /// <typeparam name="I"></typeparam>
    /// <typeparam name="O"></typeparam>
    public abstract class BasePass<I, O> : BasePass
    {

        public abstract O HandlePass(I input);

        public sealed override object Handle(object input)
        {
            return HandlePass((I)input);
        }
    }

    /// <summary>
    /// 通用同步Pass
    /// </summary>
    /// <typeparam name="I"></typeparam>
    /// <typeparam name="O"></typeparam>
    public class CommonPass<I, O> : BasePass<I, O>
    {
        private Func<I, O> func;
        public CommonPass(Func<I, O> func)
        {
            this.func = func;
        }

        public override O HandlePass(I input)
        {
            return func(input);
        }
    }

    /// <summary>
    /// 异步Pass
    /// </summary>
    public abstract class BaseAysnPass : BasePass
    {
        public abstract void Handle(object input, Action<object> outputHandler);
        public override object Handle(object input)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 异步泛型Pass
    /// </summary>
    /// <typeparam name="I"></typeparam>
    /// <typeparam name="O"></typeparam>
    public abstract class BaseAysnPass<I, O> : BaseAysnPass
    {
        public abstract void HandlePass(I input, Action<object> outputHandler);

        public override void Handle(object input, Action<object> outputHandler)
        {
            HandlePass((I)input, outputHandler);
        }
    }

    /// <summary>
    /// 通用异步Pass
    /// </summary>
    /// <typeparam name="I"></typeparam>
    /// <typeparam name="O"></typeparam>
    public class CommonAysnPass<I, O> : BaseAysnPass<I, O>
    {
        private Action<I, Action<object>> act;
        public CommonAysnPass(Action<I, Action<object>> act)
        {
            this.act = act;
        }

        public override void HandlePass(I input, Action<object> outputHandler)
        {
            act(input, outputHandler);
        }
    }
}
