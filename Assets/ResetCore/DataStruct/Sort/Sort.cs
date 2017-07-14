using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

namespace ResetCore.DataStruct
{
    public static class Sort
    {

        /// <summary>
        /// 插入排序
        /// 将右边无序的数字插入左边的队列中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void InsertionSort(this List<IComparable> list)
        {
            int N = list.Count;
            for (int i = 1; i < N; i++)
            {
                for (int j = i; j > 0 && list[j].Less(list[j - 1]); j--)
                {
                    list.Exch(j, j - 1);
                }
            }
        }

        /// <summary>
        /// 选择排序
        /// 右边找到最小的然后放到左边
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void SelectionSort(this List<IComparable> list)
        {
            int N = list.Count;
            for (int i = 0; i < N; i++)
            {
                int min = 1;
                for (int j = i; j < N; j++)
                {
                    if (list[j].Less(list[min]))
                    {
                        min = j;
                    }
                    list.Exch(i, min);
                }
            }
        }

        /// <summary>
        /// 希尔排序
        /// 将整个序列进行分组，然后逐渐进行排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void ShellSort(this List<IComparable> list)
        {
            int N = list.Count;
            int h = 1;
            while (h < N / 3) h = 3 * h + 1;
            while (h >= 1)
            {
                for (int i = h; i < N; i++)
                {
                    for (int j = i; j >= h && list[j].Less(list[j - h]); j -= h)
                    {
                        list.Exch(j, j - h);
                    }
                }
                h = h / 3;
            }
        }


        
    }

}
