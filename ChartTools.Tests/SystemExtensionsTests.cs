using ChartTools.Extensions.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChartTools.Tests;

[TestClass]
public class SystemExtensionsTests
{
	static readonly bool[] trueArray = [true, true];
	static readonly bool[] falseArray = [false, false];

	[TestMethod] public void FirstOrDefaultNullPredicate()
		=> Assert.ThrowsException<ArgumentNullException>(() => trueArray.FirstOrDefault(null!, false));

	[TestMethod] public void OutFirstOrDefaultNullPredicate()
		=> Assert.ThrowsException<ArgumentNullException>(() => trueArray.FirstOrDefault(null!, false, out bool returnedDefault));

	[TestMethod] public void FirstOrDefaultExistingItem()
		=> Assert.AreEqual(true, trueArray.FirstOrDefault(b => b, false));

	[TestMethod] public void OutFirstOrDefaultExistingItem()
	{
		Assert.AreEqual(true, trueArray.FirstOrDefault(b => b, false, out bool returnedDefault));
		Assert.IsFalse(returnedDefault);
	}

	[TestMethod] public void FirstOrDefaultNonExistentItem()
		=> Assert.AreEqual(true, trueArray.FirstOrDefault(b => !b, true));

	[TestMethod] public void OutFirstOrDefaultNonExistentItem()
	{
		Assert.AreEqual(true, trueArray.FirstOrDefault(b => !b, true, out bool returnedDefault));
		Assert.IsTrue(returnedDefault);
	}

	[TestMethod] public void TryGetFirstNullPredicate()
		=> Assert.ThrowsException<ArgumentNullException>(() => trueArray.TryGetFirst(null!, out bool b));

	[TestMethod] public void TryGetFirstNoItems()
	{
		Assert.IsFalse(Array.Empty<bool>().TryGetFirst(b => b, out bool item));
		Assert.AreEqual(default, item);
	}

	[TestMethod] public void TryGetFirstNonExistentItem()
	{
		Assert.IsFalse(falseArray.TryGetFirst(b => b, out bool item));
		Assert.AreEqual(default, item);
	}

	[TestMethod] public void TryGetFirstExistentItem()
	{
		Assert.IsTrue(trueArray.TryGetFirst(b => b, out bool item));
		Assert.AreEqual(true, item);
	}

	[TestMethod] public void ReplaceNullPredicate()
		=> Assert.ThrowsException<ArgumentNullException>(() => trueArray.Replace(null!, false).ToArray());

	[TestMethod] public void ReplaceNoMatch()
		=> Assert.IsTrue(falseArray.SequenceEqual(falseArray.Replace(b => b, true)));

	[TestMethod] public void ReplaceMatch()
	{
		int[]
			numbers  = [.. Enumerable.Range(0, 10)],
			expected = [0, 1, 2, 3, 4, 5, 0, 0, 0, 0];


		Assert.IsTrue(expected.SequenceEqual(numbers.Replace(n => n > 5, 0)));
	}

	[TestMethod] public void ReplaceSectionNullStartReplace()
		=> Assert.ThrowsException<NullReferenceException>(() => trueArray.ReplaceSection(new([], null!, b => true, true)).ToArray());

	[TestMethod] public void ReplaceSectionNullEndReplace()
		=> Assert.ThrowsException<NullReferenceException>(() => trueArray.ReplaceSection(new([], b => true, null!, true)).ToArray());

	[TestMethod] public void ReplaceSectionNeverStart()
		=> Assert.IsTrue(trueArray.SequenceEqual(trueArray.ReplaceSection(new(falseArray, b => false, b => true, false))));
}
