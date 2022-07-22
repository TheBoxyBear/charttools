using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Midi.Mapping;

namespace ChartTools.IO.Midi.Parsing
{
    internal class StandardInstrumentParser : LaneInstrumentParser<StandardChord, StandardLane, StandardChordModifier>
    {
        public new StandardInstrumentIdentity Instrument => (StandardInstrumentIdentity)base.Instrument;

        public StandardInstrumentParser(StandardInstrumentIdentity instrument, InstrumentMapper<StandardChord> mapper, ReadingSession session) : base((InstrumentIdentity)instrument, mapper, session) { }

        protected override StandardLane ToLane(byte index) => (StandardLane)index;
        protected override void AddModifier(StandardChord chord, byte index) => chord.Modifier |= (StandardChordModifier)index;
        protected override StandardChord CreateChord(uint position) => new(position);

        public override void ApplyToSong(Song song) => song.Instruments.Set(Result);
    }
}
