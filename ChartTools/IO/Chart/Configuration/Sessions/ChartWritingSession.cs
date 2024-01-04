using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration;
using ChartTools.IO.Configuration.Common;
using ChartTools.IO.Formatting;

namespace ChartTools.IO.Chart.Configuration.Sessions;

internal class ChartWritingSession(ChartWritingConfiguration? config, FormattingRules? formatting) : ChartSession(formatting)
{
    public override ChartWritingConfiguration Configuration { get; } = config ?? ChartFile.DefaultWriteConfig;
}
