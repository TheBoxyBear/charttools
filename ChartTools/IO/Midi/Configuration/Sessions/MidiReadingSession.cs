using ChartTools.IO.Configuration;
using ChartTools.IO.Formatting;
using ChartTools.Tools;
using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Midi.Configuration.Sessions;

internal class MidiReadingSession(MidiReadingConfiguration? config, FormattingRules? formatting) : MidiSession(formatting)
{
    public override MidiReadingConfiguration Configuration { get; } = config ?? MidiFile.DefaultReadConfig;

    public void HandleInvalidMidiEvent(uint position, MidiEvent e)
    {
        switch (Configuration.InvalidMidiEventPolicy)
        {
            case InvalidMidiEventPolicy.ThrowException:
                throw new InvalidMidiEventTypeException(position, e);
            case InvalidMidiEventPolicy.Ignore:
                return;
            default:
                throw ConfigurationExceptions.UnsupportedPolicy(Configuration.InvalidMidiEventPolicy);
        }
    }

    public InstrumentSpecialPhrase? HandleMisalignedBigRock(IEnumerable<InstrumentSpecialPhrase> endings) => Configuration.MisalignedBigRockMarkersPolicy switch
    {
        MisalignedBigRockMarkersPolicy.ThrowException => throw new Exception("Big rock endings are misaligned."),
        MisalignedBigRockMarkersPolicy.IgnoreAll      => null,
        MisalignedBigRockMarkersPolicy.IncludeFirst   => endings.FirstOrDefault(),
        MisalignedBigRockMarkersPolicy.Combine        => LengthMerger.MergeLengths(endings),
        _ => throw ConfigurationExceptions.UnsupportedPolicy(Configuration.MisalignedBigRockMarkersPolicy),
    };

    public bool HandleMissingBigRock() => Configuration.MissingBigRockMarkerPolicy switch
    {
        MissingBigRockMarkerPolicy.ThrowException => throw new Exception("One or more big rock ending marker are missing."),
        MissingBigRockMarkerPolicy.IgnoreAll      => false,
        MissingBigRockMarkerPolicy.IgnoreMissing  => true,
        _ => throw ConfigurationExceptions.UnsupportedPolicy(Configuration.MissingBigRockMarkerPolicy),
    };

    public bool HandleUnopened(uint position) => Configuration.UnopenedTrackObjectPolicy switch
    {
        UnopenedTrackObjectPolicy.ThrowException => throw new Exception($"Object at position {position} closed before being opened."),// TODO Create exception
        UnopenedTrackObjectPolicy.Ignore         => false,
        UnopenedTrackObjectPolicy.Create         => true,
        _ => throw ConfigurationExceptions.UnsupportedPolicy(Configuration.UnopenedTrackObjectPolicy),
    };

    public bool HandleUnclosed(uint position) => Configuration.UnclosedTrackObjectPolicy switch
    {
        UnclosedTrackObjectPolicy.ThrowException => throw new Exception($"Object at position {position} opened but never closed."),// TODO Create exception
        UnclosedTrackObjectPolicy.Ignore         => false,
        UnclosedTrackObjectPolicy.Include        => true,
        _ => throw ConfigurationExceptions.UnsupportedPolicy(Configuration.UnclosedTrackObjectPolicy),
    };

    public override MidiInstrumentOrigin HandleUncertainFormat(StandardInstrumentIdentity instrument, MidiInstrumentOrigin format) => Configuration.UncertainFormatPolicy switch
    {
        UncertainFormatPolicy.ThrowException => throw new Exception($"{instrument} has the unknown or conflicting format {format} that cannot be mapped from Midi."),
        UncertainFormatPolicy.UseGuitarHero2 => MidiInstrumentOrigin.GuitarHero2Uncertain,
        UncertainFormatPolicy.UseRockBand    => MidiInstrumentOrigin.RockBandUncertain,
        _ => throw ConfigurationExceptions.UnsupportedPolicy(Configuration.UncertainFormatPolicy)
    };
}
