using ChartTools;
using ChartTools.Collections.Alternating;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools.Tests
{
    [TestClass]
    [TestCategory("Alternating")]
    public class SerialAlternatingTests
    {
        byte[] testArrayA = new byte[] { 1, 2, 3 };
        byte[] testArrayB = new byte[] { 4, 5, 6 };

        [TestMethod]
        public void CreateEnumerableNull() => Assert.ThrowsException<ArgumentNullException>(() => new SerialAlternatingEnumerable<object>(null));
        [TestMethod]
        public void CreateEnumerableEmpty() => Assert.ThrowsException<ArgumentException>(() => new SerialAlternatingEnumerable<object>());

        [TestMethod]
        public void CreateEnumeratorNull() => Assert.ThrowsException<ArgumentNullException>(() => new SerialAlternatingEnumerator<object>(null));
        [TestMethod]
        public void CreateEnumeratorEmpty() => Assert.ThrowsException<ArgumentException>(() => new SerialAlternatingEnumerator<object>());

        [TestMethod]
        public void EnumerateFromEnumerable() => Assert.IsTrue(string.Concat(new SerialAlternatingEnumerable<byte>(testArrayA, testArrayB)) == "142536");

        [TestMethod]
        public void EnumerateFromEnumertor()
        {
            var enumerator = new SerialAlternatingEnumerator<byte>(testArrayA.AsEnumerable().GetEnumerator(), testArrayB.AsEnumerable().GetEnumerator());
            var output = new List<byte>(6);

            for (int i = 0; i < 6; i++)
            {
                Assert.IsTrue(enumerator.MoveNext());
                output.Add(enumerator.Current);
            }

            Assert.IsTrue(string.Concat(output) == "142536");
        }
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
