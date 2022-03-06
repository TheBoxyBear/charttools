using System;
using System.Collections.Generic;
using System.Linq;
using ChartTools.IO;
using ChartTools.IO.Chart;
using ChartTools.Lyrics;
using ChartTools.Collections.Alternating;
using System.Threading.Tasks;
using System.Collections;
using ChartTools.IO.Configuration;
using System.Threading;
using ChartTools.Events;

namespace ChartTools.SystemExtensions
{
    /// <summary>
    /// <see cref="IEquatable{T}"/> equivalent to the <see cref="IComparable{T}"/> <see cref="Comparison{T}"/> delegate
    /// </summary>
    public delegate bool EqualityComparison<in T>(T a, T b);

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

    internal static class TaskExtensions
    {
        // Credit: https://stackoverflow.com/a/46962416/8078210
        public static T SyncResult<T>(this Task<T> task)
        {
            task.RunSynchronously();
            return task.Result;
        }
    }
}
namespace ChartTools.SystemExtensions.Linq
{
    public record SectionReplacement<T>(Func<IEnumerable<T>> ReplacementGetter, Predicate<T> StartReplace, Predicate<T> EndReplace);

    /// <summary>
    /// Provides additional methods to Linq
    /// </summary>
    public static class LinqExtensions
    {
        public static bool All(this IEnumerable<bool> source)
        {
            bool containsItems = false;

            foreach (bool b in source)
            {
                containsItems = true;

                if (!b)
                    return false;
            }

            return containsItems;
        }
        public static bool Any(this IEnumerable<bool> source)
        {
            foreach (bool b in source)
                if (b)
                    return true;

            return false;
        }

        /// <inheritdoc cref="FirstOrDefault{T}(IEnumerable{T}, Predicate{T}, T)"/>
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
        /// Excludes <see langword="null"/> items.
        /// </summary>
        public static IEnumerable<T> NonNull<T>(this IEnumerable<T?> source) => source.Where(t => t is not null)!;

        /// <summary>
        /// Tries to get the first item that meet a condition from en enumerable.
        /// </summary>
        /// <param name="predicate">Method that returns <see langword="true"/> if a given item meets the condition</param>
        /// <param name="item">Found item</param>
        /// <returns><see langword="true"/> if an item was found</returns>
        public static bool TryGetFirst<T>(this IEnumerable<T> source, Predicate<T> predicate, out T? item)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            foreach (T t in source)
                if (predicate(t))
                {
                    item = t;
                    return true;
                }

            item = default;
            return false;
        }

