using ChartTools.Extensions.Linq;
using ChartTools.IO.Formatting;
using ChartTools.Tools.RealTime;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ChartTools.Tools
{
    /// <summary>
    /// Provides methods for simplifying songs
    /// </summary>
    public static class Optimizer
    {
        internal static bool LengthNeedsCut(ILongTrackObject current, ILongTrackObject next) => current.Position + current.Length > next.Position;

        /// <summary>
        /// Cuts short sustains that exceed the position of the next note preventing the sustain from continuing.
        /// </summary>
        /// <param name="chords">Chords to cut the sustains of</param>
        /// <param name="preOrdered">Skip ordering of chords by position</param>
        /// <returns>Passed chords, ordered by position</returns>
        public static IEnumerable<LaneChord> CutSustains(this IEnumerable<LaneChord> chords, bool preOrdered = false)
        {
            var sustains = new Dictionary<byte, (uint, LaneNote)>();

            foreach (var chord in chords)
            {
                using var noteEnumerator = chord.Notes.GetEnumerator();

                if (!noteEnumerator.MoveNext())
                {
                    yield return chord;
                    continue;
                }

                var note = noteEnumerator.Current;

                if (chord.OpenExclusivity)
                {
                    if (noteEnumerator.Current.Index == 0) // Open stops all sustains
                        foreach ((var position, var sustained) in sustains.Values)
                        {
                            if (position + sustained.Sustain > chord.Position)
                                sustained.Sustain = chord.Position;

                            sustains.Remove(noteEnumerator.Current.Index);
                        }
                    else
                        RemoveSustain(0); // Non-opens stops open sustain
                }
                else
                    RemoveSustain(note.Index);

                AddSustain();

                while (noteEnumerator.MoveNext())
                {
                    note = noteEnumerator.Current;

                    RemoveSustain(note.Index);
                    AddSustain();
                }

                yield return chord;

                void AddSustain()
                {
                    if (noteEnumerator.Current.Sustain > 0)
                        sustains[noteEnumerator.Current.Index] = (chord.Position, noteEnumerator.Current);
                }
                void RemoveSustain(byte index)
                {
                    if (sustains.TryGetValue(index, out var sustained))
                    {
                        sustained.Item2.Sustain = chord.Position;
                        sustains.Remove(index);
                    }
                }
            }
        }

        /// <summary>
        /// Cuts lengths of special phrases based on the numeric value of the type.
        /// </summary>
        /// <param name="phrases">Set of phrases</param>
        /// <param name="preOrdered">Skip ordering of phrases by position</param>
        /// <returns>Passed phrases ordered by position and grouped by type</returns>
        public static IEnumerable<IGrouping<byte, SpecialPhrase>> CutLengths(IEnumerable<SpecialPhrase> phrases, bool preOrdered = false)
        {
            foreach (var grouping in phrases.GroupBy(p => p.TypeCode))
            {
                grouping.CutLengths(preOrdered);
                yield return grouping;
            }
        }

        /// <summary>
        /// Cuts short long track objects that exceed the start of the next one.
        /// </summary>
        /// <param name="objects">Set of long track objects</param>
        /// <param name="preOrdered">Skip ordering of objects by position</param>
        /// <returns>Passed objects, ordered by position</returns>
        public static IEnumerable<ILongTrackObject> CutLengths(this IEnumerable<ILongTrackObject> objects, bool preOrdered = false)
        {
            foreach ((var current, var next) in GetTrackObjectPairs(objects, preOrdered))
                if (current is not null)
                {
                    if (LengthNeedsCut(current, next))
                        next.Length = current.Position - current.Position;

                    yield return current;
                }
        }

        /// <summary>
        /// Removes redundant tempo markers.
        /// </summary>
        /// <param name="markers">Tempo markers without anchors.</param>
        /// <param name="preOrdered">Skip ordering of markers by position.</param>
        /// <exception cref="InvalidOperationException"/>
        /// <returns></returns>
        /// <remarks>If some markers may be anchored, use the overload with a resolution.</remarks>
        public static IEnumerable<Tempo> RemoveUneeded(this ICollection<Tempo> markers, bool preOrdered = false)
        {
            if (markers.TryGetFirst(m => m.Anchor is not null, out var anchor))
                throw new InvalidOperationException($"Collection contains tempo marker with anchor at {anchor.Anchor}. Resolution needed compare tempo markers.");

            foreach ((var previous, var current) in GetTrackObjectPairs(markers, preOrdered))
                if (previous is not null && previous.Value == current.Value)
                    markers.Remove(current);
                else
                    yield return current;
        }
        /// <summary>
        /// Removes redundant tempo markers by syncing the position of anchored markers.
        /// </summary>
        /// <param name="markers">Set of markers</param>
        /// <param name="resolution">Resolution from <see cref="FormattingRules.TrueResolution"/></param>
        /// <param name="desyncedPreOrdered">Skip ordering of desynced markers by position</param>
        /// <returns>Passed markers, ordered by position</returns>
        public static IEnumerable<Tempo> RemoveUneeded(this ICollection<Tempo> markers, uint resolution, bool desyncedPreOrdered = false)
        {
            foreach ((var previous, var current) in markers.SyncAnchors(resolution, desyncedPreOrdered).RelativeLoop())
                if (previous is not null)
                    if (current.Value == previous!.Value)
                        markers.Remove(current);
                    else
                        yield return current;
        }

        /// <summary>
        /// Removes redundant time signature markers ones.
        /// </summary>
        /// <param name="signatures">Time signatures to remove the unneeded from</param>
        /// <param name="preOrdered">Skip ordering of markers by position</param>
        /// <returns>Passes markers, ordered by position</returns>
        public static IEnumerable<TimeSignature> RemoveUnneeded(this ICollection<TimeSignature> signatures, bool preOrdered = false)
        {
            foreach ((var previous, var current) in GetTrackObjectPairs(signatures, preOrdered))
                if (previous is not null && previous.Numerator == current.Numerator && previous.Denominator == current.Denominator)
                    signatures.Remove(current);
                else
                    yield return current;
        }

        private static IEnumerable<(T?, T)> GetTrackObjectPairs<T>(IEnumerable<T> source, bool preOrdered) where T : ITrackObject
        {
            var ordered = preOrdered ? source : source.OrderBy(o => o.Position);
            return ordered.RelativeLoop();
        }
    }
}
