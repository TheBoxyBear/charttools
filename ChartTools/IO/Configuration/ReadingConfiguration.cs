using ChartTools.IO.Chart;
using ChartTools.IO.Chart.Configuration;

namespace ChartTools.IO.Configuration;

public class ReadingConfiguration
{
    public static readonly ReadingConfiguration Default = new();

    public ChartReadingConfiguration Chart { get; set; } = ChartFile.DefaultReadConfig;
}
