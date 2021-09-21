using System.Collections.Generic;

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
        public UniqueTrackObjectCollection<TChord> Chords { get; } = new();

        public override IEnumerable<Chord> GetChords() => Chords;
    }
}
