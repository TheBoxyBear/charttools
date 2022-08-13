using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Sessions;
using System.Collections;
using System.Collections.Generic;

namespace ChartTools
{
    /// <summary>
    /// Base class for chords
    /// </summary>
    public abstract class Chord : TrackObject, IEnumerable<Note>
    {
        /// <summary>
        /// Notes in the chord
        /// </summary>
        public abstract IEnumerable<Note> Notes { get; }
        internal abstract bool ChartSupportedMoridier { get; }

        protected Chord() : base() { }
        /// <inheritdoc cref="TrackObject(uint)"/>
        protected Chord(uint position) : base(position) { }

        /// <summary>
        /// Gets the data to write in a chart file.
        /// </summary>
        /// <returns>Enumerable of strings containing the data to add to each line</returns>
        internal abstract IEnumerable<TrackObjectEntry> GetChartNoteData();
        internal abstract IEnumerable<TrackObjectEntry> GetChartModifierData(Chord? previous, WritingSession session);

        public IEnumerator<Note> GetEnumerator() => Notes.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
