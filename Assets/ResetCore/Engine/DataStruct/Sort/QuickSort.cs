using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace ResetCore.DataStruct
{
    public static class QuickSort
    {

        /// <summary>
        /// 快排
        /// </summary>
        /// <param name="list"></param>
        public static void QSort(this List<IComparable> list)
        {
            Sort(list, 0, list.Count - 1);
        }

        private static void Sort(List<IComparable> list, int lo, int hi)
        {
            if (hi <= lo) return;
            //递归切分
            int j = Partition(list, lo, hi);
            Sort(list, lo, j);
            Sort(list, j + 1, hi);
        }

        private static int Partition(List<IComparable> list, int lo, int hi)
        {
            int i = lo;
            int j = hi + 1;
            IComparable v = list[lo];
            while (true)
            {
                //向右扫描直到找到大于v的值
                while (list[i++].Less(v))
                {
                    if (i == hi)
                        break;
                }
                //向左扫描直到找到小于v的值
                while (v.Less(list[--j]))
                {
                    if (j == lo)
                        break;
                }
                //i与j相遇
                if (i >= j)
                    break;
                list.Exch(i, j);
            }
            list.Exch(lo, j);
            return j;
        }

        /// <summary>
        /// 三向切分快排，针对很多重复元素的情况
        /// </summary>
        /// <param name="list"></param>
        public static void QSort3way(this List<IComparable> list)
        {
            list.QSort3way(0, list.Count - 1);
        }

        public static void QSort3way(this List<IComparable> list, int lo, int hi)
        {
            if (hi <= lo) return;
            int lt = lo;
            int i = lo + 1;
            int gt = hi;
            IComparable v = list[lo];
            while (i <= gt)
            {
                int cmp = list[i].CompareTo(v);
                //如果小于那就和交换下
                if (cmp < 0)
                {
                    list.Exch(lt++, i++);
                }
                else if (cmp > 0)
                {
                    list.Exch(i, gt--);
                }
                else
                {
                    i++;
                }
            }
            list.QSort3way(lo, lt - 1);
            list.QSort3way(gt + 1, hi);
        }


    }

}
