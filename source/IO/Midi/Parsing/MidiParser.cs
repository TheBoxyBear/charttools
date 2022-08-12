using ChartTools.IO.Configuration.Sessions;
using Melanchall.DryWetMidi.Core;

using System;

namespace ChartTools.IO.Midi.Parsing
{
    internal abstract class MidiParser : FileParser<MidiEvent>, ISongAppliable
    {
        public MidiParser(ReadingSession session) : base(session) { }

        public abstract void ApplyToSong(Song song);

        protected override Exception GetHandleException(MidiEvent item, Exception innerException)
        {
            throw new NotImplementedException("Midi does not yet implement exception handling.", innerException);
        }
        protected override Exception GetFinalizeException(Exception innerException)
        {
            throw new NotImplementedException("Midi does not yet implement exception handling.", innerException);
        }
    }
}
