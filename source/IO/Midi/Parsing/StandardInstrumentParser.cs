using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Midi.Mapping;

namespace ChartTools.IO.Midi.Parsing;

internal class StandardInstrumentParser : LaneInstrumentParser<StandardChord, StandardLane, StandardChordModifiers>
{
    public StandardInstrumentIdentity Instrument { get; }

    public override StandardInstrument Result => GetResult(result);
    protected readonly StandardInstrument result;

    public override StandardInstrumentMapper Mapper => (StandardInstrumentMapper)base.Mapper;

    public StandardInstrumentParser(StandardInstrumentMapper mapper, StandardInstrumentIdentity instrument, ReadingSession session) : base(mapper, session)
    {
        Instrument = instrument;
        result = new(instrument);
    }

    protected override StandardInstrument GetInstrument() => result;

    protected override void AddModifier(StandardChord chord, byte index) => chord.Modifiers |= (StandardChordModifiers)index;
    protected override StandardChord CreateChord(uint position) => new(position);

    protected override void FinaliseParse()
    {
        var origin = Mapper.Format;

        if (origin == MidiInstrumentOrigin.Unknown)
            origin = session.UncertainFormatProcedure(Instrument, origin);

        base.FinaliseParse();
    }

    public override void ApplyToSong(Song song)
    {
        base.ApplyToSong(song);
        song.Instruments.Set(Result);
    }
}
