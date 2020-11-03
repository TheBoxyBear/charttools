using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.IO;
using ChartTools.IO;
using ChartTools.IO.Chart;
using Melanchall.DryWetMidi.Core;

namespace System
{
    /// <summary>
    /// Provides additionnal methods to Enum
    /// </summary>
    internal static class EnumExtensions
    {
        /// <summary>
        /// Gets all values of an <see langword="enum"/>.
        /// </summary>
        /// <exception cref="ArgumentException"/>
        public static IEnumerable<TEnum> GetValues<TEnum>() where TEnum : struct, IConvertible => !typeof(TEnum).IsEnum
            ? throw new ArgumentException("Type is not an enum.")
            : Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
    }

    /// <summary>
    /// Provides generic vesion of Activator methods
    /// </summary>
    internal class ActivatorExtensions
    {
        /// <inheritdoc cref="Activator.CreateInstance(Type, bool)"/>
        internal static T CreateInstance<T>(bool nonPublic) => (T)Activator.CreateInstance(typeof(T), nonPublic);
        /// <inheritdoc cref="Activator.CreateInstance(Type, object[])"/>
        internal static T CreateInstance<T>(params object[] args)
        {
            object instance;

            try { instance = Activator.CreateInstance(typeof(T), args); }
            catch { throw; }

            return (T)instance;
        }
    }
}
namespace System.Linq
{
    /// <summary>
    /// Provides additionnal methods to Linq
    /// </summary>
    public static class LinqExtensions
    {
        /// <summary>
        /// <inheritdoc cref="Enumerable.FirstOrDefault{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
        /// </summary>
        public static T FirstOrDefault<T>(this IEnumerable<T> items, Predicate<T> predicate, T defaultValue)
        {
            foreach (T item in items)
                if (predicate(item))
                    return item;
            return defaultValue;
        }
        /// <summary>
        /// <inheritdoc cref="Enumerable.FirstOrDefault{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
        /// </summary>
        public static T FirstOrDefault<T>(this IEnumerable<T> items, Predicate<T> predicate, T defaultValue, out bool returnedDefault)
        {
            foreach (T item in items)
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
        public static IEnumerable<T> Replace<T>(this IEnumerable<T> items, Predicate<T> predicate, T replacement)
        {
            foreach (T item in items)
                yield return predicate(item) ? replacement : item;
        }
        /// <summary>
        /// Replaces a section with other items
        /// </summary>
        ///<remarks>Items that match startReplace or endReplace are not included in the returned items.</remarks>
        public static IEnumerable<T> ReplaceSection<T>(this IEnumerable<T> items, IEnumerable<T> replacement, Predicate<T> startReplace, Predicate<T> endReplace, bool addIfMissing = false)
        {
            IEnumerator<T> itemsEnumerator = items.GetEnumerator();

            //Initialize the enumerator
            if (!itemsEnumerator.MoveNext())
            {
                if (addIfMissing)
                    foreach (T item in replacement)
                        yield return item;
                yield break;
            }

            //Return original until startReplace
            while (!startReplace(itemsEnumerator.Current))
            {
                if (!itemsEnumerator.MoveNext())
                {
                    if (addIfMissing)
                        foreach (T item in items)
                            yield return item;
                    yield break;
                }

                yield return itemsEnumerator.Current;
            }

            //Return replacement
            foreach (T item in replacement)
                yield return item;

            //Move enumerator to the first item after triggering startReplace
            if (!itemsEnumerator.MoveNext())
                yield break;

            //Find the end of the section to replace
            do
                if (!itemsEnumerator.MoveNext())
                    yield break;
            while (endReplace(itemsEnumerator.Current));

            //Move to the first item after the replacement
            itemsEnumerator.MoveNext();

            //Return the rest
            while (itemsEnumerator.MoveNext())
                yield return itemsEnumerator.Current;
        }
        /// <summary>
        /// Replaces multiple sections of items.
        /// </summary>
        /// <remarks>Items that match startReplace or endReplace are not included in the returned items.</remarks>
        public static IEnumerable<T> ReplaceSections<T>(this IEnumerable<T> items, bool addIfMissing, params (IEnumerable<T> replacement, Predicate<T> startReplace, Predicate<T> endReplace)[] replacements)
        {
            if (replacements.Length == 0)
                yield break;

            IEnumerator<T> itemsEnumerator = items.GetEnumerator();
            bool[] replacedSections = new bool[replacements.Length];

            do
            {
                //Initialize the enumerator or move to the next item
                if (!itemsEnumerator.MoveNext())
                {
                    if (addIfMissing)
                        //Return remaining replacements
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

                        //Return the replacement
                        foreach (T item in replacements[i].replacement)
                            yield return item;

                        //Move to the end of the section to replace
                        while (!replacements[i].endReplace(itemsEnumerator.Current))
                            if (!itemsEnumerator.MoveNext())
                            {
                                if (addIfMissing)
                                    //Return remaining replacements
                                    for (int j = 0; j < replacements.Length; j++)
                                        if (!replacedSections[j])
                                            foreach (T item in replacements[j].replacement)
                                                yield return item;
                                yield break;
                            }
                    }
            }
            while (replacedSections.Count(r => r) < replacements.Length);

            //Return the rest of the items
            while (itemsEnumerator.MoveNext())
                yield return itemsEnumerator.Current;
        }
        /// <summary>
        /// Removes a section of items.
        /// </summary>
        /// <remarks>Items that match startRemove or endRemove</remarks>
        public static IEnumerable<T> RemoveSection<T>(this IEnumerable<T> items, Predicate<T> startRemove, Predicate<T> endRemove)
        {
            IEnumerator<T> itemsEnumerator = items.GetEnumerator();

            //Initialize the enumerator 
            if (!itemsEnumerator.MoveNext())
                yield break;

            //Move to the start of items to remove
            while (!startRemove(itemsEnumerator.Current))
                if (!itemsEnumerator.MoveNext())
                    yield break;

            //Skip items to remive
            do
                if (!itemsEnumerator.MoveNext())
                    yield break;
            while (!endRemove(itemsEnumerator.Current));

            //Return the rest
            while (itemsEnumerator.MoveNext())
                yield return itemsEnumerator.Current;
        }
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
        public static void ReadDifficulty(this Instrument<DrumsChord> inst, string path)
        {
            try { inst.Difficulty = Instrument.ReadDifficulty(path, Instruments.Drums); }
            catch { throw; }
        }
        /// <inheritdoc cref="ReadDifficulty(Instrument{DrumsChord}, string)"/>
        public static void ReadDifficulty(this Instrument<GHLChord> inst, string path, GHLInstrument instrument)
        {
            if (!Enum.IsDefined(typeof(GHLInstrument), instrument))
                throw new ArgumentException("Instrument is not defined.");

            try { inst.Difficulty = Instrument.ReadDifficulty(path, (Instruments)instrument); }
            catch { throw; }
        }
        /// <inheritdoc cref="ReadDifficulty(Instrument{DrumsChord}, string)"/>
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
        public static void WriteDifficulty(this Instrument<DrumsChord> inst, string path)
        {
            if (inst.Difficulty is not null)      
                try { Instrument.WriteDifficulty(path, Instruments.Drums, inst.Difficulty.Value); }
                catch { throw; }
        }
        /// <inheritdoc cref="WriteDifficulty(Instrument{DrumsChord}, string)"/>
        public static void WriteDifficulty(this Instrument<GHLChord> inst, string path, GHLInstrument instrument)
        {
            if (!Enum.IsDefined(typeof(GHLInstrument), instrument))
                throw new ArgumentException("Instrument is not defined.");

            if (inst.Difficulty is not null)
                try { Instrument.WriteDifficulty(path, (Instruments)instrument, inst.Difficulty.Value); }
                catch { throw; }
        }
        /// <inheritdoc cref="WriteDifficulty(Instrument{DrumsChord}, string)"/>
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
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="PathTooLongException"/>
        /// <exception cref="DirectoryNotFoundException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="UnauthorizedAccessException"/>
        /// <exception cref="NotSupportedException"/>
        /// <exception cref="SecurityException"/>
        public static void ToFile(this Instrument<DrumsChord> inst, string path)
        {
            try { ExtensionHandler.Write(path, inst, (".chart", ChartParser.ReplaceDrums)); }
            catch { throw; }
        }
        /// <inheritdoc cref="ToFile(Instrument{DrumsChord}, string)"/>
        public static void ToFile(this Instrument<GHLChord> inst, string path, GHLInstrument instrument)
        {
            try { ExtensionHandler.Write(path, inst, (".chart", (p, i) => ChartParser.ReplaceInstrument(p, i, instrument))); }
            catch { throw; }
        }
        /// <inheritdoc cref="ToFile(Instrument{DrumsChord}, string)"/>
        public static void ToFile(this Instrument<StandardChord> inst, string path, StandardInstrument instrument)
        {
            try { ExtensionHandler.Write(path, inst, (".chart", (p, i) => ChartParser.ReplaceInstrument(p, i, instrument))); }
            catch { throw; }
        }
    }

    /// <summary>
    /// Provides additionnal methods to <see cref="Track{TChord}"/>
    /// </summary>
    public static class TrackExtensions
    {
        /// <summary>
        /// Writes the <see cref="Track{TChord}"/> to a file.
        /// </summary>
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
        public static void ToFile(this Track<GHLChord> track, string path, GHLInstrument instrument, Difficulty difficulty)
        {
            try { ExtensionHandler.Write(path, track, (".chart", (p, t) => ChartParser.ReplaceTrack(p, t, instrument, difficulty))); }
            catch { throw; }
        }
        /// <inheritdoc cref="ToFile(Track{DrumsChord}, string, Difficulty)"/>
        public static void ToFile(this Track<StandardChord> track, string path, StandardInstrument instrument, Difficulty difficulty)
        {
            try { ExtensionHandler.Write(path, track, (".chart", (p, t) => ChartParser.ReplaceTrack(p, t, instrument, difficulty))); }
            catch { throw; }
        }
    }
}