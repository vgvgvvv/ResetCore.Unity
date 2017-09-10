using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ResetCore.Event
{
    public class Binding<T, V>
    {
        /// <summary>
        /// 事件发起者
        /// </summary>
        public EventProperty<T> propertyA { get; private set; }
        /// <summary>
        /// 事件接受者
        /// </summary>
        public EventProperty<V> propertyB { get; private set; }

        /// <summary>
        /// 属性A流动到B时触发的事件
        /// </summary>
        public BaseCommand a2bCommand { get; private set; }
        /// <summary>
        /// 属性B流动到A时触发的事件
        /// </summary>
        public BaseCommand b2aCommand { get; private set; }

        Func<T, V> a2bConverter;
        Func<V, T> b2aConverter;

        private Binding() { }

        /// <summary>
        /// 绑定UI
        /// </summary>
        /// <returns></returns>
        public static Binding<T, V> Bind(EventProperty<T> propertyA, EventProperty<V> propertyB,
            Func<T, V> a2bConverter = null, Func<V, T> b2aConverter = null)
        {
            Binding<T, V> binding = new Binding<T, V>();

            binding.propertyA = propertyA;
            binding.propertyB = propertyB;

            binding.a2bConverter = a2bConverter ?? binding.DefaultA2BConverter;
            binding.b2aConverter = b2aConverter ?? binding.DefaultB2AConverter;

            binding.a2bCommand = propertyA.GetSetListenable().Listen(binding.A2B);
            binding.b2aCommand = propertyB.GetSetListenable().Listen(binding.B2A);

            return binding;
        }

        /// <summary>
        /// 绑定UI
        /// </summary>
        /// <returns></returns>
        public static Binding<T, V> Bind(EventProperty<T> propertyA, EventProperty<V> propertyB, 
            IValueConverter<T, V> converter)
        {
            Binding<T, V> binding = new Binding<T, V>();

            binding.propertyA = propertyA;
            binding.propertyB = propertyB;

            binding.a2bConverter = converter.Convert;
            binding.b2aConverter = converter.ConvertBack;

            propertyA.GetSetListenable().Listen(binding.A2B);
            propertyB.GetSetListenable().Listen(binding.B2A);

            return binding;
        }

        /// <summary>
        /// 解除绑定
        /// </summary>
        /// <param name="fromEvent"></param>
        public void Unbind()
        {
            propertyA.GetGetListenable().RemoveListen(A2B);
            propertyB.GetSetListenable().RemoveListen(B2A);
        }

        private void A2B(T arg)
        {
            propertyB.propValue = a2bConverter(arg);
        }

        private void B2A(V arg)
        {
            propertyA.propValue = b2aConverter(arg);
        }

        /// <summary>
        /// 默认转换器
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private V DefaultA2BConverter(T arg)
        {
            return (V)(arg as object);
        }

        /// <summary>
        /// 默认转换器
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private T DefaultB2AConverter(V arg)
        {
            return (T)(arg as object);
        }

    }
}

