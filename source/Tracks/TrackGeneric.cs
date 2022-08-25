using System.Collections.Generic;

namespace ChartTools
{
    /// <summary>
    /// Set of chords for a instrument at a certain difficulty
    /// </summary>
    public record Track<TChord> : Track where TChord : Chord, new()
    {
        public override List<TChord> Chords { get; } = new();
        public new Instrument<TChord>? ParentInstrument { get; init; }

        public override TChord CreateChord(uint position)
        {
            var chord = new TChord() { Position = position };
            Chords.Add(chord);
            return chord;
        }

        protected override Instrument? GetInstrument() => ParentInstrument;
    }
}