        /// <summary>
        /// Returns distinct elements of a sequence using a method to determine the equality of elements
        /// </summary>
        /// <param name="comparison">Method that determines if two elements are the same</param>
        /// <inheritdoc cref="Enumerable.Distinct{TSource}(IEnumerable{TSource})" path="/exception"/>
        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> source, EqualityComparison<T?> comparison) => source.Distinct(new FuncEqualityComparer<T>(comparison));

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
        /// <param name="replacement">Items to replace the section with</param>
        /// <param name="startReplace">Function that determines if an item if the first to be replaced</param>
        /// <param name="endReplace">Function that determines where to stop excluding items from the source</param>
        /// <param name="addIfMissing">Add the replacement to the end of the items if startReplace is never met</param>
        public static IEnumerable<T> ReplaceSection<T>(this IEnumerable<T> source, IEnumerable<T> replacement, Predicate<T> startReplace, Predicate<T> endReplace, bool addIfMissing = false)
        {
            if (startReplace is null)
                throw new ArgumentNullException(nameof(startReplace));
            if (endReplace is null)
                throw new ArgumentNullException(nameof(endReplace));

            IEnumerator<T> itemsEnumerator = source.GetEnumerator();

            // Initialize the enumerator
            if (!itemsEnumerator.MoveNext())
            {
                // Return replacement if source is empty
                if (addIfMissing)
                    foreach (T item in replacement)
                        yield return item;
                yield break;
            }

            // Return original until startReplace
            while (!startReplace(itemsEnumerator.Current))
            {
                yield return itemsEnumerator.Current;

                if (!itemsEnumerator.MoveNext())
                {
                    // Return the replacement
                    if (addIfMissing)
                        foreach (T item in replacement)
                            yield return item;
                    yield break;
                }
            }

            // Return replacement
            foreach (T item in replacement)
                yield return item;

            // Move enumerator to the first item after triggering startReplace
            if (!itemsEnumerator.MoveNext())
                yield break;

            // Find the end of the section to replace
            do
                if (!itemsEnumerator.MoveNext())
                    yield break;
            while (endReplace(itemsEnumerator.Current));

            // Move to the first item after the replacement
            itemsEnumerator.MoveNext();

            // Return the rest
            while (itemsEnumerator.MoveNext())
                yield return itemsEnumerator.Current;
        }

        /// <summary>
        /// Replaces multiple sections of items.
        /// </summary>
        /// <remarks>Items that match startReplace or endReplace are not included in the returned items.</remarks>
        /// <param name="source">Items to replace sections in</param>
        /// <param name="addIfMissing">Add the replacement to the end of the items if startReplace is never met</param>
        /// <param name="replacements">Array of tuples containing the items to replace the section and functions that determine the start and end of the replacement. Each tuple represents a section to replace</param>
        public static IEnumerable<T> ReplaceSections<T>(this IEnumerable<T> source, bool addIfMissing, IEnumerable<SectionReplacement<T>> replacements)
        {
            if (replacements is null || !replacements.Any())
            {
                foreach (T item in source)
                    yield return item;
                yield break;
            }

            List<SectionReplacement<T>> replacementList = replacements.ToList();
            using IEnumerator<T> itemsEnumerator = source.GetEnumerator();

            do
            {
                // Initialize the enumerator or move to the next item
                if (!itemsEnumerator.MoveNext())
                {
                    if (addIfMissing)
                        foreach (var item in ReturnRemaining())
                            yield return item;
                    yield break;
                }

                for (int i = 0; i < replacementList.Count; i++)
                    if (replacementList[i].StartReplace(itemsEnumerator.Current))
                    {
                        var replacement = replacementList[i];

                        // Return the replacement
                        foreach (T item in replacement.ReplacementGetter())
                            yield return item;

                        // Move to the end of the section to replace
                        while (!replacement.EndReplace(itemsEnumerator.Current))
                            if (!itemsEnumerator.MoveNext())
                            {
                                if (addIfMissing)
                                    foreach (var item in ReturnRemaining())
                                        yield return item;
                                yield break;
                            }

                        replacementList.RemoveAt(i);

                        break;
                    }
                    else
                        yield return itemsEnumerator.Current;

            }
            // Continue until all replacements are applied
            while (replacementList.Count > 0);

            // Return the rest of the items
            while (itemsEnumerator.MoveNext())
                yield return itemsEnumerator.Current;

            IEnumerable<T> ReturnRemaining()
            {
                // Return remaining replacements
                foreach (var replacement in replacementList)
                    // Return the replacement
                    foreach (T item in replacement.ReplacementGetter())
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

        /// <summary>
        /// Loops through a set of objects and returns a set of tuples containing the current object and the previous one.
        /// </summary>
        /// <param name="source">Items to loop through</param>
        /// <param name="firstPrevious">Value of the previous item in the first call of the action</param>
        public static IEnumerable<(T? previous, T? current)> RelativeLoop<T>(this IEnumerable<T?> source, T? firstPrevious = default)
        {
            T? previousItem = firstPrevious;

            foreach (T? item in source)
            {
                yield return (previousItem, item);
                previousItem = item;
            }
        }
        /// <summary>
        /// Removes all items in a <see cref="ICollection{T}"/> that meet a condition
        /// </summary>
        /// <param name="source">Collection to remove items from</param>
        /// <param name="predicate">Function that determines which items to remove</param>
        public static void RemoveWhere<T>(this ICollection<T> source, Predicate<T> predicate)
        {
            foreach (T item in source.Where(i => predicate(i)))
                source.Remove(item);
        }

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

        public static bool TryGetFirst<T>(this IEnumerable<T> source, out T? result)
        {
            using var enumerator = source.GetEnumerator();
            var success = enumerator.MoveNext();

            result = success ? enumerator.Current : default;
            return success;
        }
        public static bool TryGetFirstOfType<TResult>(this IEnumerable source, out TResult? result) => source.OfType<TResult>().TryGetFirst(out result);

        // Methods present in .NET 6 but needed for .NET builds
#if NET5_0
        /// <inheritdoc cref="Enumerable.FirstOrDefault{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
        /// <param name="defaultValue">Value to return if no item meets the condition</param>
        public static T? FirstOrDefault<T>(this IEnumerable<T> source, Predicate<T> predicate, T? defaultValue)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            foreach (T item in source)
                if (predicate(item))
                    return item;
            return defaultValue;
        }

        /// <summary>
        /// Finds the item for which a function returns the smallest or greatest value based on a comparison.
        /// </summary>
        /// <param name="source">Items to find the minimum or maximum of</param>
        /// <param name="selector">Function that gets the key to use in the comparison from an item</param>
        /// <param name="comparison">Function that returns <see langword="true"/> if the second item defeats the first</param>
        private static T MinMaxBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector, Func<TKey, TKey, bool> comparison) where TKey : IComparable<TKey>
        {
            T minMaxItem;
            TKey minMaxKey;

            using (IEnumerator<T> enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    throw new ArgumentException("The enumerable has no items.", nameof(source));

                minMaxItem = enumerator.Current;
                minMaxKey = selector(minMaxItem);

                while (enumerator.MoveNext())
                {
                    TKey key = selector(enumerator.Current);

                    if (comparison(key, minMaxKey))
                    {
                        minMaxItem = enumerator.Current;
                        minMaxKey = key;
                    }
                }
            }

            return minMaxItem;
        }

        /// <summary>
        /// Finds the item for which a function returns the smallest value.
        /// </summary>
        /// <remarks>If the smallest value is obtained from multiple items, the first item to do so will be returned.</remarks>
        /// <param name="source">Items to find the minimum or maximum of</param>
        /// <param name="selector">Function that gets the key to use in the comparison from an item</param>
        public static T MinBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector) where TKey : IComparable<TKey> => MinMaxBy(source, selector, (key, mmKey) => key.CompareTo(mmKey) < 0);
        /// <summary>
        /// Finds the item for which a function returns the greatest value.
        /// </summary>
        /// <remarks>If the greatest value is obtained from multiple items, the first item to do so will be returned.</remarks>
        /// <param name="source">Items to find the minimum or maximum of</param>
        /// <param name="selector">Function that gets the key to use in the comparison from an item</param>
        public static T MaxBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector) where TKey : IComparable<TKey> => MinMaxBy(source, selector, (key, mmKey) => key.CompareTo(mmKey) > 0);
