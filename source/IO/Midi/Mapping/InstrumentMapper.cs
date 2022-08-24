using ChartTools.IO.Configuration.Sessions;
using Melanchall.DryWetMidi.Core;

using System;
using System.Collections.Generic;

namespace ChartTools.IO.Midi.Mapping
{
    internal abstract class InstrumentMapper<TChord> where TChord : Chord
    {
        public ReadingSession? ReadingSession { get; }
        public WritingSession? WritingSession { get; }

        public InstrumentMapper(ReadingSession? readingSession = null, WritingSession? writingSession = null)
        {
            ReadingSession = readingSession;
            WritingSession = writingSession;
        }

        public abstract IEnumerable<NoteEventMapping> Map(uint position, NoteEvent e);
        public abstract IEnumerable<NoteMapping> Map(Instrument<TChord> instrument);

        protected T? HandleInvalidMidiEvent<T>(uint position, NoteEvent e)
        {
            (ReadingSession ?? throw new NullReferenceException("Reading session is null")).InvalidMidiEventTypeProcedure(position, e);
            return default;
        }

    }
}
