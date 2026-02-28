using ChartTools.Extensions.Collections.Alternating;

using System.Collections;

namespace ChartTools.Tests.Extensions.Collections.Alternating;

[TestClass]
public class OrderedAlternatingTests
{
	static readonly Func<byte, byte> keyGetter = static n => n;

	static readonly byte[]
		testArrayA = [1, 6, 2],
		testArrayB = [3, 5, 6],
		expected   = [1, 3, 5, 6, 2, 6];

	[TestMethod, TestCategory("Ctor"), TestCategory(nameof(Exception))]
	public void Ctor_NullKeyGetter_Throws()
		=> Assert.Throws<ArgumentNullException>(
			static () => new OrderedAlternatingEnumerable<byte, byte>(null!, []));

	[TestMethod, TestCategory("Ctor"), TestCategory(nameof(Exception))]
	public void Ctor_NoEnumerables_Throws()
		=> Assert.Throws<ArgumentException>(
			static () => new OrderedAlternatingEnumerable<byte, byte>(keyGetter));

	[TestMethod, TestCategory(nameof(IEnumerable.GetEnumerator))]
	public void Enumerate_SequenceEquals()
		=> Assert.IsTrue(expected.SequenceEqual(new OrderedAlternatingEnumerable<byte, byte>(keyGetter, testArrayA, testArrayB)));
}
