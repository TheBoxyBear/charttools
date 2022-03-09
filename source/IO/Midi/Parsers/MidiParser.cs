using ChartTools.IO.Configuration.Sessions;
using Melanchall.DryWetMidi.Core;

using System;

namespace ChartTools.IO.Midi.Parsers
{
    internal abstract class MidiParser : FileParser<MidiEvent>
    {
        public MidiParser(ReadingSession session) : base(session) { }

        protected override Exception GetHandleException(MidiEvent item, Exception innerException)
        {
            throw new NotImplementedException();
        }
    }
}
