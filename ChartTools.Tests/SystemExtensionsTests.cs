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
    }
}
