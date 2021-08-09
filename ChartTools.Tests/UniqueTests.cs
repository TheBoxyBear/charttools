using ChartTools.Collections.Unique;
using ChartTools.SystemExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

namespace ChartTools.Tests
{
    [TestClass]
    public class UniqueTests
    {
        private readonly EqualityComparison<byte> comparison = (a, b) => a == b;

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
    }
}
