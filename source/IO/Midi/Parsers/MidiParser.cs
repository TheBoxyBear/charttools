using ChartTools.IO.Configuration.Sessions;
using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Midi.Parsers
{
    internal abstract class MidiParser : FileParser<MidiEvent>
    {
        public MidiParser(ReadingSession session) : base(session) { }
    }
}
