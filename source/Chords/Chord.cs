using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Formatting;

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
        /// Creates a note of type used by the chord.
        /// </summary>
        /// <param name="index"><inheritdoc cref="Note.NoteIndex" path="/summary"/></param>
        /// <param name="length"><inheritdoc cref="Note.Length" path="/summary"/></param>
        /// <returns>Created note</returns>
        public abstract Note CreateNote(byte index, uint length = 0);

        /// <summary>
        /// Gets the data to write in a chart file.
        /// </summary>
        /// <returns>Enumerable of strings containing the data to add to each line</returns>
        internal abstract IEnumerable<TrackObjectEntry> GetChartData(Chord? previous, bool modifiers, FormattingRules formatting);

        public IEnumerator<Note> GetEnumerator() => Notes.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
