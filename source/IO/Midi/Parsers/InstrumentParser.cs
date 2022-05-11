using ChartTools.IO.Configuration.Sessions;

using System;

namespace ChartTools.IO.Midi.Parsers
{
    internal abstract class InstrumentParser<TChord> : MidiParser where TChord : Chord
    {
        public InstrumentIdentity Instrument => GetInstrument();
        protected readonly Track<TChord>[] tracks = new Track<TChord>[4];

        public override Instrument<TChord> Result => GetResult(result);
        protected readonly Instrument<TChord> result = new();

        protected InstrumentParser(ReadingSession session) : base(session) { }

        protected abstract InstrumentIdentity GetInstrument();

        protected override void FinaliseParse()
        {
            foreach (var track in tracks)
                result.SetTrack(track);

            base.FinaliseParse();
        }
    }
}
