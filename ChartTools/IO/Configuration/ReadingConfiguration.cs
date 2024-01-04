using ChartTools.IO.Chart;
using ChartTools.IO.Chart.Configuration;
using ChartTools.IO.Midi;
using ChartTools.IO.Midi.Configuration;

namespace ChartTools.IO.Configuration;

public class ReadingConfiguration
{
    public static readonly ReadingConfiguration Default = new();

    public ChartReadingConfiguration Chart { get; set; } = ChartFile.DefaultReadConfig;
    public MidiReadingConfiguration Midi { get; set; } = MidiFile.DefaultReadConfig;
}
