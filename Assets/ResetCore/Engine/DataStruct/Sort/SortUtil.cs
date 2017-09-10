using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResetCore.DataStruct
{
    public static class SortUtil
    {

        public static bool Less(this IComparable v, IComparable w)
        {
            return v.CompareTo(w) < 0;
        }

        public static void Exch<T>(this List<T> list, int i, int j)
        {
            T t = list[i];
            list[i] = list[j];
            list[j] = t;
        }

        public static void Show<T>(this List<T> list)
        {
            StringBuilder str = new StringBuilder();
            foreach (T v in list)
            {
                str.Append(v.ToString() + " ");
            }
            Debug.unityLogger.Log(str.ToString());
        }

        public static bool IsSorted(this List<IComparable> list)
        {
            for (int i = 1; i < list.Count; i++)
            {
                if (Less(list[i], list[i - 1])) return false;
            }
            return true;
        }
    }

}
