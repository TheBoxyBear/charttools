using ChartTools.IO.Configuration.Sessions;
using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Midi.Parsers
{
    internal abstract class StandardInstrumentParser : InstrumentParser<StandardChord>
    {
        public new StandardInstrumentIdentity Instrument => (StandardInstrumentIdentity)base.Instrument;

        public StandardInstrumentParser(StandardInstrumentIdentity instrument, ReadingSession session) : base((InstrumentIdentity)instrument, session) { }


        protected abstract (Track<StandardChord> track, int adjustedNoteNumber) MapNoteEvent(NoteEvent e);

        public override void ApplyToSong(Song song) => song.Instruments.Set(result);

        protected Track<StandardChord> GetOrCreateTrack(Difficulty difficulty) => tracks[(int)difficulty] ??= new() { Difficulty = difficulty };
    }
}
