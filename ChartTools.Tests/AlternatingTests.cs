using ChartTools.Extensions.Collections.Alternating;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChartTools.Tests;

[TestClass]
public class SerialAlternatingTests
{
    static readonly byte[] testArrayA = [ 1, 6, 2 ];
    static readonly byte[] testArrayB = [ 3, 5, 6 ];
    const string expected = "1 3 6 5 2 6";

    [TestMethod] public void CreateEnumerableNull() => Assert.ThrowsException<ArgumentNullException>(() => new SerialAlternatingEnumerable<byte>(null!));
    [TestMethod] public void CreateEnumerableEmpty() => Assert.ThrowsException<ArgumentException>(() => new SerialAlternatingEnumerable<byte>());

    [TestMethod] public void Enumerate() => Assert.AreEqual(expected,  Formatting.FormatCollection(new SerialAlternatingEnumerable<byte>(testArrayA, testArrayB)));
}

[TestClass]
public class OrderedAlternatingTests
{
    static readonly Func<byte, byte> keyGetter = n => n;
    static readonly byte[] testArrayA = [1, 6, 2];
    static readonly byte[] testArrayB = [3, 5, 6];
    const string expected = "1 3 5 6 2 6";

    [TestMethod] public void CreateEnumerableNullKeyGetter() => Assert.ThrowsException<ArgumentNullException>(() => new OrderedAlternatingEnumerable<byte, byte>(null!, null!));

    [TestMethod] public void CreateEnumerableNullEnumerables() => Assert.ThrowsException<ArgumentNullException>(() => new OrderedAlternatingEnumerable<byte, byte>(null!, null!));
    [TestMethod] public void CreateEnumerableEmptyEnumerables() => Assert.ThrowsException<ArgumentException>(() => new OrderedAlternatingEnumerable<byte, byte>(keyGetter));

    [TestMethod] public void Enumerate() => Assert.AreEqual(expected, Formatting.FormatCollection(new OrderedAlternatingEnumerable<byte, byte>(keyGetter, testArrayA, testArrayB)));
}
