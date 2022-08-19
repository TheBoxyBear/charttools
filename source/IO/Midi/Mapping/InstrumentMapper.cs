using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Mapping;

using System.Collections.Generic;

namespace ChartTools.IO.Midi.Mapping
{
    internal abstract class InstrumentMapper<TChord> :
        IReadMapper<GlobalNoteEvent, IEnumerable<NoteEventMapping>>,
        IWriteMapper<Instrument<TChord>, IEnumerable<NoteMapping>>
        where TChord : Chord
    {
        public abstract IEnumerable<NoteEventMapping> Map(GlobalNoteEvent e, ReadingSession session);
        public abstract IEnumerable<NoteMapping> Map(Instrument<TChord> instrument, WritingSession session);

        protected static T? HandleInvalidMidiEvent<T>(GlobalNoteEvent e, ReadingSession session)
        {
            session.HandleInvalidMidiEventType(e.Position, e.Event);
            return default;
        }

    }
}
