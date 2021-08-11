using ChartTools;
using ChartTools.Collections.Alternating;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools.Tests
{
    [TestClass]
    public class SerialAlternatingTests
    {
        readonly byte[] testArrayA = new byte[] { 1, 6, 2 };
        readonly byte[] testArrayB = new byte[] { 3, 5, 6 };

        [TestMethod]
        public void CreateEnumerableNull() => Assert.ThrowsException<ArgumentNullException>(() => new SerialAlternatingEnumerable<byte>(null));
        [TestMethod]
        public void CreateEnumerableEmpty() => Assert.ThrowsException<ArgumentException>(() => new SerialAlternatingEnumerable<byte>());

        [TestMethod]
        public void CreateEnumeratorNull() => Assert.ThrowsException<ArgumentNullException>(() => new SerialAlternatingEnumerator<byte>(null));
        [TestMethod]
        public void CreateEnumeratorEmpty() => Assert.ThrowsException<ArgumentException>(() => new SerialAlternatingEnumerator<byte>());

        [TestMethod]
        public void EnumerateFromEnumerable() => Assert.IsTrue(string.Concat(new SerialAlternatingEnumerable<byte>(testArrayA, testArrayB)) == "136526");

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

            Assert.IsTrue(string.Concat(output) == "136526");
        }
    }

    [TestClass]
    public class OrderedAlternatingTests
    {
        static readonly Func<byte, byte> keyGetter = n => n;
        readonly byte[] testArrayA = new byte[] { 1, 6, 2 };
        readonly byte[] testArrayB = new byte[] { 3, 5, 6 };

        [TestMethod]
        public void CreateEnumerableNullKeyGetter() => Assert.ThrowsException<ArgumentNullException>(() => new OrderedAlternatingEnumerable<byte, byte>(null, null));

        [TestMethod]
        public void CreateEnumerableNullEnumerables() => Assert.ThrowsException<ArgumentNullException>(() => new OrderedAlternatingEnumerable<byte, byte>(null, null));
        [TestMethod]
        public void CreateEnumerableEmptyEnumerables() => Assert.ThrowsException<ArgumentException>(() => new OrderedAlternatingEnumerable<byte, byte>(keyGetter));

        [TestMethod]
        public void EnumerateFromEnumerable() => Assert.IsTrue(string.Concat(new OrderedAlternatingEnumerable<byte, byte>(keyGetter, testArrayA, testArrayB)) == "135626");

        [TestMethod]
        public void EnumerateFromEnumertor()
        {
            var enumerator = new OrderedAlternatingEnumerator<byte, byte>(keyGetter, testArrayA.AsEnumerable().GetEnumerator(), testArrayB.AsEnumerable().GetEnumerator());
            var output = new List<byte>(6);

            for (int i = 0; i < 6; i++)
            {
                Assert.IsTrue(enumerator.MoveNext());
                output.Add(enumerator.Current);
            }

            Assert.IsTrue(string.Concat(output) == "135626");
        }
    }
}
