using ChartTools.Extensions.Linq;

namespace ChartTools.Tests.Extensions;

[TestClass]
public class EnumerableExtensionsTests
{
	static readonly bool[]
		trueArray  = [true, true],
		falseArray = [false, false];

	#region FirstOrDefault
	[TestMethod, TestCategory(nameof(EnumerableExtensions.FirstOrDefault)), TestCategory(nameof(Exception))]
	public void FirstOrDefault_NullSource_Throws()
		=> Assert.Throws<ArgumentNullException>(
			static () => EnumerableExtensions.FirstOrDefault(null!, static b => b, false, out bool returnedDefault));

	[TestMethod, TestCategory(nameof(EnumerableExtensions.FirstOrDefault)), TestCategory(nameof(Exception))]
	public void FirstOrDefault_NullPredicate_Throws()
		=> Assert.Throws<ArgumentNullException>(
			() => EnumerableExtensions.FirstOrDefault(trueArray, null!, false, out bool returnedDefault));

	[TestMethod, TestCategory(nameof(EnumerableExtensions.FirstOrDefault))]
	public void FirstOrDefault_NoItems_ReturnsTrueDefault()
	{
		Assert.IsFalse(EnumerableExtensions.FirstOrDefault([], static b => b, false, out bool returnedDefault));
		Assert.IsTrue(returnedDefault);
	}

	[TestMethod, TestCategory(nameof(EnumerableExtensions.FirstOrDefault))]
	public void FirstOrDefault_Match_ReturnsFalseItem()
	{
		Assert.IsTrue(EnumerableExtensions.FirstOrDefault(trueArray, static b => b, false, out bool returnedDefault));
		Assert.IsFalse(returnedDefault);
	}

	[TestMethod, TestCategory(nameof(EnumerableExtensions.FirstOrDefault))]
	public void FirstOrDefault_NoMatch_ReturnsTrueDefault()
	{
		Assert.IsTrue(EnumerableExtensions.FirstOrDefault(trueArray, static b => !b, true, out bool returnedDefault));
		Assert.IsTrue(returnedDefault);
	}
	#endregion

	#region TryGetFirst
	[TestMethod, TestCategory(nameof(EnumerableExtensions.TryGetFirst)), TestCategory(nameof(Exception))]
	public void TryGetFirst_NullSource_Throws()
		=> Assert.Throws<ArgumentNullException>(
			static () => EnumerableExtensions.TryGetFirst(null!, out bool b));

	[TestMethod, TestCategory(nameof(EnumerableExtensions.TryGetFirst))]
	public void TryGetFirst_NoItems_ReturnsFalseDefault()
	{
		Assert.IsFalse(EnumerableExtensions.TryGetFirst([], out bool item));
		Assert.AreEqual(default, item);
	}

	[TestMethod, TestCategory(nameof(EnumerableExtensions.TryGetFirst)), TestCategory(nameof(Exception))]
	public void TryGetFirstPredicate_NullSource_Throws()
		=> Assert.Throws<ArgumentNullException>(
			static () => EnumerableExtensions.TryGetFirst(null!, static b => b, out bool b));

	[TestMethod, TestCategory(nameof(EnumerableExtensions.TryGetFirst)), TestCategory(nameof(Exception))]
	public void TryGetFirstPredicate_NullPredicate_Throws()
		=> Assert.Throws<ArgumentNullException>(
			() => EnumerableExtensions.TryGetFirst(trueArray, null!, out bool b));

	[TestMethod, TestCategory(nameof(EnumerableExtensions.TryGetFirst))]
	public void TryGetFirstPredicate_NoItems_ReturnsFalseDefault()
	{
		Assert.IsFalse(EnumerableExtensions.TryGetFirst([], static b => b, out bool item));
		Assert.AreEqual(default, item);
	}

	[TestMethod, TestCategory(nameof(EnumerableExtensions.TryGetFirst))]
	public void TryGetFirstPredicate_Match_ReturnsTrueItem()
	{
		Assert.IsTrue(EnumerableExtensions.TryGetFirst(trueArray, static b => b, out bool item));
		Assert.IsTrue(item);
	}

	[TestMethod, TestCategory(nameof(EnumerableExtensions.TryGetFirst))]
	public void TryGetFirstPredicate_NoMatch_ReturnsFalseDefault()
	{
		Assert.IsFalse(EnumerableExtensions.TryGetFirst(falseArray, static b => b, out bool item));
		Assert.AreEqual(default, item);
	}
	#endregion

