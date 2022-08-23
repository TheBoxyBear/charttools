using ChartTools.IO.Configuration.Sessions;
using Melanchall.DryWetMidi.Core;
using System.Collections.Generic;

namespace ChartTools.IO.Midi.Mapping
{
    internal abstract class InstrumentMapper<TChord> where TChord : Chord
    {
        public abstract IEnumerable<NoteEventMapping> Map(uint position, NoteEvent e, ReadingSession session);
        public abstract IEnumerable<NoteMapping> Map(Instrument<TChord> instrument, WritingSession session);

        protected static T? HandleInvalidMidiEvent<T>(uint position, NoteEvent e, ReadingSession session)
        {
            session.HandleInvalidMidiEventType(position, e);
            return default;
        }

    }
}
