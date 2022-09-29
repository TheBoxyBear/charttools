using ChartTools.Extensions.Linq;
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
        internal static bool LengthNeedsCut(ILongTrackObject previous, ILongTrackObject current) => previous.Position + previous.Length > current.Position;

        /// <summary>
        /// Cuts short sustains that exceed the position of the next identical note.
        /// </summary>
        /// <param name="chords">Chords to cut the sustains of</param>
        /// <returns>Passed chords, ordered by position</returns>
        public static IEnumerable<LaneChord> CutSustains(this IEnumerable<LaneChord> chords, bool preOrdered = false)
        {
            foreach ((var previous, var current) in GetTrackObjectPairs(chords, preOrdered))
            {
                foreach (var note in current.Notes)
                {
                    var previousNote = previous!.Notes.First(n => n.Index == note.Index);

                    if (previousNote is not null && previous.Position + previousNote.Length > current.Position)
                        previousNote.Length = current.Position - previous.Position;
                }

                yield return current;
            }
        }

        /// <summary>
        /// Cuts lengths of special phrases based on the numeric value of the type.
        /// </summary>
        /// <param name="phrases">Set of phrases</param>
        /// <param name="preOrdered">If <see langword="true"/>, skips sorting phrases by position.</param>
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
        /// <returns>Passed objects, ordered by position</returns>
        public static IEnumerable<ILongTrackObject> CutLengths(this IEnumerable<ILongTrackObject> objects, bool preOrdered = false)
        {
            foreach ((var previous, var current) in GetTrackObjectPairs(objects, preOrdered))
            {
                if (LengthNeedsCut(previous, current))
                    previous.Length = current.Position - previous.Position;

                yield return current;
            }
        }

        /// <summary>
        /// Removes redundant tempo markers.
        /// </summary>
        /// <param name="markers">Tempo markers to remove the unneeded from</param>
        /// <exception cref="InvalidOperationException"/>
        /// <remarks>Only use if certain that no tempo marker contain an anchor, otherwise use <see cref="RemoveUneeded(ICollection{Tempo}, uint, bool)"/></remarks>
        public static void RemoveUneeded(this ICollection<Tempo> markers, bool preOrdered = false)
        {
            if (markers.TryGetFirst(m => m.Anchor is not null, out var anchor))
                throw new InvalidOperationException($"Collection contains tempo marker with anchor at {anchor.Anchor}. Use the overload with a resolution.");

            foreach ((var previous, var current) in GetTrackObjectPairs(markers, preOrdered))
                if (previous.Value == current.Value)
                    markers.Remove(current);
        }
        public static void RemoveUneeded(this ICollection<Tempo> markers, uint resolution, bool ticksPreOrdered = false)
        {
            foreach ((var previous, var current) in markers.SyncAnchors(resolution, ticksPreOrdered).RelativeLoopSkipFirst())
                if (current.Value == previous!.Value)
                    markers.Remove(current);
        }

        /// <summary>
        /// Removes redundant time signature markers ones.
        /// </summary>
        /// <param name="signatures">Time signatures to remove the unneeded from</param>
        public static void RemoveUnneeded(this ICollection<TimeSignature> signatures, bool preOrdered = false)
        {
            foreach ((var previous, var current) in GetTrackObjectPairs(signatures, preOrdered))
                if (previous.Numerator == current.Numerator && previous.Denominator == current.Denominator)
                    signatures.Remove(current);
        }

        private static IEnumerable<(T, T)> GetTrackObjectPairs<T>(IEnumerable<T> source, bool preOrdered) where T : ITrackObject
        {
            var ordered = preOrdered ? source : source.OrderBy(o => o.Position);
            return ordered.RelativeLoopSkipFirst();
        }
    }
}
