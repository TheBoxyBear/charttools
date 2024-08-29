using ChartTools.IO.Components;
using ChartTools.IO.Configuration;
using ChartTools.IO.Formatting;

namespace ChartTools.IO.Chart.Configuration.Sessions;

internal abstract class ChartSession(FormattingRules? formatting) : Session(formatting)
{
    public override abstract CommonChartConfiguration Configuration { get; }
}
