using ChartTools.IO.Configuration.Sessions;

using System;
using System.Collections.Generic;
using System.Text;

namespace ChartTools.IO.Midi.Parsers
{
    internal abstract class TrackParser : MidiParser
    {
        protected TrackParser(ReadingSession session) : base(session) { }

        public override Track Result { get; }
    }
}
