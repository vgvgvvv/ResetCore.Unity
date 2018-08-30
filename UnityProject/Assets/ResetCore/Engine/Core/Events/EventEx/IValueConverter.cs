using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResetCore.Event
{
    public interface IValueConverter<T, V>
    {

        /// <summary>
        /// 从后者转换到前者
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        V Convert(T arg);

        /// <summary>
        /// 从前者转换到后者
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        T ConvertBack(V arg);
    }

}