	#region TryGetFirstOfType
	[TestMethod, TestCategory(nameof(EnumerableExtensions.TryGetFirstOfType)), TestCategory(nameof(Exception))]
	public void TryGetFirstOfType_NullSource_Throws()
		=> Assert.Throws<ArgumentNullException>(
			static () => EnumerableExtensions.TryGetFirstOfType<object>(null!, out object? obj));

	[TestMethod, TestCategory(nameof(EnumerableExtensions.TryGetFirstOfType))]
	public void TryGetFirstOfType_NoItems_ReturnsFalseNull()
	{
		Assert.IsFalse(EnumerableExtensions.TryGetFirstOfType<object>(Enumerable.Empty<object>(), out object? obj));
		Assert.IsNull(obj);
	}

	[TestMethod, TestCategory(nameof(EnumerableExtensions.TryGetFirstOfType))]
	public void TryGetFirstOfType_Match_ReturnsTrueItem()
	{
		Assert.IsTrue(EnumerableExtensions.TryGetFirstOfType(trueArray, out bool value));
		Assert.IsTrue(value);
	}

	[TestMethod, TestCategory(nameof(EnumerableExtensions.TryGetFirstOfType))]
	public void TryGetFirstOfType_NoMatch_ReturnsFalseDefault()
	{
		Assert.IsFalse(EnumerableExtensions.TryGetFirstOfType(trueArray, out int value));
		Assert.AreEqual(default, value);
	}
	#endregion

	#region Replace
	[TestMethod, TestCategory(nameof(EnumerableExtensions.Replace)), TestCategory(nameof(Exception))]
	public void Replace_NullSource_Throws()
		=> Assert.Throws<ArgumentNullException>(
			static () => EnumerableExtensions.Replace(null!, static b => b, false).ToArray());

	[TestMethod, TestCategory(nameof(EnumerableExtensions.Replace)), TestCategory(nameof(Exception))]
	public void Replace_NullPredicate_Throws()
		=> Assert.Throws<ArgumentNullException>(
			() => EnumerableExtensions.Replace(trueArray, null!, false).ToArray());

	[TestMethod, TestCategory(nameof(EnumerableExtensions.Replace))]
	public void Replace_Match_Replaces()
	{
		int[]
			numbers  = [.. Enumerable.Range(0, 10)],
			expected = [0, 1, 2, 3, 4, 5, 0, 0, 0, 0];

		Assert.IsTrue(expected.SequenceEqual(EnumerableExtensions.Replace(numbers, static n => n > 5, 0)));
	}

	[TestMethod, TestCategory(nameof(EnumerableExtensions.Replace))]
	public void Replace_NoMatch_ReturnsSource()
		=> Assert.IsTrue(falseArray.SequenceEqual(EnumerableExtensions.Replace(falseArray, static b => b, true)));
	#endregion

	#region ReplaceSection
	[TestMethod, TestCategory(nameof(EnumerableExtensions.ReplaceSection))]
	public void ReplaceSection_NullSource_Throws()
		=> Assert.Throws<NullReferenceException>(
			static () => EnumerableExtensions.ReplaceSection<bool>(null!, new([], null!, static b => true, true)).ToArray());

	[TestMethod, TestCategory(nameof(EnumerableExtensions.ReplaceSection))]
	public void ReplaceSection_NullStartReplace_Throws()
		=> Assert.Throws<NullReferenceException>(
			() => EnumerableExtensions.ReplaceSection(trueArray, new([], null!, static b => true, true)).ToArray());

	[TestMethod, TestCategory(nameof(EnumerableExtensions.ReplaceSection))]
	public void ReplaceSection_NullEndReplace_Throws()
		=> Assert.Throws<NullReferenceException>(
			() => EnumerableExtensions.ReplaceSection(trueArray, new([], static b => true, null!, true)).ToArray());

	[TestMethod, TestCategory(nameof(EnumerableExtensions.ReplaceSection))]
	public void ReplaceSection_NeverStarts_ReturnsSource()
		=> Assert.IsTrue(trueArray.SequenceEqual(EnumerableExtensions.ReplaceSection(trueArray, new(falseArray, static b => false, static b => true, false))));
	#endregion
}
