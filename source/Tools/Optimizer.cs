using ChartTools.Extensions.Linq;
using ChartTools.IO.Formatting;

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
        /// <returns>Passed chords, ordered by position. Same instance if <paramref name="preOrdered"/> is <see langword="true"/> and <paramref name="chords"/> is <see cref="List{T}"/>.</returns>
        public static List<T> CutSustains<T>(this IEnumerable<T> chords, bool preOrdered = false) where T : LaneChord
        {
            var sustains = new Dictionary<byte, (uint, LaneNote)>();
            var output = GetOrderedList(chords, preOrdered);

            foreach (var chord in output)
            {
                if (chord.Notes.Count == 0)
                    continue;

                using var noteEnumerator = chord.Notes.GetEnumerator();
                noteEnumerator.MoveNext();

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

            return output;
        }

        /// <summary>
        /// Cuts lengths of special phrases based on the numeric value of the type.
        /// </summary>
        /// <param name="phrases">Set of phrases</param>
        /// <param name="preOrdered">Skip ordering of phrases by position</param>
        /// <returns>Passed phrases ordered by position and grouped by type</returns>
        /// <exception cref="InvalidOperationException"/>
        public static List<T>[] CutSpecialLengths<T>(IEnumerable<T> phrases, bool preOrdered = false) where T : SpecialPhrase
        {
            if (typeof(T) == typeof(SpecialPhrase))
                throw new InvalidOperationException($"Collection must be of a type deriving from {nameof(SpecialPhrase)}.");

            var output = phrases.GroupBy(p => p.TypeCode).Select(g => g.ToList()).ToArray();

            foreach (var grouping in output)
                grouping.CutLengths(preOrdered);

            return output;
        }

        /// <summary>
        /// Cuts short long track objects that exceed the start of the next one.
        /// </summary>
        /// <param name="objects">Set of long track objects</param>
        /// <param name="preOrdered">Skip ordering of objects by position</param>
        /// <returns>Passed objects, ordered by position. Same instance if <paramref name="preOrdered"/> is <see langword="true"/> and <paramref name="objects"/> is <see cref="List{T}"/>.</returns>
        public static List<T> CutLengths<T>(this IEnumerable<T> objects, bool preOrdered = false) where T : ILongTrackObject
        {
            var output = GetOrderedList(objects, preOrdered);

            foreach ((var current, var next) in output.RelativeLoopSkipFirst())
                if (LengthNeedsCut(current, next))
                    next.Length = current.Position - current.Position;

            return output;
        }

        /// <summary>
        /// Removes redundant tempo markers.
        /// </summary>
        /// <param name="markers">Tempo markers without anchors.</param>
        /// <param name="preOrdered">Skip ordering of markers by position.</param>
        /// <exception cref="InvalidOperationException"/>
        /// <returns>Passed tempos, ordered by position. Same instance if <paramref name="preOrdered"/> is <see langword="true"/> and <paramref name="markers"/> is <see cref="List{T}"/>.</returns>
        /// <remarks>If some markers may be anchored, use the overload with a resolution.</remarks>
        public static List<Tempo> RemoveUneeded(this ICollection<Tempo> markers, bool preOrdered = false)
        {
            if (markers.TryGetFirst(m => !m.PositionSynced, out var marker))
                throw new InvalidOperationException($"Collection contains tempo marker with desynced anchor at {marker.Anchor}. Resolution needed to synchronize anchors.");

            var output = GetOrderedList(markers, preOrdered);

            foreach ((var previous, var current) in output.RelativeLoopSkipFirst())
                if (previous.Value == current.Value)
                    markers.Remove(current);

            return output;
        }
        /// <summary>
        /// Removes redundant tempo markers by syncing the position of anchored markers.
        /// </summary>
        /// <param name="markers">Set of markers</param>
        /// <param name="resolution">Resolution from <see cref="FormattingRules.TrueResolution"/></param>
        /// <param name="desyncedPreOrdered">Skip ordering of desynced markers by position</param>
        public static List<Tempo> RemoveUneeded(this TempoMap markers, uint resolution, bool desyncedPreOrdered = false)
        {
            markers.Synchronize(resolution, desyncedPreOrdered);

            var output = markers.OrderBy(m => m.Position).ToList();

            foreach ((var previous, var current) in output.RelativeLoopSkipFirst())
                if (current.Value == previous!.Value)
                    markers.Remove(current);

            return output;
        }

        /// <summary>
        /// Removes redundant time signature markers.
        /// </summary>
        /// <param name="signatures">Time signatures to remove the unneeded from</param>
        /// <param name="preOrdered">Skip ordering of markers by position</param>
        /// <returns>Passed markers, ordered by position. Same instance if <paramref name="preOrdered"/> is <see langword="true"/> and <paramref name="signatures"/> is <see cref="List{T}"/>.</returns>
        public static List<TimeSignature> RemoveUnneeded(this ICollection<TimeSignature> signatures, bool preOrdered = false)
        {
            var output = GetOrderedList(signatures, preOrdered);

            foreach ((var previous, var current) in output.RelativeLoopSkipFirst())
                if (previous.Numerator == current.Numerator && previous.Denominator == current.Denominator)
                    signatures.Remove(current);

            return output;
        }

        private static List<T> GetOrderedList<T>(IEnumerable<T> items, bool preOredered) where T : ITrackObject => (preOredered ? items : items.OrderBy(i => i.Position)).ToList();
    }
}
