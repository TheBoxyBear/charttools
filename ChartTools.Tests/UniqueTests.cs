using ChartTools.Collections.Unique;
using ChartTools.SystemExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;

namespace ChartTools.Tests
{
    [TestClass]
    public class UniqueTests
    {
        readonly EqualityComparison<byte> comparison = (a, b) => a == b;

        readonly byte[] testArrayA = new byte[] { 1, 1, 2, 3 };
        readonly byte[] testArrayB = new byte[] { 4, 5, 6, 2, 2, 1, 3, 6 };

        [TestMethod]
        public void CreateEnumerableNullComparison() => Assert.ThrowsException<ArgumentNullException>(() => new UniqueEnumerable<byte>(null));
        [TestMethod]
        public void CreateEnumerableNullEnumerables() => Assert.ThrowsException<ArgumentNullException>(() => new UniqueEnumerable<byte>(comparison, null));
        [TestMethod]
        public void CreateEnumerableEmptyEnumerables() => Assert.ThrowsException<ArgumentException>(() => new UniqueEnumerable<byte>(comparison));

        [TestMethod]
        public void CreateEnumeratorNullComparison() => Assert.ThrowsException<ArgumentNullException>(() => new UniqueEnumerator<byte>(null));
        [TestMethod]
        public void CreateEnumeratorNullEnumerators() => Assert.ThrowsException<ArgumentNullException>(() => new UniqueEnumerator<byte>(comparison, null));
        [TestMethod]
        public void CreateEnumeratorEmptyEnumerators() => Assert.ThrowsException<ArgumentException>(() => new UniqueEnumerator<byte>(comparison));

        [TestMethod]
        public void TestFromEnumerable() => Assert.IsTrue(string.Concat(new UniqueEnumerable<byte>(comparison, testArrayA, testArrayB)) == "123456");
    }
}
