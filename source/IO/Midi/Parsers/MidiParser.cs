using ChartTools.IO.Configuration.Sessions;
using Melanchall.DryWetMidi.Core;

using System;

namespace ChartTools.IO.Midi.Parsers
{
    internal abstract class MidiParser : FileParser<MidiEvent>, ISongAppliable
    {
        public ReadingSettings ReadingSettings { get; }

        public MidiParser(ReadingSession session) : base(session) => ReadingSettings = session.Configuration.MidiFirstPassReadingSettings ?? MidiFile.DefaultDryWetReadingSettings;

        public abstract void ApplyToSong(Song song);

        protected override Exception GetHandleException(MidiEvent item, Exception innerException)
        {
            throw new NotImplementedException();
        }
    }
}
