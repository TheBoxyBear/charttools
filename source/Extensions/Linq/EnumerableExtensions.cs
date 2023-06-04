﻿using ChartTools.Extensions.Collections;

using System.Collections;
using System.Runtime.CompilerServices;

namespace ChartTools.Extensions.Linq;

public static class EnumerableExtensions
{
    /// <summary>
    /// Checks that all booleans in a collection are <see langword="true"/>.
    /// </summary>
    /// <param name="source">Source of booleans</param>
    /// <returns><see langword="true"/> if all booleans are <see langword="true"/> or the collection is empty</returns>
    public static bool All(this IEnumerable<bool> source)
    {
        foreach (bool b in source)
            if (!b)
                return false;

        return true;
    }

    /// <summary>
    /// Checks if any boolean in a collection is <see langword="true"/>.
    /// </summary>
    /// <param name="source">Source of booleans</param>
    public static bool Any(this IEnumerable<bool> source)
    {
        foreach (bool b in source)
            if (b)
                return true;

        return false;
    }

    #region First
    /// <inheritdoc cref="Enumerable.FirstOrDefault{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
    /// <param name="returnedDefault"><see langword="true"/> if no items meeting the condition were found</param>
    public static T? FirstOrDefault<T>(this IEnumerable<T> source, Predicate<T> predicate, T? defaultValue, out bool returnedDefault)
    {
        if (predicate is null)
            throw new ArgumentNullException(nameof(predicate));

        foreach (T item in source)
            if (predicate(item))
            {
                returnedDefault = false;
                return item;
            }

        returnedDefault = true;
        return defaultValue;
    }
    /// <summary>
    /// Tries to get the first item that meet a condition from en enumerable.
    /// </summary>
    /// <param name="predicate">Method that returns <see langword="true"/> if a given item meets the condition</param>
    /// <param name="item">Found item</param>
    /// <returns><see langword="true"/> if an item was found</returns>
    public static bool TryGetFirst<T>(this IEnumerable<T> source, Predicate<T> predicate, out T item)
    {
        if (predicate is null)
            throw new ArgumentNullException(nameof(predicate));

        foreach (T t in source)
            if (predicate(t))
            {
                item = t;
                return true;
            }

        item = default!;
        return false;
    }
    /// <summary>
    /// Tries to get the first element of a collection.
    /// </summary>
    /// <param name="source">Source of items</param>
    /// <param name="result">Found item</param>
    /// <returns><see langword="true"/> if an item was found</returns>
    public static bool TryGetFirst<T>(this IEnumerable<T> source, out T result)
    {
        using var enumerator = source.GetEnumerator();
        var success = enumerator.MoveNext();

        result = success ? enumerator.Current : default!;
        return success;
    }
    /// <summary>
    /// Tries to get the first item of a given type in a collection.
    /// </summary>
    /// <param name="source">Source of items</param>
    /// <param name="result">Found item</param>
    /// <returns><see langword="true"/> if an item was found</returns>
    public static bool TryGetFirstOfType<TResult>(this IEnumerable source, out TResult result) => source.OfType<TResult>().TryGetFirst(out result);
    #endregion

    /// <summary>
    /// Excludes <see langword="null"/> items.
    /// </summary>
    public static IEnumerable<T> NonNull<T>(this IEnumerable<T?> source) => source.Where(t => t is not null)!;
    public static IEnumerable<T> NonNull<T>(this IEnumerable<T?> source) where T : struct
    {
        foreach (var item in source)
            if (item.HasValue)
                yield return item.Value;
    }

    #region Replace
    /// <summary>
    /// Replaces items that meet a condition with another item.
    /// </summary>
    /// <param name="source">The IEnumerable&lt;out T&gt; to replace the items of</param>
    /// <param name="predicate">A function that determines if an item must be replaced</param>
    /// <param name="replacement">The item to replace items with</param>
    public static IEnumerable<T> Replace<T>(this IEnumerable<T> source, Predicate<T> predicate, T replacement)
    {
        if (predicate is null)
            throw new ArgumentNullException(nameof(predicate));

        foreach (T item in source)
            yield return predicate(item) ? replacement : item;
    }

