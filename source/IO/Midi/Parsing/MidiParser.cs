using ChartTools.IO.Configuration.Sessions;
using Melanchall.DryWetMidi.Core;

using System;

namespace ChartTools.IO.Midi.Parsing
{
    internal abstract class MidiParser : FileParser<MidiEvent>, ISongAppliable
    {
        protected uint globalPosition;

        public MidiParser(ReadingSession session) : base(session) { }

        public abstract void ApplyToSong(Song song);

        protected override Exception GetHandleException(MidiEvent item, Exception innerException)
        {
            return new NotImplementedException(innerException.Message, innerException);
        }
        protected override Exception GetFinalizeException(Exception innerException)
        {
            return new NotImplementedException(innerException.Message, innerException);
        }
    }
}
