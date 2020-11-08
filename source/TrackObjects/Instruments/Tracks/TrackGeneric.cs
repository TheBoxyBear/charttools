using ChartTools.Collections;

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
        public UniqueListComparable<TChord> Chords { get; set; } = new UniqueListComparable<TChord>();
        /// <summary>
        /// Sets of chrds that give star power
        /// </summary>
        public UniqueListComparable<StarPowerPhrase> StarPowerPhrases { get; set; } = new UniqueListComparable<StarPowerPhrase>();
    }
}
