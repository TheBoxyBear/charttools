using ChartTools.IO.Chart;
using ChartTools.IO.Chart.Configuration;

namespace ChartTools.IO.Configuration;

public class WritingConfiguration
{
    public static readonly WritingConfiguration Default = new();

    public ChartWritingConfiguration Chart { get; set; } = ChartFile.DefaultWriteConfig;
}
