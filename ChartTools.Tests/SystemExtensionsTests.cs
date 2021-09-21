using ChartTools.SystemExtensions;
using ChartTools.SystemExtensions.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChartTools.Tests
{
    [TestClass]
    public class SystemExtensionsTests
    {
        static readonly bool[] trueArray = new bool[] { true, true };
        static readonly bool[] falseArray = new bool[] { false, false };
        static readonly bool[] mixBoolArray = new bool[] { true, false };

        [TestMethod] public void AllNoBools() => Assert.AreEqual(false, Array.Empty<bool>().All());
        [TestMethod] public void AllNoFalse() => Assert.AreEqual(true, trueArray.All());
        [TestMethod] public void AllNoTrue() => Assert.AreEqual(false, falseArray.All());
        [TestMethod] public void AllMix() => Assert.AreEqual(false, mixBoolArray.All());

        [TestMethod] public void AnyNoBools() => Assert.AreEqual(false, Array.Empty<bool>().Any());
        [TestMethod] public void AnyNoFalse() => Assert.AreEqual(true, trueArray.Any());
        [TestMethod] public void AnyNoTrue() => Assert.AreEqual(false, falseArray.Any());
        [TestMethod] public void AnyMix() => Assert.AreEqual(true, mixBoolArray.Any());

        [TestMethod] public void FirstOrDefaultNullPredicate() => Assert.ThrowsException<ArgumentNullException>(() => trueArray.FirstOrDefault(null, false));
        [TestMethod] public void OutFirstOrDefaultNullPredicate() => Assert.ThrowsException<ArgumentNullException>(() => trueArray.FirstOrDefault(null, false, out bool returnedDefault));
        [TestMethod] public void FirstOrDefaultExistingItem() => Assert.AreEqual(true, trueArray.FirstOrDefault(b => b, false));
        [TestMethod] public void OutFirstOrDefaultExistingItem()
        {
            Assert.AreEqual(true, trueArray.FirstOrDefault(b => b, false, out bool returnedDefault));
            Assert.IsFalse(returnedDefault);
        }
        [TestMethod] public void FirstOrDefaultNonExistentItem() => Assert.AreEqual(true, trueArray.FirstOrDefault(b => !b, true));
        [TestMethod] public void OutFirstOrDefaultNonExistentItem()
        {
            Assert.AreEqual(true, trueArray.FirstOrDefault(b => !b, true, out bool returnedDefault));
            Assert.IsTrue(returnedDefault);
        }

        [TestMethod] public void TryGetFirstNullPredicate() => Assert.ThrowsException<ArgumentNullException>(() => trueArray.TryGetFirst(null, out bool b));
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

        [TestMethod] public void ReplaceNullPredicate() => Assert.ThrowsException<ArgumentNullException>(() => trueArray.Replace(null, false).ToArray());
        [TestMethod] public void ReplaceNoMatch() => Assert.AreEqual(string.Join(' ', falseArray), string.Join(' ', falseArray.Replace(b => b, true)));
        [TestMethod] public void ReplaceMatch()
        {
            int[] numbers = Enumerable.Range(0, 10).ToArray();
            Assert.AreEqual("0 1 2 3 4 5 0 0 0 0", string.Join(' ', numbers.Replace(n => n > 5, 0)));
        }

        [TestMethod] public void ReplaceSectionNullStartReplace() => Assert.ThrowsException<ArgumentNullException>(() => trueArray.ReplaceSection(Enumerable.Empty<bool>(), null, b => true).ToArray());
        [TestMethod] public void ReplaceSectionNullEndReplace() => Assert.ThrowsException<ArgumentNullException>(() => trueArray.ReplaceSection(Enumerable.Empty<bool>(), b => true, null).ToArray());
        [TestMethod] public void ReplaceSectioNeverStart() => Assert.AreEqual(Formatting.FormatCollection(trueArray), Formatting.FormatCollection(trueArray.ReplaceSection(falseArray, b => false, b => true)));
    }
}
