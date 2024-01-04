using ChartTools.IO.Configuration;
using ChartTools.IO.Configuration.Common;

namespace ChartTools.IO.Chart.Configuration;

public abstract record CommonChartConfiguration : ICommonConfiguration
{
    public required DuplicateTrackObjectPolicy DuplicateTrackObjectPolicy { get; init; }
    public required OverlappingSpecialPhrasePolicy OverlappingStarPowerPolicy { get; init; }
    public required SnappedNotesPolicy SnappedNotesPolicy { get; init; }
    public required SoloNoStarPowerPolicy SoloNoStarPowerPolicy { get; init; }
}
