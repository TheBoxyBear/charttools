using ChartTools.IO.Chart;
using ChartTools.IO.Chart.Configuration;
using ChartTools.IO.Midi;
using ChartTools.IO.Midi.Configuration;

namespace ChartTools.IO.Configuration;

public class WritingConfiguration
{
    public static readonly WritingConfiguration Default = new();

    public ChartWritingConfiguration Chart { get; set; } = ChartFile.DefaultWriteConfig;
    public MidiWritingConfiguration Midi { get; set; } = MidiFile.DefaultWriteConfig;
}