    /// <summary>
    /// Replaces a section with other items.
    /// </summary>
    /// <remarks>Items that match startReplace or endReplace are not included in the returned items.</remarks>
    /// <param name="source">Items to replace a section in</param>
    public static IEnumerable<T> ReplaceSection<T>(this IEnumerable<T> source, SectionReplacement<T> replacement)
    {
        if (replacement.StartReplace is null)
            throw new NullReferenceException(nameof(replacement.StartReplace));
        if (replacement.EndReplace is null)
            throw new NullReferenceException(nameof(replacement.EndReplace));

        IEnumerator<T> itemsEnumerator = source.GetEnumerator();

        // Initialize the enumerator
        if (!itemsEnumerator.MoveNext())
        {
            // Return the replacement
            if (replacement.AddIfMissing)
                foreach (T item in replacement.Replacement)
                    yield return item;

            yield break;
        }

        // Return original until startReplace
        while (!replacement.StartReplace(itemsEnumerator.Current))
        {
            yield return itemsEnumerator.Current;

            if (!itemsEnumerator.MoveNext())
            {
                // Return the replacement
                if (replacement.AddIfMissing)
                    foreach (T item in replacement.Replacement)
                        yield return item;
                yield break;
            }
        }

        // Return replacement
        foreach (T item in replacement.Replacement)
            yield return item;

        // Find the end of the section to replace
        do
            if (!itemsEnumerator.MoveNext())
                yield break;
        while (replacement.EndReplace(itemsEnumerator.Current));

        // Return the rest
        while (itemsEnumerator.MoveNext())
            yield return itemsEnumerator.Current;
    }

    /// <summary>
    /// Replaces multiple sections of items.
    /// </summary>
    /// <remarks>Items that match startReplace or endReplace are not included in the returned items.</remarks>
    /// <param name="source">Items to replace sections in</param>
    public static IEnumerable<T> ReplaceSections<T>(this IEnumerable<T> source, IEnumerable<SectionReplacement<T>> replacements)
    {
        if (replacements is null || !replacements.Any())
        {
            foreach (T item in source)
                yield return item;
            yield break;
        }

        List<SectionReplacement<T>> replacementList = replacements.ToList();
        using IEnumerator<T> itemsEnumerator = source.GetEnumerator();

        if (!itemsEnumerator.MoveNext())
        {
            foreach (var item in AddMissing())
                yield return item;

            yield break;
        }

        do
        {
            // Find a matching replacement start
            if (replacementList.TryGetFirst(r => r.StartReplace(itemsEnumerator.Current), out var replacement))
            {
                // Move to the end of the section to replace
                do
                    if (!itemsEnumerator.MoveNext())
                    {
                        foreach (var item in AddMissing())
                            yield return item;
                        yield break;
                    }
                while (!replacement.EndReplace(itemsEnumerator.Current));

                // Return the replacement
                foreach (T item in replacement.Replacement)
                    yield return item;

                replacementList.Remove(replacement);
            }
            else
            {
                yield return itemsEnumerator.Current;

                if (!itemsEnumerator.MoveNext())
                {
                    foreach (var item in AddMissing())
                        yield return item;
                    yield break;
                }
            }
        }
        // Continue until all replacements are applied
        while (replacementList.Count > 0);

        // Return the rest of the items
        while (itemsEnumerator.MoveNext())
            yield return itemsEnumerator.Current;

        IEnumerable<T> AddMissing()
        {
            // Return remaining replacements
            foreach (var replacement in replacementList.Where(r => r.AddIfMissing))
                // Return the replacement
                foreach (T item in replacement.Replacement)
                    yield return item;
        }
    }
    /// <summary>
    /// Removes a section of items.
    /// </summary>
    /// <remarks>Items that match startRemove or endRemove</remarks>
    /// <param name="source">Source items to remove a section of</param>
    /// <param name="startRemove">Function that determines the start of the section to replace</param>
    /// <param name="endRemove">Function that determines the end of the section to replace</param>
    public static IEnumerable<T> RemoveSection<T>(this IEnumerable<T> source, Predicate<T> startRemove, Predicate<T> endRemove)
    {
        IEnumerator<T> itemsEnumerator = source.GetEnumerator();

        // Initialize the enumerator
        if (!itemsEnumerator.MoveNext())
            yield break;

        // Move to the start of items to remove
        while (!startRemove(itemsEnumerator.Current))
            if (!itemsEnumerator.MoveNext())
                yield break;

        // Skip items to remove
        do
            if (!itemsEnumerator.MoveNext())
                yield break;
        while (!endRemove(itemsEnumerator.Current));

        // Return the rest
        while (itemsEnumerator.MoveNext())
            yield return itemsEnumerator.Current;
    }
    #endregion

