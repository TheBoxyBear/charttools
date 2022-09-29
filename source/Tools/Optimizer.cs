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
        public static void CutSustains(this IEnumerable<LaneChord> chords, bool preOrdered = false)
        {
            foreach ((var previous, var current) in GetTrackObjectPairs(chords, preOrdered))
                foreach (var note in current.Notes)
                {
                    var previousNote = previous!.Notes.First(n => n.Index == note.Index);

                    if (previousNote is not null && previous.Position + previousNote.Length > current.Position)
                        previousNote.Length = current.Position - previous.Position;
                }
        }

        public static void CutLengths(IEnumerable<SpecialPhrase> phrases)
        {
            foreach (var grouping in phrases.GroupBy(p => p.TypeCode))
                grouping.CutLengths();
        }

        /// <summary>
        /// Cuts short long track objects that exceed the start of the next one.
        /// </summary>
        /// <param name="phrases">Star power phrases to cut the lengths of</param>
        public static void CutLengths(this IEnumerable<ILongTrackObject> phrases, bool preOrdered = false)
        {
            foreach ((var previous, var current) in GetTrackObjectPairs(phrases, preOrdered))
                if (LengthNeedsCut(previous!, current))
                    previous!.Length = current.Position - previous.Position;
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
