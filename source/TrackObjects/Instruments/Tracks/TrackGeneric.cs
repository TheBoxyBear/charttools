using ChartTools.Collections.Unique;

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
        public UniqueList<TChord> Chords { get; set; } = new UniqueList<TChord>((c, other) => c.Equals(other));
        /// <summary>
        /// Sets of chrds that give star power
        /// </summary>
        public UniqueList<StarPowerPhrase> StarPower { get; set; } = new UniqueList<StarPowerPhrase>((s, other) => s.Equals(other));
    }
}
