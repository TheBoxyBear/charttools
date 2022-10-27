using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Midi.Mapping;

namespace ChartTools.IO.Midi.Parsing
{
    internal class StandardInstrumentParser : LaneInstrumentParser<StandardChord, StandardLane, StandardChordModifiers>
    {
        public StandardInstrumentIdentity Instrument { get; }
        public virtual MidiInstrumentOrigin Origin => MidiInstrumentOrigin.RockBand;

        public override StandardInstrument Result => GetResult(result);
        private readonly StandardInstrument result;

        public StandardInstrumentParser(StandardInstrumentIdentity instrument, InstrumentMapper<StandardChord> mapper, ReadingSession session) : base(mapper, session)
        {
            Instrument = instrument;
            result = new(instrument);
        }

        protected override StandardInstrument GetInstrument() => result;

        protected override void HandleBigRock(NoteEventMapping mapping)
        {
            if (!Origin.HasFlag(MidiInstrumentOrigin.RockBand))
                return;

            base.HandleBigRock(mapping);
        }

        protected override StandardLane ToLane(byte index) => (StandardLane)index;
        protected override void AddModifier(StandardChord chord, byte index) => chord.Modifiers |= (StandardChordModifiers)index;
        protected override StandardChord CreateChord(uint position) => new(position);

        public override void ApplyToSong(Song song) => song.Instruments.Set(Result);
    }
}
