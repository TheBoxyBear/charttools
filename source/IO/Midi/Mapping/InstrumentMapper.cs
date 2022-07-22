using Melanchall.DryWetMidi.Core;
using System.Collections.Generic;

namespace ChartTools.IO.Midi.Mapping
{
    internal abstract class InstrumentMapper<TChord> where TChord : Chord
    {
        public abstract IEnumerable<MidiMappingResult> MapNoteEvent(uint position, NoteEvent e);
        public abstract IEnumerable<TrackObjectMappingResult> MapInstrument(Instrument<TChord> instrument);

        protected NoteState GetState(NoteEvent note) => note is NoteOnEvent ? NoteState.Open : NoteState.Close;
    }
}
