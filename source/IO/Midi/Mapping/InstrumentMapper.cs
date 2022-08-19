using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Mapping;

using Melanchall.DryWetMidi.Core;
using System.Collections.Generic;

namespace ChartTools.IO.Midi.Mapping
{
    internal abstract class InstrumentMapper<TChord> : IReadMapper<GlobalNoteEvent, NoteEventMapping>, IWriteMapper<Instrument<TChord>, TrackObjectMapping> where TChord : Chord
    {
        public abstract IEnumerable<NoteEventMapping> Map(GlobalNoteEvent e, ReadingSession session);
        public abstract IEnumerable<TrackObjectMapping> Map(Instrument<TChord> instrument, WritingSession session);

        protected static NoteState GetState(NoteEvent note) => note is NoteOnEvent ? NoteState.Open : NoteState.Close;

        protected static T? HandleInvalidMidiEvent<T>(GlobalNoteEvent e, ReadingSession session)
        {
            session.HandleInvalidMidiEventType(e.Position, e.Event);
            return default;
        }

    }
}
