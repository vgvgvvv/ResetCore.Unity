using ResetCore.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ResetCore.Event
{
    public static class UIEventEx
    {

        #region Text

        /// <summary>
        /// 绑定UI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text"></param>
        /// <param name="property"></param>
        /// <param name="act"></param>
        public static BaseCommand Bind<T>(this Text text, EventProperty<T> property, Action<Text, T> actOnPropertyChange = null)
        {
            if (actOnPropertyChange == null)
                actOnPropertyChange = (txt, str) => txt.text = str == null ? "" : str.ToString();

            var setListenable = property.GetSetListenable();
            return setListenable.Listen((str)=> actOnPropertyChange(text, str), text)
                .RemoveWhenDestroy(text.gameObject)
                .WhenMonobehaviorDestory(text.gameObject, (cmd) => { setListenable.RemoveAllListen(); });
        }

        /// <summary>
        /// 绑定属性
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="text"></param>
        /// <param name="property"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        public static Binding<string, V> Bind<V>(this Text text, EventProperty<V> property, IValueConverter<string, V> converter)
        {
            var binding = text.GetEventText().Bind(property, converter);
            text.gameObject.GetCallbacks().onDestroy += binding.Unbind;
            return binding;
        }
        /// <summary>
        /// 将Text中的text包装为事件属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text"></param>
        /// <returns></returns>
        public static EventProperty<string> GetEventText(this Text text)
        {
            var prop = new EventProperty<string>(() => { return text.text; }, (x) => { text.text = x; });
            return prop;
        }
        #endregion Text

        #region InputField

        // <summary>
        /// 绑定UI
        /// </summary>
        public static BaseCommand[] Bind<T>(this InputField text, EventProperty<T> property, 
            Action<InputField, EventProperty<T>> actOnChanged = null, Action<InputField, T> actOnValueChange = null)
        {
            if (actOnChanged == null)
                actOnChanged = (txt, str) => property.propValue = txt.text.GetValue<T>();

            if (actOnValueChange == null)
                actOnValueChange = (txt, str) => txt.text = str == null ? "" : str.ToString();

            BaseCommand[] commands = new BaseCommand[2];

            var getListenable = text.onValueChanged.GetListenable();
            commands[0] = getListenable.Listen((str) =>
            {
                actOnChanged(text, property);
            }, text)
                .RemoveWhenDestroy(text.gameObject)
                .WhenMonobehaviorDestory(text.gameObject, (cmd)=> { getListenable.RemoveAllListen(); });

            var setListenable = property.GetSetListenable();
            commands[1] = setListenable.Listen((str) => actOnValueChange(text, str), text)
                .RemoveWhenDestroy(text.gameObject)
                .WhenMonobehaviorDestory(text.gameObject, (cmd) => { setListenable.RemoveAllListen(); });

            return commands;
        }

        /// <summary>
        /// 绑定属性
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="input"></param>
        /// <param name="property"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        public static Binding<string, V> Bind<V>(this InputField input, EventProperty<V> property, 
            IValueConverter<string, V> converter)
        {
            var binding = input.GetEventText().Bind(property, converter);
            input.gameObject.GetCallbacks().onDestroy += binding.Unbind;
            return binding;
        }

        /// <summary>
        /// 将InputField包装为事件属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text"></param>
        /// <returns></returns>
        public static EventProperty<string> GetEventText(this InputField intput)
        {
            var prop = new EventProperty<string>(() => { return intput.text; }, (x) => { intput.text = x; }, 
                null, intput.onValueChanged);
            return prop;
        }
        #endregion InputField

        #region image
        /// <summary>
        /// 绑定UI
        /// </summary>
        public static BaseCommand Bind<T>(this Image img, EventProperty<T> property, Action<Image, T> actOnPropertyValurChange)
        {
            var setListenable = property.GetSetListenable();
            return setListenable.Listen((str) => actOnPropertyValurChange(img, str), img)
                .RemoveWhenDestroy(img.gameObject)
                .WhenMonobehaviorDestory(img.gameObject, (cmd) => { setListenable.RemoveAllListen(); });
        }
        #endregion

        #region Button
        // <summary>
        /// 绑定UI
        /// </summary>
        public static BaseCommand Bind<T>(this Button btn, EventProperty<T> property, Action<Button, T> act)
        {
            var setListenable = property.GetSetListenable();
            return setListenable.Listen((str) => act(btn, str), btn)
                .RemoveWhenDestroy(btn.gameObject)
                .WhenMonobehaviorDestory(btn.gameObject, (cmd) => { setListenable.RemoveAllListen(); });
        }
        #endregion Button

    }

}
