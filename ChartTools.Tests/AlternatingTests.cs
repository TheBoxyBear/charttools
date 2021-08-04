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
        public void CreateEnumerableEmpty() => Assert.ThrowsException<ArgumentException>(() => new SerialAlternatingEnumerable<object>(Array.Empty<IEnumerable<string>>()));
    }

    [TestClass]
    [TestCategory("Alternating")]
    public class OrderedAlternatingTests
    {
        private static readonly Func<int, object> keyGetter = (i => i);

        [TestMethod]
        public void CreateEnumerableNullKeyGetter => Assert.ThrowsException<ArgumentNullException>(() => new OrderedAlternatingEnumerable<int, int>(null, null));

        [TestMethod]
        public void CreateEnumerableNullEnumerables() => Assert.ThrowsException<ArgumentNullException>(() => new OrderedAlternatingEnumerable<string>(null));
        [TestMethod]
        public void CreateEnumerableEmptyEnumerables() => Assert.ThrowsException<ArgumentException>(() => new OrderedAlternatingEnumerable<string>(Array.Empty<IEnumerable<string>>()));
    }
}