#endif

        public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IEnumerable<T> source)
        {
            foreach (var item in source)
                yield return await Task.FromResult(item);
        }
    }
}

namespace ChartTools
{
    /// <summary>
    /// Provides templates for commonly thrown exceptions
    /// </summary>
    public static class CommonExceptions
    {
        public static ArgumentException GetUndefinedException<TEnum>(TEnum value) where TEnum : Enum => new($"{typeof(TEnum).Name} \"{value}\" is not defined.");

        /// <summary>
        /// The exception that is thrown when a method is called with <see langword="null"/> as a parameter for which <see langword="null"/> is not an accepted value
        /// </summary>
        [Obsolete("Replaced with ArgumentNullException")]
        public class ParameterNullException : Exception
        {
            /// <summary>
            /// Default value of <see cref="MessageTemplate"/>
            /// </summary>
            public const string DefaultTemplate = "Parameter {position} \"{name}\" cannot be null.";
            /// <summary>
            /// Format of the message where "{position}" and "{name}" will be replaced by the respective values.
            /// </summary>
            public static string MessageTemplate { get; set; } = DefaultTemplate;
            private static string FormatReadyTemplate => MessageTemplate.Replace("{name}", "{0}").Replace("{position}", "{1}");

            /// <summary>
            /// Zero-based position of the parameter in the method signature
            /// </summary>
            public byte ParameterPosition { get; set; } = 0;
            /// <summary>
            /// Name of the parameter in the method signature
            /// </summary>
            public string ParameterName { get; set; }

