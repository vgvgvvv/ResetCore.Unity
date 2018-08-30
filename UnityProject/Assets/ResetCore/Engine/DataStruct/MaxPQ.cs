using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace ResetCore.DataStruct
{
    public class MaxPQ<T> where T : IComparable
    {
        List<T> queue = new List<T>();

        public int size { get { return queue.Count - 1; } }

        public List<T> list
        {
            get
            {
                List<T> list = new List<T>();
                for (int i = 1; i < queue.Count; i++)
                {
                    list.Add(queue[i]);
                }
                return list;
            }
        }

        public MaxPQ()
        {
            queue = new List<T>();
            //序号0的值不被使用
            queue.Add(default(T));
        }

        public MaxPQ(IEnumerable<T> list)
            : this()
        {
            foreach (T item in list)
            {
                Insert(item);
            }
        }

        public void Insert(T value)
        {
            queue.Add(value);
            Swim(size - 1);
        }

        public T Max()
        {
            return queue[1];
        }

        public T DeQueueMax()
        {
            T max = queue[1];
            Exch(1, size - 1);
            Sink(1);
            return max;
        }

        public bool IsEmpty()
        {
            return size <= 1;
        }

        private bool Less(int i, int j)
        {
            return queue[i].CompareTo(queue[j]) < 0;
        }

        private void Exch(int i, int j)
        {
            T temp = queue[i];
            queue[i] = queue[j];
            queue[j] = temp;
        }

        /// <summary>
        /// 上浮
        /// </summary>
        /// <param name="k"></param>
        private void Swim(int k)
        {
            while (k > 1 && Less(k / 2, k))
            {
                Exch(k / 2, k);
                k = k / 2;
            }
        }

        /// <summary>
        /// 下沉
        /// </summary>
        /// <param name="k"></param>
        private void Sink(int k)
        {
            while (2 * k <= size)
            {
                int j = 2 * k;
                if (j < size && Less(j, j + 1)) j++;
                if (Less(j, k)) break;
                Exch(j, k);
                k = j;
            }
        }

    }

}
