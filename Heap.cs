using UnityEngine;
using System.Collections;
using System;
namespace EE.PathfindingSystem {

    internal interface IHeapItem<T> : IComparable<T> {
        int HeapIndex { get; set; }
    }

    internal class Heap<T> where T : IHeapItem<T> {
        private readonly T[] _items;
        private int _count;
        public int Count => _count;

        public Heap(int maxHeapSize) {
            _items = new T[maxHeapSize];
        }

        public void Add(T item) {
            item.HeapIndex = _count;
            _items[_count] = item;
            _count++;

            SortUp(item);
        }

        public T RemoveFirst() {
            T firstItem = _items[0];
            _count--;
            _items[0] = _items[_count];
            _items[0].HeapIndex = 0;
            SortDown(_items[0]);
            return firstItem;
        }

        public bool Contains(T item) => Equals(_items[item.HeapIndex], item);

        public void UpdateItem(T item) => SortUp(item);

        private void SortUp(T item) {
            int parentIndex = (item.HeapIndex - 1) / 2;

            while (true) {
                T parentItem = _items[parentIndex];
                if (item.CompareTo(parentItem) > 0) {
                    Swap(item, parentItem);
                }
                else {
                    break;
                }

                parentIndex = (item.HeapIndex - 1) / 2;
            }
        }

        private void SortDown(T item) {
            while (true) {
                int childIndexLeft = item.HeapIndex * 2 + 1;
                int childIndexRight = item.HeapIndex * 2 + 2;
                if (childIndexLeft < _count) {
                    int swapIndex = childIndexLeft;
                    if (childIndexRight < _count) {
                        if (_items[childIndexLeft].CompareTo(_items[childIndexRight]) < 0) {
                            swapIndex = childIndexRight;
                        }
                    }

                    if (item.CompareTo(_items[swapIndex]) < 0) {
                        Swap(item, _items[swapIndex]);
                    }
                    else {
                        return;
                    }

                }
                else {
                    return;
                }

            }
        }

        private void Swap(T itemA, T itemB) {
            _items[itemA.HeapIndex] = itemB;
            _items[itemB.HeapIndex] = itemA;
            (itemB.HeapIndex, itemA.HeapIndex) = (itemA.HeapIndex, itemB.HeapIndex);
        }
    }

}