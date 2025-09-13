using ChartTools.IO.Configuration;
using ChartTools.IO.Configuration.Common;

namespace ChartTools.IO.Chart.Configuration;

/// <summary>
/// Set of options that control how a chart file is read
/// </summary>
public record ChartReadingConfiguration : CommonChartConfiguration, ICommonReadingConfiguration
{
	/// <inheritdoc cref="IO.Configuration.TempolessAnchorPolicy"/>
	public required TempolessAnchorPolicy TempolessAnchorPolicy { get; init; }

	/// <inheritdoc cref="IO.Configuration.UnknownSectionPolicy"/>/>
	public required UnknownSectionPolicy UnknownSectionPolicy { get; init; }
}
