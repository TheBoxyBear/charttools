using ChartTools.IO;
using ChartTools.IO.Chart;
using ChartTools.IO.Configuration;
using ChartTools.IO.Formatting;

namespace ChartTools;

public record Drums : Instrument<DrumsChord>
{
    protected override InstrumentIdentity GetIdentity() => InstrumentIdentity.Drums;
}
