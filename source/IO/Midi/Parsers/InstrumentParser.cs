using ChartTools.IO.Configuration.Sessions;

using System;

namespace ChartTools.IO.Midi.Parsers
{
    internal abstract class InstrumentParser<TChord, TInstEnum> : MidiParser where TChord : Chord where TInstEnum : Enum
    {
        public TInstEnum Instrument { get; }
        public override Instrument<TChord>? Result => result;
        protected Instrument<TChord> preResult = new();
        protected Instrument<TChord>? result;

        protected InstrumentParser(TInstEnum instrument, ReadingSession session) : base(session) => Instrument = instrument;
    }
}
