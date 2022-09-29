using ChartTools.Extensions.Linq;

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
        internal static bool LengthNeedsCut(ILongTrackObject? previous, ILongTrackObject current) => previous?.Position + previous?.Length > current.Position;

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
                if (LengthNeedsCut(previous!, current))
                    previous!.Length = current.Position - previous.Position;

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
            foreach ((var previous, var current) in GetTrackObjectPairs(markers, preOrdered))
            {
                if (current.Anchor is not null)
                    throw new InvalidOperationException($"Collection contains tempo marker with anchor at {current.Anchor}. Use the overload with a resolution.");

                if (previous is not null && previous.Value == current.Value)
                    markers.Remove(current);
            }
        }
        public static void RemoveUneeded(this ICollection<Tempo> markers, uint resolution, bool ticksPreOrdered = false)
        {
            var syncked = new List<Tempo>();
            var unsynched = new List<Tempo>();

            foreach (var tempo in markers)
            {
                var list = tempo.PositionSynced ? syncked : unsynched;
                list.Add(tempo);
            }

            if (!ticksPreOrdered)
                syncked.Sort((a, b) => a.Position.CompareTo(b.Position));

            unsynched.Sort((a, b) => a.Anchor!.Value.CompareTo(b.Anchor!.Value));
        }

        /// <summary>
        /// Removes redundant time signature markers ones.
        /// </summary>
        /// <param name="signatures">Time signatures to remove the unneeded from</param>
        public static void RemoveUnneeded(this ICollection<TimeSignature> signatures, bool preOrdered = false)
        {
            foreach ((var previous, var current) in GetTrackObjectPairs(signatures, preOrdered))
                if (previous is not null && previous.Numerator == current.Numerator && previous.Denominator == current.Denominator)
                    signatures.Remove(current);
        }

        private static IEnumerable<(T?, T)> GetTrackObjectPairs<T>(IEnumerable<T> source, bool preOrdered) where T : ITrackObject
        {
            var ordered = preOrdered ? source : source.OrderBy(o => o.Position);
            return ordered.RelativeLoop();
        }
    }
}
