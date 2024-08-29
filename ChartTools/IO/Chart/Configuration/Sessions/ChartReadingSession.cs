using ChartTools.IO.Components;
using ChartTools.IO.Configuration;
using ChartTools.IO.Formatting;

namespace ChartTools.IO.Chart.Configuration.Sessions;

internal class ChartReadingSession(ComponentList components, ChartReadingConfiguration? config, FormattingRules? formatting)
    : ChartSession(formatting)
{
    public ComponentList Components { get; set; } = components;

    public override ChartReadingConfiguration Configuration { get; } = config ?? ChartFile.DefaultReadConfig;

    public bool HandleTempolessAnchor(Anchor anchor) => Configuration.TempolessAnchorPolicy switch
    {
        TempolessAnchorPolicy.ThrowException => throw new Exception($"Tempo anchor at position {anchor.Position} does not have a parent tempo marker."),
        TempolessAnchorPolicy.Ignore         => false,
        TempolessAnchorPolicy.Create         => true,
        _ => throw ConfigurationExceptions.UnsupportedPolicy(Configuration.TempolessAnchorPolicy)
    };
}
