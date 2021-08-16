using System.Collections.Generic;

using ChartTools.Collections.Unique;
using ChartTools.SystemExtensions;

namespace ChartTools
{
    /// <summary>
    /// Set of chords for a instrument at a certain difficulty
    /// </summary>
    public class Track<TChord> : Track where TChord : Chord
    {
        /// <summary>
        /// Groups of notes of the same position
        /// </summary>
        public NonStackableTrackObjectCollection<TChord> Chords { get; } = new();

        public override IEnumerable<Chord> GetChords() => Chords;
        internal static readonly EqualityComparison<TChord> chordComparison = (c, other) => c.Equals(other);
    }
}
