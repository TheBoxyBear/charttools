using ChartTools.IO.Configuration;
using ChartTools.IO.Configuration.Common;

namespace ChartTools.IO.Midi.Configuration;

public record MidiWritingConfiguration : CommonMidiConfiguration, ICommonWritingConfiguration
{
    public required UnsupportedModifiersPolicy UnsupportedModifiersPolicy { get; init; }

    public required TrackObjectSource EventSource { get; init; }

    public required TrackObjectSource SpecialSource { get; init; }
}
