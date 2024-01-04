using ChartTools.IO.Configuration;
using ChartTools.IO.Configuration.Common;

namespace ChartTools.IO.Midi.Configuration;

public record CommonMidiConfiguration : CommonConfiguration
{
    /// <inheritdoc cref="Configuration.UncertainFormatPolicy"/>
    public required UncertainFormatPolicy UncertainFormatPolicy { get; init; }

}