    /// <summary>
    /// Loops through a set of objects and returns a set of tuples containing the current object and the previous one.
    /// </summary>
    /// <param name="source">Items to loop through</param>
    /// <param name="firstPrevious">Value of the previous item in the first call of the action</param>
    public static IEnumerable<(T? previous, T current)> RelativeLoop<T>(this IEnumerable<T> source, T? firstPrevious = default)
    {
        var previousItem = firstPrevious;

        foreach (var item in source)
        {
            yield return (previousItem, item);
            previousItem = item;
        }
    }
    public static IEnumerable<(T previous, T current)> RelativeLoopSkipFirst<T>(this IEnumerable<T> source)
    {
        using var enumerator = source.GetEnumerator();

        if (enumerator.MoveNext())
            yield break;

        var previous = enumerator.Current;

        while (enumerator.MoveNext())
            yield return (previous, enumerator.Current);
    }

    #region Unique
    /// <summary>
    /// Returns distinct elements of a sequence using a method to determine the equality of elements
    /// </summary>
    /// <param name="comparison">Method that determines if two elements are the same</param>
    public static IEnumerable<T> Distinct<T>(this IEnumerable<T> source, EqualityComparison<T?> comparison) => source.Distinct(new FuncEqualityComparer<T>(comparison));
    public static bool Unique<T>(this IEnumerable<T> source) => UniqueFromDistinct(source.Distinct());
    public static bool UniqueBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector) => UniqueFromDistinct(source.DistinctBy(selector));
    private static bool UniqueFromDistinct<T>(IEnumerable<T> distinct) => !distinct.Skip(1).Any();
    #endregion

    #region MinMax
    /// <summary>
    /// Finds the items for which a function returns the smallest or greatest value based on a comparison.
    /// </summary>
    /// <param name="source">Items to find the minimum or maximum of</param>
    /// <param name="selector">Function that gets the key to use in the comparison from an item</param>
    /// <param name="comparison">Function that returns <see langword="true"/> if the second item defeats the first</param>
    private static IEnumerable<T> ManyMinMaxBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector, Func<TKey, TKey, bool> comparison) where TKey : IComparable<TKey>
    {
        TKey minMaxKey;

        using (IEnumerator<T> enumerator = source.GetEnumerator())
        {
            if (!enumerator.MoveNext())
                throw new ArgumentException("The enumerable has no items.", nameof(source));

            minMaxKey = selector(enumerator.Current);

            while (enumerator.MoveNext())
            {
                TKey key = selector(enumerator.Current);

                if (comparison(key, minMaxKey))
                    minMaxKey = key;
            }
        }
        return source.Where(t => selector(t).CompareTo(minMaxKey) == 0);
    }
    /// <summary>
    /// Finds the items for which a function returns the smallest value.
    /// </summary>
    /// <param name="source">Items to find the minimum or maximum of</param>
    /// <param name="selector">Function that gets the key to use in the comparison from an item</param>
    public static IEnumerable<T> ManyMinBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector) where TKey : IComparable<TKey> => ManyMinMaxBy(source, selector, (key, mmkey) => key.CompareTo(mmkey) < 0);
    /// <summary>
    /// Finds the items for which a function returns the greatest value.
    /// </summary>
    /// <param name="source">Items to find the minimum or maximum of</param>
    /// <param name="selector">Function that gets the key to use in the comparison from an item</param>
    public static IEnumerable<T> ManyMaxBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector) where TKey : IComparable<TKey> => ManyMinMaxBy(source, selector, (key, mmkey) => key.CompareTo(mmkey) > 0);
    #endregion

    #region Collections
    /// <summary>
    /// Alternates items from multiple sources.
    /// </summary>
    /// <param name="sources">Sources of items in order to alternate in</param>
    public static IEnumerable<T> Alternate<T>(this IEnumerable<IEnumerable<T>> sources) => new SerialAlternatingEnumerable<T>(sources.ToArray());
    /// <summary>
    /// Alternates items from multiple sources in order based on a key.
    /// </summary>
    /// <param name="sources">Sources of items to alternate</param>
    /// <param name="selector">Function returning to key to use for ordering items</param>
    public static IEnumerable<T> AlternateBy<T, TKey>(this IEnumerable<IEnumerable<T>> sources, Func<T, TKey> selector) where TKey : IComparable<TKey> => new OrderedAlternatingEnumerable<T, TKey>(selector, sources.ToArray());
    #endregion
}
