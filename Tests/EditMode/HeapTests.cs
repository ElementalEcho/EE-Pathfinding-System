using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using EE.PathfindingSystem;

namespace EE.PathfindingSystem.EditModeTests {
    public class HeapTests {

        internal class TestHeapItem : IHeapItem<TestHeapItem> {

            public int compareValue;
            public int heapIndex;
            public int HeapIndex { get => heapIndex; set => heapIndex = value; }

            public TestHeapItem(int compareValue = 0) {
                this.compareValue = compareValue;
            }

            public int CompareTo(TestHeapItem other) {
                return compareValue.CompareTo(other.compareValue);
            }
        }
        [Test]
        public void Heap_Count() {
            var heap = new Heap<TestHeapItem>(5);

            Assert.AreEqual(0, heap.Count);

            heap.Add(new TestHeapItem());

            Assert.AreEqual(1, heap.Count);
        }

        [Test]
        public void Heap_Add() {
            var heap = new Heap<TestHeapItem>(5);

            var testHeapItem1 = new TestHeapItem();
            var testHeapItem2 = new TestHeapItem();

            heap.Add(testHeapItem1);
            heap.Add(testHeapItem2);

            Assert.AreEqual(2, heap.Count);
            Assert.AreEqual(0, testHeapItem1.HeapIndex);
            Assert.AreEqual(1, testHeapItem2.HeapIndex);
        }
        [Test]
        public void Heap_Remove() {
            var heap = new Heap<TestHeapItem>(5);

            var testHeapItem1 = new TestHeapItem(3);
            var testHeapItem2 = new TestHeapItem(4);
            var testHeapItem3 = new TestHeapItem(5);

            heap.Add(testHeapItem1);
            heap.Add(testHeapItem2);
            heap.Add(testHeapItem3);

            Assert.AreEqual(3, heap.Count);
            Assert.AreEqual(1, testHeapItem1.HeapIndex);
            Assert.AreEqual(2, testHeapItem2.HeapIndex);
            Assert.AreEqual(0, testHeapItem3.HeapIndex);

            var removedValue = heap.RemoveFirst();

            Assert.AreEqual(2, heap.Count);
            Assert.AreEqual(1, testHeapItem1.HeapIndex);
            Assert.AreEqual(0, testHeapItem2.HeapIndex);
            Assert.AreEqual(0, testHeapItem3.HeapIndex);
            Assert.AreEqual(testHeapItem3, removedValue);
        }
        [Test]
        public void Heap_UpdateItem() {
            var heap = new Heap<TestHeapItem>(5);

            var testHeapItem1 = new TestHeapItem(3);
            var testHeapItem2 = new TestHeapItem(4);
            var testHeapItem3 = new TestHeapItem(5);

            heap.Add(testHeapItem1);
            heap.Add(testHeapItem2);
            heap.Add(testHeapItem3);

            testHeapItem1.compareValue = 20;

            Assert.AreEqual(1, testHeapItem1.HeapIndex);
            Assert.AreEqual(2, testHeapItem2.HeapIndex);
            Assert.AreEqual(0, testHeapItem3.HeapIndex);

            heap.UpdateItem(testHeapItem1);

            Assert.AreEqual(0, testHeapItem1.HeapIndex);
            Assert.AreEqual(2, testHeapItem2.HeapIndex);
            Assert.AreEqual(1, testHeapItem3.HeapIndex);
        }
        [Test]
        public void Heap_Contains() {
            var heap = new Heap<TestHeapItem>(5);

            var testHeapItem1 = new TestHeapItem();
            var testHeapItem2 = new TestHeapItem();

            heap.Add(testHeapItem1);

            Assert.IsTrue(heap.Contains(testHeapItem1));
            Assert.IsFalse(heap.Contains(testHeapItem2));
        }

        [Test]
        public void Heap_Is_InCorrectOrder() {
            int maxSize = 6;

            var testHeapItem1 = new TestHeapItem(1);
            var testHeapItem2 = new TestHeapItem(2);
            var testHeapItem3 = new TestHeapItem(3);
            var testHeapItem4 = new TestHeapItem(4);
            var testHeapItem5 = new TestHeapItem(5);
            var testHeapItem6 = new TestHeapItem(6);

            var heap = new Heap<TestHeapItem>(maxSize);
            heap.Add(testHeapItem1);
            heap.Add(testHeapItem2);
            heap.Add(testHeapItem3);
            heap.Add(testHeapItem4);
            heap.Add(testHeapItem5);
            heap.Add(testHeapItem6);

            Assert.AreEqual(6, heap.Count);
            Assert.AreEqual(3, testHeapItem1.HeapIndex);
            Assert.AreEqual(5, testHeapItem2.HeapIndex);
            Assert.AreEqual(4, testHeapItem3.HeapIndex);
            Assert.AreEqual(1, testHeapItem4.HeapIndex);
            Assert.AreEqual(2, testHeapItem5.HeapIndex);
            Assert.AreEqual(0, testHeapItem6.HeapIndex);
        }
        [Test]
        public void Heap_Is_InCorrectOrder2() {
            int maxSize = 6;

            var testHeapItem1 = new TestHeapItem(5);
            var testHeapItem2 = new TestHeapItem(3);
            var testHeapItem3 = new TestHeapItem(4);
            var testHeapItem4 = new TestHeapItem(1);
            var testHeapItem5 = new TestHeapItem(2);
            var testHeapItem6 = new TestHeapItem(0);

            var heap = new Heap<TestHeapItem>(maxSize);
            heap.Add(testHeapItem1);
            heap.Add(testHeapItem2);
            heap.Add(testHeapItem3);
            heap.Add(testHeapItem4);
            heap.Add(testHeapItem5);
            heap.Add(testHeapItem6);

            Assert.AreEqual(6, heap.Count);
            Assert.AreEqual(0, testHeapItem1.HeapIndex);
            Assert.AreEqual(1, testHeapItem2.HeapIndex);
            Assert.AreEqual(2, testHeapItem3.HeapIndex);
            Assert.AreEqual(3, testHeapItem4.HeapIndex);
            Assert.AreEqual(4, testHeapItem5.HeapIndex);
            Assert.AreEqual(5, testHeapItem6.HeapIndex);
        }




    }
}