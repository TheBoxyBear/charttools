using ChartTools.Extensions;
using ChartTools.Extensions.Collections;
using ChartTools.Extensions.Collections.Unique;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChartTools.Tests;

[TestClass]
public class UniqueTests
{
    static readonly byte[] testItems = new byte[] { 1, 2, 2, 5, 2, 5, 7 };
    static readonly byte[] expectedArray = new byte[] { 1, 2, 5, 7 };
    static readonly Func<byte, byte> comparison = a => a;
    const string expectedString = "1 2 5 7";
    const byte missingItem = 10;

    [TestMethod] public void Count() => Assert.AreEqual(expectedArray.Length, GetList().Count);
    [TestMethod] public void CreateListNullSelector() => Assert.ThrowsException<ArgumentNullException>(() => new UniqueCollection<byte, byte>(null!));
    [TestMethod] public void CreateListStartingItems() => Assert.AreEqual(expectedString, Formatting.FormatCollection(new UniqueSelfCollection<byte>(testItems)));

    [TestMethod] public void Add()
    {
        var list = new UniqueCollection<byte, byte>(comparison);

        foreach (byte item in testItems)
            list.Add(item);

        Assert.AreEqual(expectedString, Formatting.FormatCollection(list));
    }
    [TestMethod] public void AddRange()
    {
        var list = new UniqueCollection<byte, byte>(comparison);
        list.AddRange(testItems);

        Assert.AreEqual(expectedString, Formatting.FormatCollection(list));
    }
    [TestMethod] public void Clear()
    {
        var list = GetList();
        list.Clear();

        string result = Formatting.FormatCollection(list);

        Assert.AreEqual(string.Empty, result);
    }
    [TestMethod] public void Contains()
    {
        var list = GetList();

        foreach (byte item in testItems)
            Assert.IsTrue(list.Contains(item), $"List did not report {item} as being contained.");
    }
    [TestMethod] public void ContainsMissingItem() => Assert.IsFalse(GetList().Contains(missingItem), $"List reported {missingItem} as being contained despite not being in the list");
    [TestMethod] public void CopyTo()
    {
        var list = GetList();
        byte[] result = new byte[4];

        list.CopyTo(result, 0);

        Assert.AreEqual(Formatting.FormatCollection(expectedArray),  Formatting.FormatCollection(result));
    }
    [TestMethod] public void RemoveMissingItem() => Assert.IsFalse(GetList().Remove(missingItem));
    [TestMethod] public void RemoveExistingItem()
    {
        var list = GetList();

        Assert.IsTrue(list.Remove(5));
        Assert.AreEqual("1 2 7", Formatting.FormatCollection(list));
    }
    private static UniqueSelfCollection<byte> GetList() => new(testItems);
}
