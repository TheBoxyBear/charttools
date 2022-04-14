using System.Collections.Generic;

namespace ChartTools
{
    /// <summary>
    /// Set of chords for a instrument at a certain difficulty
    /// </summary>
    public record Track<TChord> : Track where TChord : Chord
    {
        public override List<TChord> Chords { get; } = new();
        public new Instrument<TChord>? ParentInstrument { get; init; }

        protected override Instrument? GetInstrument() => ParentInstrument;
    }
}
