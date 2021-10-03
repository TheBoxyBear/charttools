using ChartTools.IO;
using ChartTools.IO.Chart;

using System.Collections;
using System.Collections.Generic;

namespace ChartTools
{
    /// <summary>
    /// Base class for chords
    /// </summary>
    public abstract class Chord : TrackObject, IEnumerable<Note>
    {
        protected abstract bool OpenExclusivity { get; }
        public abstract byte ModifierKey { get; set; }

        /// <param name="position">Position of the Chord on the Track</param>
        protected Chord(uint position) : base(position) { }

        /// <summary>
        /// Gets the data to write in a chart file.
        /// </summary>
        /// <returns>Enumerable of strings containing the data to add to each line</returns>
        internal abstract IEnumerable<string> GetChartData(ChartParser.WritingSession session, ICollection<byte> ignored);
        internal abstract bool ChartModifierSupported();

        public abstract IEnumerable<Note> GetNotes();

        public IEnumerator<Note> GetEnumerator() => GetNotes().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
