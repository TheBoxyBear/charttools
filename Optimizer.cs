using System.Linq;
using System.Collections.Generic;
using System;
using System.Data;

namespace ChartTools.Optimization
{
    /// <summary>
    /// Pvodies methods for simplifying songs
    /// </summary>
    public static class Optimizer
    {
        /// <summary>
        /// Cuts short sustains that exceed the position of the next identical note.
        /// </summary>
        public static void CutSustains<TNote>(this IEnumerable<Chord<TNote>> chords) where TNote : Note => TrackObjectLoop(chords, (previous, current) =>
        {
            foreach (TNote note in current.Notes)
            {
                TNote previousNote = previous.Notes.First(n => n.NoteIndex == note.NoteIndex);

                if (previousNote is not null && previous.Position + previousNote.SustainLength > current.Position)
                    previousNote.SustainLength = current.Position - previous.Position;
            }
        });
        /// <summary>
        /// Cuts short star power phrases that exceed the start of the next phrase
        /// </summary>
        public static void CutLengths(this IEnumerable<StarPowerPhrase> phrases) => TrackObjectLoop(phrases, (previous, current) =>
        {
            if (previous is not null && previous.Position + previous.Length > current.Position)
                previous.Length = current.Position - previous.Position;
        });

        /// <summary>
        /// Sorts tempo markers and removes redundant ones.
        /// </summary>
        /// <param name="markers"></param>
        public static void RemoveUneeded(this IList<Tempo> markers) => TrackObjectLoop(markers, (previous, current) =>
        {
            if (previous is not null && previous.Anchor is null && current.Anchor is null && previous.Value == current.Value)
                markers.Remove(current);
        });
        /// <summary>
        /// Sorts time signatures and removes redundant ones.
        /// </summary>
        public static void RemoveUneeded(this IList<TimeSignature> signatures) => TrackObjectLoop(signatures, (previous, current) =>
        {
            if (previous is not null && previous.Numerator == current.Numerator && previous.Denominator == current.Denominator)
                signatures.Remove(current);
        });

        /// <summary>
        /// Loops through a set of <see cref="TrackObject"/>.
        /// </summary>
        private static void TrackObjectLoop<T>(IEnumerable<T> items, Action<T, T> action) where T : TrackObject
        {
            T previousItem = null;

            foreach (T item in items.OrderBy(i => i.Position))
            {
                action(previousItem, item);
                previousItem = item;
            }
        }
    }
}
