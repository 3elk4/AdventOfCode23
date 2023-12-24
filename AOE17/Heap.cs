using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOE17
{
    public abstract class Heap<T> : IEnumerable<T>
    {
        private const int InitCap = 0;
        private const int GrowFactor = 2;
        private const int MinGrow = 1;

        private int _capacity = InitCap;
        private T[] _heap = new T[InitCap];
        private int _count = 0;

        public int Count { get { return _count; } }
        public int Capacity { get { return _capacity; } }


        protected Comparer<T> Comparer { get; private set; }

        protected Heap() : this(Comparer<T>.Default) { }

        protected Heap(Comparer<T> comparer) : this(Enumerable.Empty<T>(), comparer) { }

        protected Heap(IEnumerable<T> collection) : this(collection, Comparer<T>.Default) { }

        protected Heap(IEnumerable<T> collection, Comparer<T> comparer)
        {
            if (collection == null) throw new ArgumentNullException("Collection");
            if (comparer == null) throw new ArgumentNullException("Comparer");

            Comparer = comparer;

            foreach (var item in collection)
            {
                if (_count == _capacity)
                    Grow();

                _heap[_count++] = item;
            }

            for (int i = Parent(_count - 1); i >= 0; i--)
                Sink(i);
        }

        private void Grow()
        {
            int newCapacity = _capacity * GrowFactor + MinGrow;
            var newHeap = new T[newCapacity];
            Array.Copy(_heap, newHeap, _capacity);
            _heap = newHeap;
            _capacity = newCapacity;
        }

        private void Swap(int idx1, int idx2)
        {
            T tmp = _heap[idx1];
            _heap[idx1] = _heap[idx2];
            _heap[idx2] = tmp;
        }
        /// <summary>
        /// Shifting down value in heap.
        /// </summary>
        /// <param name="idx"> Index of value to sink. </param>
        private void Sink(int idx)
        {
            int domIdx = FindIndexOfDominatingNode(idx);

            if (domIdx == idx) return;
            Swap(idx, domIdx);
            Sink(domIdx);
        }

        private int FindIndexOfDominatingNode(int idx)
        {
            int dominatingNode = idx;
            dominatingNode = GetIndexOfDominatingNode(LeftChild(idx), dominatingNode);
            dominatingNode = GetIndexOfDominatingNode(RightChild(idx), dominatingNode);

            return dominatingNode;
        }

        private int GetIndexOfDominatingNode(int newIdx, int currentIdx)
        {
            return (newIdx < _count && !CheckProperOrder(_heap[currentIdx], _heap[newIdx])) ? newIdx : currentIdx;
        }

        /// <summary>
        /// Shifting up value in heap
        /// </summary>
        /// <param name="idx"> Index of value to swim. </param>
        private void Swim(int idx)
        {
            if (idx == 0 || CheckProperOrder(_heap[Parent(idx)], _heap[idx])) return;

            Swap(idx, Parent(idx));
            Swim(Parent(idx));
        }

        private int Parent(int idx)
        {
            return (idx + 1) / 2 - 1;
        }

        private int LeftChild(int idx)
        {
            return (idx + 1) * 2 - 1;
        }

        private int RightChild(int idx)
        {
            return LeftChild(idx) + 1;
        }

        protected abstract bool CheckProperOrder(T x, T y);

        public T Extract()
        {
            if (_count == 0) throw new InvalidOperationException("Heap is empty");
            T result = _heap[0];
            _count--;
            Swap(_count, 0);
            Sink(0);
            return result;
        }

        /// <summary>
        /// Takes first element of heap to see it, not to extract it. To extract see extract method.
        /// </summary>
        /// <returns> Top of heap. </returns>
        public T Peek()
        {
            if (_count == 0) return default(T);
            return _heap[0];
        }

        public void Insert(T item)
        {
            if (_count == _capacity)
                Grow();

            _heap[_count++] = item;
            Swim(_count - 1);
        }

        public void Replace(T item)
        {
            Extract();
            Insert(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _heap.Take(Count).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    public class MinHeap<T> : Heap<T>
    {
        public MinHeap() : this(Comparer<T>.Default) { }

        public MinHeap(Comparer<T> comparer) : base(comparer) { }

        public MinHeap(IEnumerable<T> collection) : base(collection) { }

        public MinHeap(IEnumerable<T> collection, Comparer<T> comparer) : base(collection, comparer) { }

        protected override bool CheckProperOrder(T x, T y)
        {
            return Comparer.Compare(x, y) <= 0;
        }
    }

    public class MaxHeap<T> : Heap<T>
    {
        public MaxHeap() : this(Comparer<T>.Default) { }

        public MaxHeap(Comparer<T> comparer) : base(comparer) { }

        public MaxHeap(IEnumerable<T> collection) : base(collection) { }

        public MaxHeap(IEnumerable<T> collection, Comparer<T> comparer) : base(collection, comparer) { }

        protected override bool CheckProperOrder(T x, T y)
        {
            return Comparer.Compare(x, y) <= 0;
        }
    }
}
