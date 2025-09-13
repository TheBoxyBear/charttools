using ChartTools.IO.Configuration;
using ChartTools.IO.Configuration.Common;

namespace ChartTools.IO.Chart.Configuration;

public abstract record CommonChartConfiguration : ICommonConfiguration
{
	public DuplicateTrackObjectPolicy DuplicateTrackObjectPolicy { get; init; }
	public OverlappingSpecialPhrasePolicy OverlappingStarPowerPolicy { get; init; }
	public SnappedNotesPolicy SnappedNotesPolicy { get; init; }
	public SoloNoStarPowerPolicy SoloNoStarPowerPolicy { get; init; }
}
