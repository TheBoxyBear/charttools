using ChartTools.IO.Configuration;
using ChartTools.IO.Configuration.Common;

namespace ChartTools.IO.Chart.Configuration;

/// <summary>
/// Set of options that control how a chart file is read
/// </summary>
public record ChartReadingConfiguration : CommonChartConfiguration, ICommonReadingConfiguration
{
	/// <inheritdoc cref="IO.Configuration.TempolessAnchorPolicy"/>
	public TempolessAnchorPolicy TempolessAnchorPolicy { get; init; }

	/// <inheritdoc cref="IO.Configuration.UnknownSectionPolicy"/>/>
	public UnknownSectionPolicy UnknownSectionPolicy { get; init; }

	public ChartReadingConfiguration() : this(true) { }

	internal ChartReadingConfiguration(bool setDefaults)
	{
		if (setDefaults)
		{
            DuplicateTrackObjectPolicy = ChartFile.DefaultReadConfig.DuplicateTrackObjectPolicy;
            OverlappingStarPowerPolicy = ChartFile.DefaultReadConfig.OverlappingStarPowerPolicy;
            SnappedNotesPolicy         = ChartFile.DefaultReadConfig.SnappedNotesPolicy;
            SoloNoStarPowerPolicy      = ChartFile.DefaultReadConfig.SoloNoStarPowerPolicy;
			TempolessAnchorPolicy      = ChartFile.DefaultReadConfig.TempolessAnchorPolicy;
			UnknownSectionPolicy       = ChartFile.DefaultReadConfig.UnknownSectionPolicy;
        }
	}
}
