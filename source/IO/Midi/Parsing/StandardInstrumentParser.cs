using ChartTools.IO.Configuration.Sessions;

namespace ChartTools.IO.Midi.Parsing
{
    internal abstract class StandardInstrumentParser : LaneInstrumentParser<StandardChord, Note<StandardLane>, StandardLane, StandardChordModifier>
    {
        public new StandardInstrumentIdentity Instrument => (StandardInstrumentIdentity)base.Instrument;

        public StandardInstrumentParser(StandardInstrumentIdentity instrument, ReadingSession session) : base((InstrumentIdentity)instrument, session) { }

        protected override StandardLane ToLane(byte index) => (StandardLane)index;
        protected override void AddModifier(StandardChord chord, byte index) => chord.Modifier |= (StandardChordModifier)index;
        protected override StandardChord CreateChord(uint position) => new(position);

        public override void ApplyToSong(Song song) => song.Instruments.Set(Result);
    }
}
