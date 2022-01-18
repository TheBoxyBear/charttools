using ChartTools.IO.Configuration.Sessions;

using System;
using System.Collections.Generic;
using System.Text;

namespace ChartTools.IO.Midi.Parsers
{
    internal abstract class ChunkTrackParser : MidiParser
    {
        public ChunkTrackParser(ReadingSession session) : base(session) { }
    }
}
