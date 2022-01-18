using ChartTools.IO.Configuration.Sessions;

using Melanchall.DryWetMidi.Core;

using System;

namespace ChartTools.IO.Midi.Parsers
{
    internal abstract class ChunkInstrumentParser<TChord, TInstEnum> : MidiParser where TChord : Chord where TInstEnum : Enum
    {
        public TInstEnum Instrument { get; }
        public override Instrument<TChord>? Result => Result;
        private Instrument<TChord>? preResult, result;

        public ChunkInstrumentParser(TInstEnum instrument, ReadingSession session) : base(session) => Instrument = instrument;
    }
}
