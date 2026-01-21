using ChartTools.IO.Configuration;
using ChartTools.IO.Configuration.Common;

namespace ChartTools.IO.Chart.Configuration;

public record ChartWritingConfiguration : CommonChartConfiguration, ICommonWritingConfiguration
{
	public UnsupportedModifierPolicy UnsupportedModifierPolicy { get; init; }

    public ChartWritingConfiguration() : this(true) { }

    internal ChartWritingConfiguration(bool setDefaults)
    {
        if (setDefaults)
        {
            UnsupportedModifierPolicy  = ChartFile.DefaultWriteConfig.UnsupportedModifierPolicy;
            DuplicateTrackObjectPolicy = ChartFile.DefaultWriteConfig.DuplicateTrackObjectPolicy;
            OverlappingStarPowerPolicy = ChartFile.DefaultWriteConfig.OverlappingStarPowerPolicy;
            SnappedNotesPolicy         = ChartFile.DefaultWriteConfig.SnappedNotesPolicy;
            SoloNoStarPowerPolicy      = ChartFile.DefaultWriteConfig.SoloNoStarPowerPolicy;
        }
    }
}
