using System;
using System.Collections.Generic;
using System.Text;

namespace Heap
{
    public class Heap<K, D> where K : IComparable<K>
    {
        private class Node : IHeapifyable<K, D>
        {
            public D Data { get; set; }
            public K Key { get; set; }
            public int Position { get; set; }

            public Node(K key, D value, int position)
            {
                Data = value;
                Key = key;
                Position = position;
            }

            public override string ToString()
            {
                return "(" + Key.ToString() + "," + Data.ToString() + "," + Position + ")";
            }
        }

        public int Count { get; private set; }

        private List<Node> data = new List<Node>();
        private IComparer<K> comparer;

        public Heap(IComparer<K> comparer)
        {
            this.comparer = comparer;

            if (this.comparer == null)
                this.comparer = Comparer<K>.Default;

            data.Add(new Node(default(K), default(D), 0));
        }

        public IHeapifyable<K, D> Min()
        {
            if (Count == 0)
                throw new InvalidOperationException("The heap is empty.");

            return data[1];
        }

        public IHeapifyable<K, D> Insert(K key, D value)
        {
            Count++;
            Node node = new Node(key, value, Count);
            data.Add(node);
            UpHeap(Count);
            return node;
        }

        private void UpHeap(int start)
        {
            int position = start;

            while (position != 1)
            {
                if (comparer.Compare(data[position].Key, data[position / 2].Key) < 0)
                    Swap(position, position / 2);

                position = position / 2;
            }
        }

        private void Swap(int from, int to)
        {
            Node temp = data[from];
            data[from] = data[to];
            data[to] = temp;

            data[to].Position = to;
            data[from].Position = from;
        }

        public void Clear()
        {
            for (int i = 0; i <= Count; i++)
                data[i].Position = -1;

            data.Clear();
            data.Add(new Node(default(K), default(D), 0));
            Count = 0;
        }

        public override string ToString()
        {
            if (Count == 0)
                return "[]";

            StringBuilder s = new StringBuilder();
            s.Append("[");

            for (int i = 0; i < Count; i++)
            {
                s.Append(data[i + 1]);

                if (i + 1 < Count)
                    s.Append(",");
            }

            s.Append("]");
            return s.ToString();
        }

        public IHeapifyable<K, D> Delete()
        {
            if (Count == 0)
                throw new InvalidOperationException("The heap is empty.");

            Node root = data[1];

            if (Count == 1)
            {
                data.RemoveAt(1);
                Count--;
                root.Position = -1;
                return root;
            }

            data[1] = data[Count];
            data[1].Position = 1;

            data.RemoveAt(Count);
            Count--;

            root.Position = -1;

            DownHeap(1);

            return root;
        }

        private void DownHeap(int start)
        {
            int position = start;

            while (2 * position <= Count)
            {
                int leftChild = 2 * position;
                int rightChild = leftChild + 1;
                int bestChild = leftChild;

                if (rightChild <= Count &&
                    comparer.Compare(data[rightChild].Key, data[leftChild].Key) < 0)
                {
                    bestChild = rightChild;
                }

                if (comparer.Compare(data[bestChild].Key, data[position].Key) < 0)
                {
                    Swap(position, bestChild);
                    position = bestChild;
                }
                else
                {
                    break;
                }
            }
        }

        public IHeapifyable<K, D>[] BuildHeap(K[] keys, D[] values)
        {
            if (Count != 0)
                throw new InvalidOperationException("The heap is not empty.");

            if (keys == null || values == null)
                throw new ArgumentNullException("The key and data arrays must not be null.");

            if (keys.Length != values.Length)
                throw new ArgumentException("The key and data arrays must have the same length.");

            IHeapifyable<K, D>[] result = new IHeapifyable<K, D>[keys.Length];

            for (int i = 0; i < keys.Length; i++)
            {
                Node node = new Node(keys[i], values[i], i + 1);
                data.Add(node);
                result[i] = node;
            }

            Count = keys.Length;

            for (int i = Count / 2; i >= 1; i--)
            {
                DownHeap(i);
            }

            return result;
        }

        public void DecreaseKey(IHeapifyable<K, D> element, K new_key)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if (!(element is Node node))
                throw new InvalidOperationException("The specified element does not belong to this heap.");

            if (node.Position < 1 ||
                node.Position > Count ||
                !object.ReferenceEquals(data[node.Position], node))
            {
                throw new InvalidOperationException("The specified element is inconsistent with the current heap state.");
            }

            K oldKey = node.Key;
            node.Key = new_key;

            if (node.Position > 1 &&
                comparer.Compare(node.Key, data[node.Position / 2].Key) < 0)
            {
                UpHeap(node.Position);
            }
            else if (comparer.Compare(node.Key, oldKey) > 0)
            {
                DownHeap(node.Position);
            }
        }
    }
}