            /// <summary>
            /// Creates an instance of <see cref="ParameterNullException"/> using the previously defined template.
            /// </summary>
            public ParameterNullException(string paramName, byte paramPosition) : base(string.Format(FormatReadyTemplate, paramName, paramPosition))
            {
                ParameterName = paramName;
                ParameterPosition = paramPosition;
            }
            /// <summary>
            /// Creates an instance of <see cref="ParameterNullException"/> using a single-use template.
            /// </summary>
            public ParameterNullException(string paramName, byte paramPosition, string template) : base(string.Format(template, paramName, paramPosition))
            {
                ParameterName = paramName;
                ParameterPosition = paramPosition;
            }
        }
    }

    /// <summary>
    /// Provides additional methods for <see cref="GlobalEvent"/>
    /// </summary>
    public static class GlobalEventExtensions
    {
        public static void ToFile(string path, IEnumerable<GlobalEvent> events) => ExtensionHandler.Write(path, events, null, (".chart", (path, token, _) => ChartWriter.ReplaceGlobalEvents(path, token)));
        public static async Task ToFileAsync(string path, IEnumerable<GlobalEvent> events, CancellationToken cancellationToken) => await ExtensionHandler.WriteAsync(path, events, cancellationToken, null, (".chart", (path, events, token, _) => ChartWriter.ReplaceGlobalEventsAsync(path, events, token)));

        /// <summary>
        /// Gets the lyrics from an enumerable of <see cref="GlobalEvent"/>
        /// </summary>
        /// <returns>Enumerable of <see cref="Phrase"/></returns>
        public static IEnumerable<Phrase> GetLyrics(this IEnumerable<GlobalEvent> globalEvents)
        {
            Phrase? phrase = null;
            Syllable? phraselessFirstSyllable = null;

            foreach (GlobalEvent globalEvent in globalEvents.OrderBy(e => e.Position).Distinct())
                switch (globalEvent.EventType)
                {
                    // Change active phrase
                    case EventTypeHelper.Global.PhraseStart:
                        if (phrase is not null)
                            yield return phrase;

                        phrase = new Phrase(globalEvent.Position);

                        // If the stored lyric has the same position as the new phrase, add it to the phrase
                        if (phraselessFirstSyllable is not null && phraselessFirstSyllable.Position == globalEvent.Position)
                        {
                            phrase.Notes.Add(phraselessFirstSyllable);
                            phraselessFirstSyllable = null;
                        }
                        break;
                    // Add syllable to the active phrase using the event argument
                    case EventTypeHelper.Global.Lyric:
                        Syllable newSyllable = new(globalEvent.Position, VocalsPitches.None) { RawText = globalEvent.Argument };

                        // If the first lyric precedes the first phrase, store it
                        if (phrase is null)
                            phraselessFirstSyllable = newSyllable;
                        else
                            phrase.Notes.Add(newSyllable);
                        break;
                    // Set end position of active phrase
                    case EventTypeHelper.Global.PhraseEnd:
                        if (phrase is not null)
                            phrase.EndPositionOverride = globalEvent.Position;
                        break;
                }

            if (phrase is not null)
                yield return phrase;
        }
        /// <summary>
        /// Gets a set of <see cref="GlobalEvent"/> where phrase and lyric events are replaced with the events making up a set of <see cref="Phrase"/>.
        /// </summary>
        /// <returns>Enumerable of <see cref="GlobalEvent"/></returns>
        public static IEnumerable<GlobalEvent> SetLyrics(this IEnumerable<GlobalEvent> events, IEnumerable<Phrase> lyrics)
        {
            foreach (GlobalEvent globalEvent in new OrderedAlternatingEnumerable<uint, GlobalEvent>(i => i.Position, events.Where(e => !e.IsLyricEvent), lyrics.SelectMany(p => p.ToGlobalEvents())))
                yield return globalEvent;
        }
    }
}

namespace ChartTools.Lyrics
{
    /// <summary>
    /// Provides additional methods to <see cref="Phrase"/>
    /// </summary>
    public static class PhraseExtensions
    {
        /// <summary>
        /// Converts a set of <see cref="Phrase"/> to a set of <see cref="GlobalEvent"/> making up the phrases.
        /// </summary>
        /// <param name="source">Phrases to convert into global events</param>
        /// <returns>Global events making up the phrases</returns>
        public static IEnumerable<GlobalEvent> ToGlobalEvents(this IEnumerable<Phrase> source) => source.SelectMany(p => p.ToGlobalEvents());
    }
}
