using ChartTools.IO.Components;
using ChartTools.IO.Configuration;

namespace ChartTools.IO.Chart.Configuration.Sessions;

internal class ChartReadingSession(ComponentList components, ChartReadingConfiguration? config)
	: ChartSession(null)
{
	public ComponentList Components { get; set; } = components;

#if NET5_0_OR_GREATER
	public override ChartReadingConfiguration Configuration { get; } = config ?? ChartFile.DefaultReadConfig;
#else
	public new ChartReadingConfiguration Configuration { get; } = config ?? ChartFile.DefaultReadConfig;

	protected override IO.Configuration.Common.ICommonConfiguration GetCommonConfiguration()
		=> Configuration;
#endif

	public bool HandleTempolessAnchor(Anchor anchor)
		=> Configuration.TempolessAnchorPolicy switch
		{
			TempolessAnchorPolicy.ThrowException => throw new Exception($"Tempo anchor at position {anchor.Position} does not have a parent tempo marker."),
			TempolessAnchorPolicy.Ignore         => false,
			TempolessAnchorPolicy.Create         => true,
			_ => throw ConfigurationExceptions.UnsupportedPolicy(Configuration.TempolessAnchorPolicy)
		};
}
