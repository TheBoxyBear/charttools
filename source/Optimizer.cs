using ChartTools.SystemExtensions.Linq;

using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ChartTools.Optimization
{
    /// <summary>
    /// Provides methods for simplifying songs
    /// </summary>
    public static class Optimizer
    {
        /// <summary>
        /// Cuts short sustains that exceed the position of the next identical note.
        /// </summary>
        /// <param name="chords">Chords to cut the sustains of</param>
        public static void CutSustains<TNote, TNoteEnum>(this IEnumerable<Chord<TNote, TNoteEnum>> chords) where TNote : Note<TNoteEnum> where TNoteEnum : struct, System.Enum => chords.OrderBy(c => c.Position).RelativeLoop((previous, current) =>
        {
            foreach (TNote note in current!.Notes)
            {
                TNote previousNote = previous!.Notes.First(n => n.NoteIndex == note.NoteIndex);

                if (previousNote is not null && previous.Position + previousNote.SustainLength > current.Position)
                    previousNote.SustainLength = current.Position - previous.Position;
            }
        });
        /// <summary>
        /// Cuts short star power phrases that exceed the start of the next phrase
        /// </summary>
        /// <param name="phrases">Star power phrases to cut the lenghts of</param>
        public static void CutLengths(this IEnumerable<StarPowerPhrase> phrases) => phrases.OrderBy(p => p.Position).RelativeLoop((previous, current) =>
        {
            if (previous is not null && previous.Position + previous.Length > current?.Position)
                previous.Length = current.Position - previous.Position;
        });

        /// <summary>
        /// Sorts tempo markers and removes redundant ones.
        /// </summary>
        /// <param name="markers">Tempo markers to remove the unneeded from</param>
        public static void RemoveUneeded(this IList<Tempo> markers) => markers.OrderBy(m => m.Position).RelativeLoop((previous, current) =>
        {
            if (previous is not null && previous.Anchor is null && current?.Anchor is null && previous.Value == current?.Value)
                markers.Remove(current);
        });
        /// <summary>
        /// Sorts time signatures and removes redundant ones.
        /// </summary>
        /// <param name="signatures">Time signatures to remove the unneeded from</param>
        public static void RemoveUnneeded(this IList<TimeSignature> signatures) => signatures.OrderBy(s => s.Position).RelativeLoop((previous, current) =>
        {
            if (previous is not null && previous.Numerator == current?.Numerator && previous.Denominator == current.Denominator)
                signatures.Remove(current);
        });
    }
}
