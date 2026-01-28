using ChartTools.Extensions.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChartTools.Tests.Extensions;

[TestClass]
public class EnumerableExtensionsTests
{
	static readonly bool[]
		trueArray  = [true, true],
		falseArray = [false, false];

	[TestMethod, TestCategory(nameof(EnumerableExtensions.FirstOrDefault)), TestCategory(nameof(Exception))]
	public void FirstOrDefault_NullSource_Throws()
		=> Assert.ThrowsException<ArgumentNullException>(
			() => EnumerableExtensions.FirstOrDefault(null!, b => b, false, out bool returnedDefault));

	[TestMethod, TestCategory(nameof(EnumerableExtensions.FirstOrDefault)), TestCategory(nameof(Exception))]
	public void FirstOrDefault_NullPredicate_Throws()
		=> Assert.ThrowsException<ArgumentNullException>(
			() => EnumerableExtensions.FirstOrDefault( trueArray, null!, false, out bool returnedDefault));

	[TestMethod, TestCategory(nameof(EnumerableExtensions.FirstOrDefault))]
	public void FirstOrDefault_Match_ReturnsFalseItem()
	{
		Assert.AreEqual(true, EnumerableExtensions.FirstOrDefault(trueArray, b => b, false, out bool returnedDefault));
		Assert.IsFalse(returnedDefault);
	}

	[TestMethod, TestCategory(nameof(EnumerableExtensions.FirstOrDefault))]
	public void FirstOrDefault_NoMatch_ReturnsTrueDefault()
	{
		Assert.AreEqual(true, EnumerableExtensions.FirstOrDefault(trueArray, b => !b, true, out bool returnedDefault));
		Assert.IsTrue(returnedDefault);
	}

	[TestMethod, TestCategory(nameof(EnumerableExtensions.TryGetFirst)), TestCategory(nameof(Exception))]
	public void TryGetFirst_NullSource_Throws()
	=> Assert.ThrowsException<ArgumentNullException>(
		() => EnumerableExtensions.TryGetFirst(null!, out bool b));

	[TestMethod, TestCategory(nameof(EnumerableExtensions.TryGetFirst))]
	public void TryGetFirst_NoItems_ReturnsFalseDefault()
	{
		Assert.IsFalse(EnumerableExtensions.TryGetFirst([], out bool item));
		Assert.AreEqual(default, item);
	}

	[TestMethod, TestCategory(nameof(EnumerableExtensions.TryGetFirst))]
	public void TryGetFirstPredicate_NoItems_ReturnsFalseDefault()
	{
		Assert.IsFalse(EnumerableExtensions.TryGetFirst([], b => b, out bool item));
		Assert.AreEqual(default, item);
	}

	[TestMethod, TestCategory(nameof(EnumerableExtensions.TryGetFirst))]
	public void TryGetFirstPredicate_Match_ReturnsTrueItem()
	{
		Assert.IsTrue(EnumerableExtensions.TryGetFirst(trueArray, b => b, out bool item));
		Assert.AreEqual(true, item);
	}

	[TestMethod, TestCategory(nameof(EnumerableExtensions.Replace)), TestCategory(nameof(Exception))]
	public void Replace_NullSource_Throws()
		=> Assert.ThrowsException<ArgumentNullException>(
			() => EnumerableExtensions.Replace(null!, b => b, false).ToArray());

	[TestMethod, TestCategory(nameof(EnumerableExtensions.Replace))]
	public void Replace_NoMatch_ReturnsSource()
		=> Assert.IsTrue(falseArray.SequenceEqual(falseArray.Replace(b => b, true)));

	[TestMethod, TestCategory(nameof(EnumerableExtensions.Replace))]
	public void Replace_Match_Replaces()
	{
		int[]
			numbers = [.. Enumerable.Range(0, 10)],
			expected = [0, 1, 2, 3, 4, 5, 0, 0, 0, 0];

		Assert.IsTrue(expected.SequenceEqual(EnumerableExtensions.Replace(numbers, n => n > 5, 0)));
	}

	[TestMethod, TestCategory(nameof(EnumerableExtensions.ReplaceSection))]
	public void ReplaceSection_NullStartReplace_Throws()
		=> Assert.ThrowsException<NullReferenceException>(
			() => EnumerableExtensions.ReplaceSection(trueArray, new([], null!, b => true, true)).ToArray());

	[TestMethod, TestCategory(nameof(EnumerableExtensions.ReplaceSection))]
	public void ReplaceSection_NullEndReplace_Throws()
		=> Assert.ThrowsException<NullReferenceException>(
			() => EnumerableExtensions.ReplaceSection(trueArray, new([], b => true, null!, true)).ToArray());

	[TestMethod, TestCategory(nameof(EnumerableExtensions.ReplaceSection))]
	public void ReplaceSection_NeverStarts_ReturnsSource()
		=> Assert.IsTrue(trueArray.SequenceEqual(EnumerableExtensions.ReplaceSection(trueArray, new(falseArray, b => false, b => true, false))));
}
