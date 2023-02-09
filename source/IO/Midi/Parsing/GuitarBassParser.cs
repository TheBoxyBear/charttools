using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Midi.Mapping;

namespace ChartTools.IO.Midi.Parsing;

internal class GuitarBassParser : StandardInstrumentParser
{
    public override MidiInstrumentOrigin Format => _format;
    private MidiInstrumentOrigin _format;

    protected override byte BigRockCount => 5;

    public GuitarBassParser(StandardInstrumentIdentity instrument, ReadingSession session) : base(new GuitarBassMapper(instrument), instrument, session)
    {
        if (instrument is not StandardInstrumentIdentity.LeadGuitar or StandardInstrumentIdentity.Bass)
            throw new ArgumentException($"Instrument must be lead guitar or bass to use to use {nameof(GuitarBassParser)}.", nameof(instrument));
    }

    protected override void FinaliseParse()
    {
        var origin = Format;

        if (origin == MidiInstrumentOrigin.Unknown)
            _format = session.UncertainGuitarBassFormatProcedure(Instrument, origin);

        base.FinaliseParse();
    }
}
