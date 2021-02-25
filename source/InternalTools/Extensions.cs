using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.IO;
using ChartTools.IO;
using ChartTools.IO.Chart;
using ChartTools.Lyrics;
using ChartTools.Collections.Alternating;

namespace ChartTools.SystemExtensions
{
    /// <summary>
    /// <see cref="IEquatable{T}"/> equivalent to the <see cref="IComparable{T}"/> <see cref="Comparison{T}"/> delegate
    /// </summary>
    public delegate bool EqualityComparison<T>(T a, T b);

    /// <summary>
    /// Provides additionnal methods to Enum
    /// </summary>
    internal static class EnumExtensions
    {
        /// <summary>
        /// Gets all values of an <see langword="enum"/>.
        /// </summary>
        /// <exception cref="ArgumentException"/>
        public static IEnumerable<TEnum> GetValues<TEnum>() where TEnum : Enum => Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
    }

    internal static class ObjectExtensions
    {
        public static bool IsCast<TSource, TTarget>(this TSource source, out TTarget target) where TTarget : TSource
        {
            if (source is TTarget)
            {
                target = (TTarget)source;
                return true;
            }

            target = default;
            return false;
        }
    }

    /// <summary>
    /// Provides additionnal methods to string
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
        public static string VerbalEnumerate(string lastItemPreceder, params string[] items) => items is null ? throw new ArgumentNullException() : items.Length switch
        {
            0 => string.Empty,
            1 => items[0],
            2 => $"{items[0]} {lastItemPreceder} {items[1]}",
            _ => $"{string.Join(", ", items, items.Length - 1)} {lastItemPreceder} {items[^0]}"
        };
    }
}
namespace ChartTools.SystemExtensions.Linq
{
    /// <summary>
    /// Provides additionnal methods to Linq
    /// </summary>
    public static class LinqExtensions
    {
        /// <inheritdoc cref="Enumerable.FirstOrDefault{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
        /// <param name="defaultValue">Value to return if no item meets the condition</param>
        public static T FirstOrDefault<T>(this IEnumerable<T> source, Predicate<T> predicate, T defaultValue)
        {
            foreach (T item in source)
                if (predicate(item))
                    return item;
            return defaultValue;
        }
        /// <inheritdoc cref="FirstOrDefault{T}(IEnumerable{T}, Predicate{T}, T)"/>
        /// <param name="returnedDefault"><see langword="true"/> if no items meeting the condition were found</param>
        public static T FirstOrDefault<T>(this IEnumerable<T> source, Predicate<T> predicate, T defaultValue, out bool returnedDefault)
        {
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
        /// Replaces items that meet a condition with another item.
        /// </summary>
        /// <param name="source">The IEnumerable&lt;out T&gt; to replace the items of</param>
        /// <param name="predicate">A function that determines if an item must be replaced</param>
        /// <param name="replacement">The item to replace items with</param>
        public static IEnumerable<T> Replace<T>(this IEnumerable<T> source, Predicate<T> predicate, T replacement)
        {
            foreach (T item in source)
                yield return predicate(item) ? replacement : item;
        }
        /// <summary>
        /// Replaces a section with other items
        /// </summary>
        /// <remarks>Items that match startReplace or endReplace are not included in the returned items.</remarks>
        /// <param name="source">Items to replace a section in</param>
        /// <param name="replacement">Items to replace the section with</param>
        /// <param name="startReplace">Function that determines if an item if the first to be replaced</param>
        /// <param name="endReplace">Function that determines where to stop excluding items from the source</param>
        /// <param name="addIfMissing">Add the replacement to the end of the items if startReplace is never met</param>
        public static IEnumerable<T> ReplaceSection<T>(this IEnumerable<T> source, IEnumerable<T> replacement, Predicate<T> startReplace, Predicate<T> endReplace, bool addIfMissing = false)
        {
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
                    if (addIfMissing)
                        foreach (T item in source)
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
        public static IEnumerable<T> ReplaceSections<T>(this IEnumerable<T> source, bool addIfMissing, params (IEnumerable<T> replacement, Predicate<T> startReplace, Predicate<T> endReplace)[] replacements)
        {
            if (replacements.Length == 0)
                yield break;

            IEnumerator<T> itemsEnumerator = source.GetEnumerator();
            bool[] replacedSections = new bool[replacements.Length];

            do
            {
                // Initialize the enumerator or move to the next item
                if (!itemsEnumerator.MoveNext())
                {
                    if (addIfMissing)
                        // Return remaining replacements
                        for (int j = 0; j < replacements.Length; j++)
                            if (!replacedSections[j])
                                foreach (T item in replacements[j].replacement)
                                    yield return item;
                    yield break;
                }

                for (int i = 0; i < replacements.Length; i++)
                    if (!replacedSections[i] && replacements[i].startReplace(itemsEnumerator.Current))
                    {
                        replacedSections[i] = true;

                        // Return the replacement
                        foreach (T item in replacements[i].replacement)
                            yield return item;

                        // Move to the end of the section to replace
                        while (!replacements[i].endReplace(itemsEnumerator.Current))
                            if (!itemsEnumerator.MoveNext())
                            {
                                if (addIfMissing)
                                    // Return remaining replacements
                                    for (int j = 0; j < replacements.Length; j++)
                                        if (!replacedSections[j])
                                            foreach (T item in replacements[j].replacement)
                                                yield return item;
                                yield break;
                            }
                    }
            }
            while (replacedSections.Count(r => r) < replacements.Length);

            // Return the rest of the items
            while (itemsEnumerator.MoveNext())
                yield return itemsEnumerator.Current;
        }
        /// <summary>
        /// Removes a section of items.
        /// </summary>
        /// <remarks>Items that match startRemove or endRemove</remarks>
        /// <param name="source">Source items to remove a section of</param>
        /// <param name="startRemove">Function that determines the start of the section to replace</param>
        /// <param name="endRemove">Function that determines the end of the section to repalce</param>
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

            // Skip items to remive
            do
                if (!itemsEnumerator.MoveNext())
                    yield break;
            while (!endRemove(itemsEnumerator.Current));

            // Return the rest
            while (itemsEnumerator.MoveNext())
                yield return itemsEnumerator.Current;
        }
        /// <summary>
        /// Loops through a set of objects and runs a function referencing the previous item
        /// </summary>
        /// <param name="source">Items to loop through</param>
        /// <param name="action">Function to run for each item</param>
        /// <param name="firstPrevious">Value of the previous item in the first call of the action</param>
        public static void RelativeLoop<T>(this IEnumerable<T> source, Action<T, T> action, T firstPrevious = default)
        {
            T previousItem = firstPrevious;

            foreach (T item in source)
            {
                action(previousItem, item);
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
                    throw new Exception("The enumerable has no items.");

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
                    throw new Exception("The enumerable has no items.");

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
    }
}

namespace ChartTools
{
    /// <summary>
    /// Provides additionnal methods to <see cref="Instrument{TChord}"/>
    /// </summary>
    public static class InstrumentExtensions
    {
        /// <summary>
        /// Reads <see cref="Instrument.Difficulty"/> from a file.
        /// </summary>
        /// <param name="inst">Instrument to the <see cref="Instrument.Difficulty"/> property of</param>
        /// <param name="path">Path of the file to read the difficulty from</param>
        public static void ReadDifficulty(this Instrument<DrumsChord> inst, string path)
        {
            try { inst.Difficulty = Instrument.ReadDifficulty(path, Instruments.Drums); }
            catch { throw; }
        }
        /// <inheritdoc cref="ReadDifficulty(Instrument{DrumsChord}, string)"/>
        /// <param name="instrument">Instrument to read the difficulty of</param>
        public static void ReadDifficulty(this Instrument<GHLChord> inst, string path, GHLInstrument instrument)
        {
            if (!Enum.IsDefined(typeof(GHLInstrument), instrument))
                throw new ArgumentException("Instrument is not defined.");

            try { inst.Difficulty = Instrument.ReadDifficulty(path, (Instruments)instrument); }
            catch { throw; }
        }
        /// <inheritdoc cref="ReadDifficulty(Instrument{GHLChord}, string)"/>
        public static void ReadDifficulty(this Instrument<StandardChord> inst, string path, StandardInstrument instrument)
        {
            if (!Enum.IsDefined(typeof(StandardInstrument), instrument))
                throw new ArgumentException("Instrument is not defined.");

            try { inst.Difficulty = Instrument.ReadDifficulty(path, (Instruments)100); }
            catch { throw; }
        }

        /// <summary>
        /// Writes <see cref="Instrument.Difficulty"/> to a file.
        /// </summary>
        /// <param name="inst"><see cref="Instrument"/> containing the difficulty to write</param>
        /// <param name="path">Path of the file to write the difficulty to</param>
        public static void WriteDifficulty(this Instrument<DrumsChord> inst, string path)
        {
            if (inst.Difficulty is not null)
                try { Instrument.WriteDifficulty(path, Instruments.Drums, inst.Difficulty.Value); }
                catch { throw; }
        }
        /// <inheritdoc cref="WriteDifficulty(Instrument{DrumsChord}, string)"/>
        /// <param name="instrument">Instrument to assign the difficulty to</param>
        public static void WriteDifficulty(this Instrument<GHLChord> inst, string path, GHLInstrument instrument)
        {
            if (!Enum.IsDefined(typeof(GHLInstrument), instrument))
                throw new ArgumentException("Instrument is not defined.");

            if (inst.Difficulty is not null)
                try { Instrument.WriteDifficulty(path, (Instruments)instrument, inst.Difficulty.Value); }
                catch { throw; }
        }
        /// <inheritdoc cref="WriteDifficulty(Instrument{GHLChord}, string)"/>
        public static void WriteDifficulty(this Instrument<StandardChord> inst, string path, StandardInstrument instrument)
        {
            if (!Enum.IsDefined(typeof(StandardInstrument), instrument))
                throw new ArgumentException("Instrument is not defined.");

            if (inst.Difficulty is not null)
                try { Instrument.WriteDifficulty(path, (Instruments)instrument, inst.Difficulty.Value); }
                catch { throw; }
        }

        /// <summary>
        /// Writes the <see cref="Instrument{TChord}"/> to a file.
        /// </summary>
        /// <param name="path">Path of the file to write to</param>
        /// <param name="midiEventSource">Source of global events to use for every difficulty if written to a MIDI file
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="PathTooLongException"/>
        /// <exception cref="DirectoryNotFoundException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="UnauthorizedAccessException"/>
        /// <exception cref="NotSupportedException"/>
        /// <exception cref="SecurityException"/>
        public static void ToFile(this Instrument<DrumsChord> inst, string path, LocalEventSource eventSource = LocalEventSource.Auto)
        {
            try { ExtensionHandler.Write(path, inst, (".chart", ChartParser.ReplaceDrums)); }
            catch { throw; }
        }
        /// <inheritdoc cref="ToFile(Instrument{DrumsChord}, string)"/>
        /// <param name="instrument">Instrument to assign the data to</param>
        public static void ToFile(this Instrument<GHLChord> inst, string path, GHLInstrument instrument)
        {
            try { ExtensionHandler.Write(path, inst, (".chart", (p, i) => ChartParser.ReplaceInstrument(p, i, instrument))); }
            catch { throw; }
        }
        /// <inheritdoc cref="ToFile(Instrument{GHLChord}, string)"/>
        public static void ToFile(this Instrument<StandardChord> inst, string path, StandardInstrument instrument)
        {
            try { ExtensionHandler.Write(path, inst, (".chart", (p, i) => ChartParser.ReplaceInstrument(p, i, instrument))); }
            catch { throw; }
        }
    }

    /// <summary>
    /// Provides templates for commonly thrown exceptions
    /// </summary>
    internal static class CommonExceptions
    {
        internal static ArgumentException GetUndefinedException(object value) => new ArgumentException($"{value.GetType().Name} \"{value}\" is not defined.");
    }

    /// <summary>
    /// Provides additionnal methods to <see cref="Track{TChord}"/>
    /// </summary>
    public static class TrackExtensions
    {
        /// <summary>
        /// Writes the <see cref="Track{TChord}"/> to a file.
        /// </summary>
        /// <param name="path">Path of the file to write to</param>
        /// <param name="difficulty">Difficulty to assign the <see cref="Track"/> to</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="PathTooLongException"/>
        /// <exception cref="DirectoryNotFoundException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="UnauthorizedAccessException"/>
        /// <exception cref="NotSupportedException"/>
        /// <exception cref="SecurityException"/>
        public static void ToFile(this Track<DrumsChord> track, string path, Difficulty difficulty)
        {
            try { ExtensionHandler.Write(path, track, (".chart", (p, t) => ChartParser.ReplaceDrumsTrack(p, t, difficulty))); }
            catch { throw; }
        }
        /// <inheritdoc cref="ToFile(Track{DrumsChord}, string, Difficulty)"/>
        /// <param name="instrument">Instrument to assign the <see cref="Track{TChord}"/> to</param>
        public static void ToFile(this Track<GHLChord> track, string path, GHLInstrument instrument, Difficulty difficulty)
        {
            try { ExtensionHandler.Write(path, track, (".chart", (p, t) => ChartParser.ReplaceTrack(p, t, instrument, difficulty))); }
            catch { throw; }
        }
        /// <inheritdoc cref="ToFile(Track{GHLChord}, string, Difficulty)"/>
        public static void ToFile(this Track<StandardChord> track, string path, StandardInstrument instrument, Difficulty difficulty)
        {
            try { ExtensionHandler.Write(path, track, (".chart", (p, t) => ChartParser.ReplaceTrack(p, t, instrument, difficulty))); }
            catch { throw; }
        }
    }

    /// <summary>
    /// Provides additionnal methods to <see cref="Event"/>
    /// </summary>
    public static class EventExtensions
    {
        /// <summary>
        /// Writes the global events to a file
        /// </summary>
        /// <param name="events">Events to write</param>
        /// <param name="path">Path of the file to write</param>
        public static void ToFile(this IEnumerable<GlobalEvent> events, string path)
        {
            try { ExtensionHandler.Write(path, events, (".chart", ChartParser.ReplaceGlobalEvents)); }
            catch { throw; }
        }
    }
    /// <summary>
    /// Provides additionnal methods for <see cref="GlobalEvent"/>
    /// </summary>
    public static class GlobalEventExtensions
    {
        /// <summary>
        /// Gets the lyrics from an enumerable of <see cref="GlobalEvent"/>
        /// </summary>
        /// <returns>Enumerable of <see cref="Phrase"/></returns>
        public static IEnumerable<Phrase> GetLyrics(this IEnumerable<GlobalEvent> globalEvents)
        {
            Phrase phrase = null;
            Syllable phraselessFirstSyllable = null;

            foreach (GlobalEvent globalEvent in globalEvents.OrderBy(e => e.Position))
                switch (globalEvent.EventType)
                {
                    // Change active phrase
                    case GlobalEventType.PhraseStart:
                        if (phrase is not null)
                            yield return phrase;

                        phrase = new Phrase(globalEvent.Position);

                        // If the stored lyric has the same position as the new phrase, add it to the phrase
                        if (phraselessFirstSyllable is not null && phraselessFirstSyllable.Position == globalEvent.Position)
                        {
                            phrase.Syllables.Add(phraselessFirstSyllable);
                            phraselessFirstSyllable = null;
                        }
                        break;
                    // Add syllable to the active phrase usign the event argument
                    case GlobalEventType.Lyric:
                        Syllable newSyllable = new Syllable(globalEvent.Position) { RawText = globalEvent.Argument };

                        // If the first lyric preceeds the first phrase, store it
                        if (phrase is null)
                            phraselessFirstSyllable = newSyllable;
                        else
                            phrase.Syllables.Add(newSyllable);
                        break;
                    // Set end position of active phrase
                    case GlobalEventType.PhraseEnd:
                        if (phrase is not null)
                            phrase.EndPosition = globalEvent.Position;
                        break;
                }

            if (phrase is not null)
                yield return phrase;
        }
        /// <summary>
        /// Gets a set of <see cref="GlobalEvent"/> where phrase and lyric events are replaced with the events makign up a set of <see cref="Phrase"/>.
        /// </summary>
        /// <returns>Enumerable of <see cref="GlobalEvent"/></returns>
        public static IEnumerable<GlobalEvent> SetLyrics(this IEnumerable<GlobalEvent> events, IEnumerable<Phrase> lyrics)
        {
            foreach (GlobalEvent globalEvent in new OrderedAlternatingEnumerable<GlobalEvent, uint>(i => i.Position, events.Where(e => !e.IsLyricEvent), lyrics.SelectMany(p => p.ToGlobalEvents())))
                yield return globalEvent;
        }
    }
}
 
namespace ChartTools.Lyrics
{
    /// <summary>
    /// Provides additionnal methods to <see cref="Phrase"/>
    /// </summary>
    public static class PhraseExtensions
    {
        /// <summary>
        /// Converts a set of <see cref="Phrase"/> to a set of <see cref="GlobalEvent"/> making up the phrases.
        /// </summary>
        /// <param name="source">Phrases to covnert into global evnets</param>
        /// <returns>Global events making up the phrases</returns>
        public static IEnumerable<GlobalEvent> ToGlobalEvents(this IEnumerable<Phrase> source) => source.SelectMany(p => p.ToGlobalEvents());
    }
}