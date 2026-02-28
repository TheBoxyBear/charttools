using ChartTools.Extensions.Collections.Alternating;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections;

namespace ChartTools.Tests.Extensions.Collections.Alternating;

[TestClass]
public class SerialAlternatingTests
{
	static readonly byte[]
		testArrayA = [1, 6, 2],
		testArrayB = [3, 5, 6],
		expected   = [1, 3, 6, 5, 2, 6];

	[TestMethod, TestCategory("Ctor"), TestCategory(nameof(Exception))]
	public void Ctor_NoEnumerables_Throws()
		=> Assert.Throws<ArgumentException>(
			static () => new SerialAlternatingEnumerable<byte>());

	[TestMethod, TestCategory(nameof(IEnumerable.GetEnumerator))]
	public void Enumerate_SequenceEquals()
		=> Assert.IsTrue(expected.SequenceEqual(new SerialAlternatingEnumerable<byte>(testArrayA, testArrayB)));
}
