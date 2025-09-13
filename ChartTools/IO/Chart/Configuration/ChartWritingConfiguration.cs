using ChartTools.IO.Configuration;
using ChartTools.IO.Configuration.Common;

namespace ChartTools.IO.Chart.Configuration;

public record ChartWritingConfiguration : CommonChartConfiguration, ICommonWritingConfiguration
{
	public UnsupportedModifierPolicy UnsupportedModifierPolicy { get; init; } = ChartFile.DefaultWriteConfig.UnsupportedModifierPolicy;

    ChartWritingConfiguration()
    {
        DuplicateTrackObjectPolicy = ChartFile.DefaultWriteConfig.DuplicateTrackObjectPolicy;
        OverlappingStarPowerPolicy = ChartFile.DefaultWriteConfig.OverlappingStarPowerPolicy;
        SnappedNotesPolicy         = ChartFile.DefaultWriteConfig.SnappedNotesPolicy;
        SoloNoStarPowerPolicy      = ChartFile.DefaultWriteConfig.SoloNoStarPowerPolicy;
    }
}
