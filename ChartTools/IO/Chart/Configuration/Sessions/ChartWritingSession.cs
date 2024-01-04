using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration;
using ChartTools.IO.Formatting;

namespace ChartTools.IO.Chart.Configuration.Sessions;

internal class ChartWritingSession(ChartWritingConfiguration? config, FormattingRules? formatting) : ChartSession(formatting)
{
    public override ChartWritingConfiguration Configuration { get; } = config ?? ChartFile.DefaultWriteConfig;

    public IEnumerable<TrackObjectEntry> GetUnsupportedModifierChordEntries(LaneChord? previous, LaneChord current) => Configuration.UnsupportedModifiersPolicy switch
    {
        UnsupportedModifiersPolicy.ThrowException => throw new Exception($"Chord at position {current.Position} as an unsupported modifier for the chart format."),
        UnsupportedModifiersPolicy.IgnoreChord    => Enumerable.Empty<TrackObjectEntry>(),
        UnsupportedModifiersPolicy.IgnoreModifier => current.GetChartNoteData(),
        UnsupportedModifiersPolicy.Convert        => current.GetChartModifierData(previous, this),
        _ => throw ConfigurationExceptions.UnsupportedPolicy(Configuration.UnsupportedModifiersPolicy)
    };
}
