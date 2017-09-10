using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace ResetCore.DataStruct
{
    public static class SortMerge
    {

        private static List<IComparable> temp;
        public static void MergeSort(this List<IComparable> list)
        {
            temp = new List<IComparable>();
            list.Sort(0, list.Count - 1);
        }

        private static void Sort(this List<IComparable> list, int lo, int hi)
        {
            if (lo >= hi) return;
            int mid = lo + (hi - lo) / 2;
            list.Sort(lo, mid);
            list.Sort(mid + 1, hi);

        }

        private static void Merge(this List<IComparable> list, int lo, int mid, int hi)
        {
            int i = lo;
            int j = mid + 1;

            temp = new List<IComparable>();
            foreach (IComparable el in list)
            {
                temp.Add(el);
            }

            for (int k = lo; k < hi; k++)
            {
                if (i > mid)
                    list[k] = temp[j++];//左边用尽
                else if (j > hi)
                    list[k] = temp[i++];//右边用尽
                else if (temp[j].Less(temp[i]))
                    list[k] = temp[j++];//右边比较大
                else
                    list[k] = temp[i++];//左边比较大
            }
        }
    }

}
