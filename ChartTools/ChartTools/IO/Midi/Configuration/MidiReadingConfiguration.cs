using ChartTools.IO.Configuration;
using ChartTools.IO.Configuration.Common;
using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Midi.Configuration;

public record MidiReadingConfiguration : CommonMidiConfiguration, ICommonReadingConfiguration
{

    /// <inheritdoc cref="Configuration.InvalidMidiEventPolicy"/>
    public required InvalidMidiEventPolicy InvalidMidiEventPolicy { get; init; }

    /// <inheritdoc cref="Configuration.MisalignedBigRockMarkersPolicy"/>
    public required MisalignedBigRockMarkersPolicy MisalignedBigRockMarkersPolicy { get; init; }

    /// <inheritdoc cref="Configuration.MissingBigRockMarkerPolicy"/>
    public required MissingBigRockMarkerPolicy MissingBigRockMarkerPolicy { get; init; }

    public required UnknownSectionPolicy UnknownSectionPolicy { get; init; }

    /// <inheritdoc cref="Configuration.UnopenedTrackObjectPolicy"/>
    public required UnopenedTrackObjectPolicy UnopenedTrackObjectPolicy { get; init; }

    /// <inheritdoc cref="Configuration.UnclosedTrackObjectPolicy"/>
    public required UnclosedTrackObjectPolicy UnclosedTrackObjectPolicy { get; init; }

    /// <summary>
    /// Configuration object to customize how DryWetMidi reads Midi files before being parsed
    /// </summary>
    /// <remarks>Setting to <see landword="null"/> will use default settings</remarks>
    public ReadingSettings? FirstPassReadingSettings { get; set; }
}
