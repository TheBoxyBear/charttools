using ChartTools.Lyrics;

namespace ChartTools;

public record Vocals : Instrument<Phrase>
{
    protected override InstrumentIdentity GetIdentity() => InstrumentIdentity.Vocals;
}
