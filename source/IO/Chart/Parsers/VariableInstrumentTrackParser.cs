using System;

namespace ChartTools.IO.Chart.Parsers
{
    internal abstract class VariableInstrumentTrackParser<TChord, TInstEnum> : TrackParser<TChord> where TChord : Chord where TInstEnum : Enum
    {
        public TInstEnum Instrument { get; }
        public VariableInstrumentTrackParser(Difficulty difficulty, TInstEnum instrument) : base(difficulty) => Instrument = instrument;
    }
}
