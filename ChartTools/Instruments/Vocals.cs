namespace ChartTools.Lyrics;

public record Vocals : Instrument<Phrase>
{
    protected override InstrumentIdentity GetIdentity() => InstrumentIdentity.Vocals;
}
