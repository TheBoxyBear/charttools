using ChartTools.IO.Configuration;
using ChartTools.IO.Configuration.Common;

namespace ChartTools.IO.Chart.Configuration;

public record ChartReadingConfiguration : CommonChartConfiguration, ICommonReadingConfiguration
{
    /// <inheritdoc cref="IO.Configuration.TempolessAnchorPolicy"/>
    public required TempolessAnchorPolicy TempolessAnchorPolicy { get; init; }

    public required UnknownSectionPolicy UnknownSectionPolicy { get; init; }
}
