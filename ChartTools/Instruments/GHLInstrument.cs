namespace ChartTools;

public record GHLInstrument : Instrument<GHLChord>
{
    public new GHLInstrumentIdentity InstrumentIdentity { get; init; }

    public GHLInstrument() { }
    public GHLInstrument(GHLInstrumentIdentity identity) => InstrumentIdentity = identity;

    protected override InstrumentIdentity GetIdentity() => (InstrumentIdentity)InstrumentIdentity;
}
