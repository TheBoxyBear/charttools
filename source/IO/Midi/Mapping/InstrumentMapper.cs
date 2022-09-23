using ChartTools.IO.Configuration.Sessions;

using Melanchall.DryWetMidi.Core;
using System.Collections.Generic;

namespace ChartTools.IO.Midi.Mapping
{
    internal abstract class InstrumentMapper<TChord> where TChord : IChord
    {
        public abstract IEnumerable<MidiMappingResult> MapNoteEvent(uint position, NoteEvent e, ReadingSession session);
        public abstract IEnumerable<TrackObjectMappingResult> MapInstrument(Instrument<TChord> instrument);

        protected static NoteState GetState(NoteEvent note) => note is NoteOnEvent ? NoteState.Open : NoteState.Close;

        protected static T? HandleInvalidMidiEvent<T>(uint position, MidiEvent e, ReadingSession session)
        {
            session.HandleInvalidMidiEventType(position, e);
            return default;
        }
    }
}
