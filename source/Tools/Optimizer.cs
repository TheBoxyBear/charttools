using ChartTools.Extensions.Linq;

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
        public static void CutSustains(this IEnumerable<LaneChord> chords)
        {
            foreach ((var previous, var current) in GetTrackObjectPairs(chords))
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
        public static void CutLengths(this IEnumerable<ILongTrackObject> phrases)
        {
            foreach ((var previous, var current) in GetTrackObjectPairs(phrases))
                if (LengthNeedsCut(previous!, current))
                    previous!.Length = current.Position - previous.Position;
        }

        /// <summary>
        /// Removes redundant tempo markers.
        /// </summary>
        /// <param name="markers">Tempo markers to remove the unneeded from</param>
        public static void RemoveUneeded(this ICollection<Tempo> markers)
        {
            foreach ((var previous, var current) in GetTrackObjectPairs(markers))
                if (previous is not null && previous.Anchor is null && current.Anchor is null && previous.Value == current.Value)
                    markers.Remove(current);
        }

        /// <summary>
        /// Removes redundant time signature markers ones.
        /// </summary>
        /// <param name="signatures">Time signatures to remove the unneeded from</param>
        public static void RemoveUnneeded(this ICollection<TimeSignature> signatures)
        {
            foreach ((var previous, var current) in GetTrackObjectPairs(signatures))
                if (previous is not null && previous.Numerator == current.Numerator && previous.Denominator == current.Denominator)
                    signatures.Remove(current);
        }

        private static IEnumerable<(T?, T)> GetTrackObjectPairs<T>(IEnumerable<T> source) where T : ITrackObject => source.OrderBy(p => p.Position).RelativeLoop();
    }
}
