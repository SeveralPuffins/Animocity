using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace Animocity.Cities.Algorithms
{
    /// <summary>
    /// A CompSci would call this an N-ary Heap, but to a biologist like me this is an affinity column.
    /// Chuck things in and see where they stick, elute them by low affinity to high affinity!
    /// </summary>
    /// <typeparam name="T">Can be used with any type, but you have to submit costs alongside.</typeparam>
    public class AffinityColumn<T> : IEnumerable<T>
    {
        private const int K = 4;

        private int count;
        private T[] items;
        private float[] costs;

        public AffinityColumn(int capacity)
        {
            items = new T[capacity];
            costs = new float[capacity];
            count = 0;
        }

        public T Peek(out float cost)
        {
            if (this.count == 0)
            {
                cost = float.MaxValue;
                return default(T);
            }
            else cost = costs[0];
            return items[0];
        }

        public T Pop(out float cost)
        {
            if (count == 0)
            {
                cost = float.MinValue;
                return default(T);
            }

            cost = costs[0];
            var result = items[0];

            count -= 1;
            items[0] = items[count];
            costs[0] = costs[count];

            BubbleUp(0);

            return result;
        }

        private void BubbleUp(int parentIdx)
        {

            float minCostInChildren = costs[parentIdx];
            int minCostIdx = parentIdx;
            //for all children
            for (int i = 1 + (parentIdx) * K; i < 1 + (parentIdx + 1) * K; i++)
            {
                if (i >= count) break;

                if (costs[i] <= minCostInChildren)
                {
                    minCostInChildren = costs[i];
                    minCostIdx = i;
                }
            }
            if (minCostIdx != parentIdx)
            {
                T minCostChild = items[minCostIdx];

                items[minCostIdx] = items[parentIdx];
                costs[minCostIdx] = costs[parentIdx];

                items[parentIdx] = minCostChild;
                costs[parentIdx] = minCostInChildren;

                BubbleUp(minCostIdx);
            }
        }

        //Should only be called with a LOWER number. Only really works with objects too!
        public void UpdateValue(T item, float newValue)
        {
            for (int i = 0; i < count; i++)
            {
                if (items[i].Equals(item))
                {
                    costs[i] = newValue;
                    SinkDown(i);
                    return;
                }
            }
        }


        public void Add(T item, float cost)
        {
            int idx = count;
            items[idx] = item;
            costs[idx] = cost;
            count++;


            if (count > 1)
            {
                SinkDown(idx);
            }
        }

        private void SinkDown(int childIdx)
        {
            if (childIdx <= 0)
            {
                return;
            }
            int parentIdx = parentOf(childIdx);


            if (costs[parentIdx] > costs[childIdx])
            {
                float childCost = costs[childIdx];
                T childItem = items[childIdx];

                costs[childIdx] = costs[parentIdx];
                items[childIdx] = items[parentIdx];

                costs[parentIdx] = childCost;
                items[parentIdx] = childItem;

                SinkDown(parentIdx);
            }
        }

        private int parentOf(int i)
        {
            return (i - 1) / K;
        }

        public int Count
        {
            get
            {
                return count;
            }
        }

        public void Clear()
        {
            count = 0;
            Array.Clear(items, 0, items.Length - 1);
            Array.Clear(costs, 0, costs.Length - 1);
        }

        public IEnumerator<T> GetEnumerator()
        {
            var enumerator = items.AsEnumerable().GetEnumerator();
            enumerator.Reset();
            int i = 0;

            while (i < count)
            {
                enumerator.MoveNext();
                yield return enumerator.Current;
                i++;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
