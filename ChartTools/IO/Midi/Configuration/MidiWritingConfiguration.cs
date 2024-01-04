using ChartTools.IO.Configuration;
using ChartTools.IO.Configuration.Common;

namespace ChartTools.IO.Midi.Configuration;

public record MidiWritingConfiguration : CommonMidiConfiguration, ICommonWritingConfiguration
{
    public required UnsupportedModifierPolicy UnsupportedModifierPolicy { get; init; }
}
