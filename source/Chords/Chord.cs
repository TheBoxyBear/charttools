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
        /// Notes in the <see cref="Chord{TNote}"/>
        /// </summary>
        public abstract IEnumerable<Note> Notes { get; }

        /// <param name="position">Position of the Chord on the Track</param>
        protected Chord(uint position) : base(position) { }

        internal abstract bool ChartSupportedMoridier { get; }

        public IEnumerator<Note> GetEnumerator() => Notes.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
