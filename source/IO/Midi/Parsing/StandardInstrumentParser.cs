using ChartTools.IO.Configuration.Sessions;
using Melanchall.DryWetMidi.Core;
using System;

namespace ChartTools.IO.Midi.Parsing
{
    internal abstract class StandardInstrumentParser : InstrumentParser<StandardChord, StandardInstrumentIdentity>
    {
        public StandardInstrumentParser(StandardInstrumentIdentity instrument, ReadingSession session) : base(instrument, session) { }

        public override void ApplyToSong(Song song)
        {
            throw new NotImplementedException();
        }

        protected override void HandleItem(MidiEvent item)
        {
            throw new NotImplementedException();
        }

        protected abstract (Track<StandardChord> track, int adjustedNoteNumber) MapNoteEvent(NoteEvent e);
    }
}
