using ChartTools.Collections.Unique;
using ChartTools.SystemExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Linq;

namespace ChartTools.Tests
{
    [TestClass]
    public class UniqueTests
    {
        static readonly byte[] testItems = new byte[] { 1, 2, 2, 5, 2, 5, 7 };
        static readonly byte[] expectedArray = new byte[] { 1, 2, 5, 7 };
        static readonly EqualityComparison<byte> comparison = (a, b) => a == b;
        const string expectedString = "1257";
        const byte missingItem = 10;

        [TestMethod] public void Count() => Assert.AreEqual(expectedArray.Length, GetList().Count);
        [TestMethod] public void IndexGetter() => Assert.AreEqual(2, GetList()[1]);
        [TestMethod] public void IndexSetter()
        {
            var list = GetList();
            list[1] = 5;

            Assert.AreEqual(string.Concat(new byte[] { 1, 5, 7 }), string.Concat(list));
        }

        [TestMethod] public void CreateListNullComparison() => Assert.ThrowsException<ArgumentNullException>(() => new UniqueList<byte>(null));
        [TestMethod] public void CreateListNegativeCapacity() => Assert.ThrowsException<ArgumentOutOfRangeException>(() => new UniqueList<byte>(comparison, -1));
        [TestMethod] public void CreateListStartingItems() => Assert.IsTrue(string.Concat(new UniqueList<byte>(comparison, testItems.Length, testItems)) == expectedString);

        [TestMethod] public void Add()
        {
            var list = new UniqueList<byte>(comparison);

            foreach (byte item in testItems)
                list.Add(item);

            Assert.AreEqual(expectedString, string.Concat(list));
        }
        [TestMethod] public void AddRange()
        {
            var list = new UniqueList<int>((a ,b) => a == b);
            list.AddRange(Enumerable.Range(0, 50000));
            list.AddRange(Enumerable.Repeat(1, 50000));

            Assert.IsTrue(string.Concat(list) == expectedString);
        }
        [TestMethod] public void Clear()
        {
            var list = GetList();
            list.Clear();

            string result = string.Concat(list);

            Assert.AreEqual(string.Empty, result);
        }
        [TestMethod] public void Contains()
        {
            var list = GetList();

            foreach (byte item in testItems)
                Assert.IsTrue(list.Contains(item), $"List did not report {item} as being contained.");
        }
        [TestMethod] public void ContainsMissingItem() => Assert.IsFalse(GetList().Contains(missingItem), $"List reported {missingItem} as being contained despite not being in the list");
        [TestMethod] public void CopyTo()
        {
            var list = GetList();
            byte[] result = new byte[4];

            list.CopyTo(result, 0);

            Assert.AreEqual(string.Concat(expectedArray), string.Concat(result));
        }
        [TestMethod] public void IndexOf()
        {
            var list = GetList();

            for (int i = 0; i < expectedArray.Length; i++)
                Assert.AreEqual(i, list.IndexOf(expectedArray[i]));
        }
        [TestMethod] public void IndexOfMissingItem() => Assert.AreEqual(-1, GetList().IndexOf(missingItem));
        [TestMethod] public void InsertMissingItem()
        {
            var list = GetList();
            list.Insert(0, missingItem);

            Assert.AreEqual(string.Concat(new byte[] { 10 }.Concat(expectedArray)), string.Concat(list));
        }
        [TestMethod] public void InsertExistingItem()
        {
            var list = GetList();
            list.Insert(0, 5);

            Assert.AreEqual(string.Concat(new byte[] { 5, 1, 2, 7 }), string.Concat(list));
        }
        [TestMethod] public void RemoveMissingItem() => Assert.IsFalse(GetList().Remove(missingItem));
        [TestMethod] public void RemoveExistingItem()
        {
            var list = GetList();

            Assert.IsTrue(list.Remove(5));
            Assert.AreEqual(string.Concat(new byte[] { 1, 2, 7 }), string.Concat(list));
        }
        [TestMethod] public void RemoteAtNegativeIndex() => Assert.ThrowsException<ArgumentOutOfRangeException>(() => GetList().RemoveAt(-1));
        [TestMethod] public void RemoteAtHighIndex() => Assert.ThrowsException<ArgumentOutOfRangeException>(() => GetList().RemoveAt(int.MaxValue));
        [TestMethod] public void RemoveAt()
        {
            var list = GetList();
            list.RemoveAt(2);

            Assert.AreEqual(string.Concat(new byte[] { 1, 2, 7 }), string.Concat(list));
        }

        private static UniqueList<byte> GetList() => new UniqueList<byte>(comparison, testItems.Length, testItems);
    }
}
