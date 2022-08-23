using System;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools.Extensions
{
    /// <summary>
    /// Provides additional methods to string
    /// </summary>
    internal static class StringExtensions
    {
        /// <inheritdoc cref="VerbalEnumerate(string, string[])"/>
        public static string VerbalEnumerate(this IEnumerable<string> items, string lastItemPreceder) => VerbalEnumerate(lastItemPreceder, items.ToArray());
        /// <summary>
        /// Enumerates items with commas and a set word preceding the last item.
        /// </summary>
        /// <param name="lastItemPreceder">Word to place before the last item</param>
        /// <exception cref="ArgumentNullException"/>
        public static string VerbalEnumerate(string lastItemPreceder, params string[] items) => items is null ? throw new ArgumentNullException(nameof(items)) : items.Length switch
        {
            0 => string.Empty, // ""
            1 => items[0], // "Item1"
            2 => $"{items[0]} {lastItemPreceder} {items[1]}", // "Item1 lastItemPreceder Item2"
            _ => $"{string.Join(", ", items, items.Length - 1)} {lastItemPreceder} {items[^0]}" // "Item1, Item2 lastItemPreceder Item3"
        };
    }
}
