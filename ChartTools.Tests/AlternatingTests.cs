using ChartTools;
using ChartTools.Collections.Alternating;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;

namespace ChartTools.Tests
{
    [TestClass]
    [TestCategory("Alternating")]
    public class SerialAlternatingTests
    {
        [TestMethod]
        public void CreateEnumerableNull() => Assert.ThrowsException<ArgumentNullException>(() => new SerialAlternatingEnumerable<object>(null));
        [TestMethod]
        public void CreateEnumerableEmpty() => Assert.ThrowsException<ArgumentException>(() => new SerialAlternatingEnumerable<object>());
    }

    [TestClass]
    [TestCategory("Alternating")]
    public class OrderedAlternatingTests
    {
        private static readonly Func<object, int> keyGetter = o => 0;

        [TestMethod]
        public void CreateEnumerableNullKeyGetter() => Assert.ThrowsException<ArgumentNullException>(() => new OrderedAlternatingEnumerable<int, object>(null, null));

        [TestMethod]
        public void CreateEnumerableNullEnumerables() => Assert.ThrowsException<ArgumentNullException>(() => new OrderedAlternatingEnumerable<int, object>(null, null));
        [TestMethod]
        public void CreateEnumerableEmptyEnumerables() => Assert.ThrowsException<ArgumentException>(() => new OrderedAlternatingEnumerable<int, object>(keyGetter));
    }
}
