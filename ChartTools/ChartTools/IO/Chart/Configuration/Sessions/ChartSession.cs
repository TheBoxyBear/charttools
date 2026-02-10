using ChartTools.IO.Configuration;
using ChartTools.IO.Formatting;

namespace ChartTools.IO.Chart.Configuration.Sessions;

internal abstract class ChartSession(FormattingRules? formatting)
	: Session(formatting)
{
#if NET5_0_OR_GREATER
	public override abstract CommonChartConfiguration Configuration { get; }
#else
	public new CommonChartConfiguration Configuration { get; }

	protected override IO.Configuration.Common.ICommonConfiguration GetCommonConfiguration()
		=> Configuration;
#endif
}
