using ChartTools.IO.Configuration.Sessions;

using System;

namespace ChartTools.IO.Midi.Parsing
{
    internal abstract class InstrumentParser<TChord, TInstEnum> : MidiParser where TChord : Chord where TInstEnum : Enum
    {
        public TInstEnum Instrument { get; }

        public override Instrument<TChord> Result => GetResult(result);
        protected readonly Instrument<TChord> result = new();

        protected InstrumentParser(TInstEnum instrument, ReadingSession session) : base(session) => Instrument = instrument;
    }
}
