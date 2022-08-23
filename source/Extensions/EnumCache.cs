using System;
using System.Linq;

namespace ChartTools.SystemExtensions
{
    internal static class EnumCache<T> where T : struct, Enum
    {
        public static T[] Values => _values ??= Enum.GetValues<T>().ToArray();
        private static T[]? _values;

        public static void Clear() => _values = null;
    }
}
