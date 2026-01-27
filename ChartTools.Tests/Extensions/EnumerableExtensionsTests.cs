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
	public void FirstOrDefault_ExistingItem_ReturnsItem()
	{
		Assert.AreEqual(true, trueArray.FirstOrDefault(b => b, false, out bool returnedDefault));
		Assert.IsFalse(returnedDefault);
	}

	[TestMethod, TestCategory(nameof(EnumerableExtensions.FirstOrDefault))]
	public void FirstOrDefault_MissingItem_ReturnsDefault()
	{
		Assert.AreEqual(true, trueArray.FirstOrDefault(b => !b, true, out bool returnedDefault));
		Assert.IsTrue(returnedDefault);
	}

	[TestMethod, TestCategory(nameof(EnumerableExtensions.TryGetFirst)), TestCategory(nameof(Exception))]
	public void TryGetFirst_NullPredicate_Throws()
		=> Assert.ThrowsException<ArgumentNullException>(
			() => trueArray.TryGetFirst(null!, out bool b));

	[TestMethod, TestCategory(nameof(EnumerableExtensions.TryGetFirst))]
	public void TryGetFirst_NoItems_ReturnsDefault()
	{
		Assert.IsFalse(Array.Empty<bool>().TryGetFirst(b => b, out bool item));
		Assert.AreEqual(default, item);
	}

	[TestMethod, TestCategory(nameof(EnumerableExtensions.TryGetFirst))]
	public void TryGetFirst_MissingItem_ReturnsDefault()
	{
		Assert.IsFalse(falseArray.TryGetFirst(b => b, out bool item));
		Assert.AreEqual(default, item);
	}

	[TestMethod, TestCategory(nameof(EnumerableExtensions.TryGetFirst))]
	public void TryGetFirst_ExistingItem_ReturnsItem()
	{
		Assert.IsTrue(trueArray.TryGetFirst(b => b, out bool item));
		Assert.AreEqual(true, item);
	}

	[TestMethod, TestCategory(nameof(EnumerableExtensions.Replace)), TestCategory(nameof(Exception))]
	public void Replace_NullPredicate_Throws()
		=> Assert.ThrowsException<ArgumentNullException>(
			() => trueArray.Replace(null!, false).ToArray());

	[TestMethod, TestCategory(nameof(EnumerableExtensions.Replace))]
	public void Replace_NoMatch_ReturnsSource()
		=> Assert.IsTrue(falseArray.SequenceEqual(falseArray.Replace(b => b, true)));

	[TestMethod, TestCategory(nameof(EnumerableExtensions.Replace))]
	public void Replace_Match_Replaces()
	{
		int[]
			numbers  = [.. Enumerable.Range(0, 10)],
			expected = [0, 1, 2, 3, 4, 5, 0, 0, 0, 0];


		Assert.IsTrue(expected.SequenceEqual(numbers.Replace(n => n > 5, 0)));
	}

	[TestMethod, TestCategory(nameof(EnumerableExtensions.ReplaceSection))]
	public void ReplaceSection_NullStartReplace_Throws()
		=> Assert.ThrowsException<NullReferenceException>(
			() => trueArray.ReplaceSection(new([], null!, b => true, true)).ToArray());

	[TestMethod, TestCategory(nameof(EnumerableExtensions.ReplaceSection))]
	public void ReplaceSection_NullEndReplace_Throws()
		=> Assert.ThrowsException<NullReferenceException>(
			() => trueArray.ReplaceSection(new([], b => true, null!, true)).ToArray());

	[TestMethod, TestCategory(nameof(EnumerableExtensions.ReplaceSection))]
	public void ReplaceSection_NeverStarts_ReturnsSource()
		=> Assert.IsTrue(trueArray.SequenceEqual(trueArray.ReplaceSection(new(falseArray, b => false, b => true, false))));
}
