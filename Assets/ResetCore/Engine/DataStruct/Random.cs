using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ResetCore.DataStruct
{
    public static class Random
    {
        public static void Mess<T>(this List<T> list)
        {
            T tmp;
            int index;
            int n = list.Count-1;
            for (int i = 0; i < list.Count; i++)
            {
                index = UnityEngine.Random.Range(i, n);
                if (index != i)
                {
                    tmp = list[i];
                    list[i] = list[index];
                    list[index] = tmp;
                }  
            }
        }

    }

}
