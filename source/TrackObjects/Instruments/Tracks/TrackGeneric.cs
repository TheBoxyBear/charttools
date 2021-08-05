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
        /// Chords to play
        /// </summary>
        public UniqueList<TChord> Chords { get; } = new UniqueList<TChord>(chordComparison);

        public override IEnumerable<Chord> GetChords() => Chords;
        internal static readonly EqualityComparison<TChord> chordComparison = (c, other) => c.Equals(other);
    }
}
