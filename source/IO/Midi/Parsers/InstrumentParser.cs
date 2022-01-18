using ChartTools.IO.Configuration.Sessions;

using Melanchall.DryWetMidi.Core;

using System;
using System.Collections.Generic;
using System.Text;

namespace ChartTools.IO.Midi.Parsers
{
    internal abstract class InstrumentParser : MidiParser
    {
        protected InstrumentParser(ReadingSession session) : base(session) { }

        public override Instrument? Result { get; }

        public override void ApplyResultToSong(Song song)
        {
            throw new NotImplementedException();
        }

        protected override void FinaliseParse()
        {
            throw new NotImplementedException();
        }

        protected override void HandleItem(TrackChunk line)
        {
            throw new NotImplementedException();
        }

        protected override void PrepareParse()
        {
            throw new NotImplementedException();
        }
    }
}
