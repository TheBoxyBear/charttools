using ChartTools.IO.Formatting;
using ChartTools.IO.Midi;
using ChartTools.Tools;

using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Configuration.Sessions;

internal class ReadingSession : Session
{
    public delegate void InvalidMidiEventHandler(uint position, MidiEvent e);
    public delegate InstrumentSpecialPhrase? MisalignedBigRockHandler(IEnumerable<InstrumentSpecialPhrase> endings);
    public delegate bool TempolessAnchorHandler(Anchor anchor);
    public delegate void UnopenedUnclosedObjectHandler(uint position, Action createOrInclude);

    public override ReadingConfiguration Configuration { get; }
    public InvalidMidiEventHandler InvalidMidiEventProcedure { get; private set; }
    public MisalignedBigRockHandler MisalignedBigRockProcedure { get; private set; }
    public Func<bool> MissingBigRockProcedure { get; private set; }
    public UnopenedUnclosedObjectHandler UnopenedProcedure { get; private set; }
    public UnopenedUnclosedObjectHandler UnclosedProcedure { get; private set; }
    public TempolessAnchorHandler TempolessAnchorProcedure { get; private set; }

    public ReadingSession(ReadingConfiguration config, FormattingRules? formatting) : base(formatting)
    {
        Configuration = config;

        InvalidMidiEventProcedure = (position, e) => (InvalidMidiEventProcedure = Configuration.IgnoreInvalidMidiEvent
        ? (position, e) => throw new InvalidMidiEventTypeException(position, e)
        : (_, _) => { })(position, e);
        MisalignedBigRockProcedure = endings => (MisalignedBigRockProcedure = Configuration.MisalignedBigRockMarkersPolicy switch
        {
            MisalignedBigRockMarkersPolicy.ThrowException => _ => throw new Exception("Big rock endings are misaligned."),
            MisalignedBigRockMarkersPolicy.IgnoreAll => _ => null,
            MisalignedBigRockMarkersPolicy.IncludeFirst => endings => endings.First(),
            MisalignedBigRockMarkersPolicy.Combine => endings => LengthMerger.MergeLengths(endings)
        })(endings);
        MissingBigRockProcedure = () => (MissingBigRockProcedure = Configuration.MissingBigRockMarkerPolicy switch
        {
            MissingBigRockMarkerPolicy.ThrowException => () => throw new Exception("One or more big rock ending marker is missing."),
            MissingBigRockMarkerPolicy.IgnoreAll => () => false,
            MissingBigRockMarkerPolicy.IgnoreMissing => () => true,
            _ => throw ConfigurationExceptions.UnsupportedPolicy(Configuration.MissingBigRockMarkerPolicy)
        })();
        UnopenedProcedure = (position, create) => (UnopenedProcedure = Configuration.UnopenedTrackObjectPolicy switch
        {
            UnopenedTrackObjectPolicy.ThrowException => (position, _) => throw new Exception($"Object at position {position} closed before being opened."), // TODO Create exception
            UnopenedTrackObjectPolicy.Ignore => (_, _) => { },
            UnopenedTrackObjectPolicy.Create => (_, create) => create(),
            _ => throw ConfigurationExceptions.UnsupportedPolicy(Configuration.UnopenedTrackObjectPolicy)
        })(position, create);
        UnclosedProcedure = (position, include) => (UnclosedProcedure = Configuration.UnclosedTrackObjectPolicy switch
        {
            UnclosedTrackObjectPolicy.ThrowException => throw new Exception($"Object at position {position} opened but never closed."), // TODO Create exception
            UnclosedTrackObjectPolicy.Ignore => (_, _) => { },
            UnclosedTrackObjectPolicy.Include => (_, include) => include()
        })(position, include);
        TempolessAnchorProcedure = anchor => (TempolessAnchorProcedure = Configuration.TempolessAnchorPolicy switch
        {
            TempolessAnchorPolicy.ThrowException => anchor => throw new Exception($"Tempo anchor at position {anchor.Position} does not have a parent tempo marker."),
            TempolessAnchorPolicy.Ignore => anchor => false,
            TempolessAnchorPolicy.Create => anchor => true,
            _ => throw ConfigurationExceptions.UnsupportedPolicy(Configuration.TempolessAnchorPolicy)
        })(anchor);
        UncertainFormatProcedure = (instrument, format) => (UncertainFormatProcedure = Configuration.UncertainFormatPolicy switch
        {
            UncertainFormatPolicy.ThrowException => (instrument, format) => throw new Exception($"{instrument} has the unknown or conflicting format {format} that cannot be mapped from Midi."),
            UncertainFormatPolicy.UseGuitarHero2 => (_, _) => MidiInstrumentOrigin.GuitarHero2Uncertain,
            UncertainFormatPolicy.UseRockBand => (_, _) => MidiInstrumentOrigin.RockBandUncertain,
            _ => throw ConfigurationExceptions.UnsupportedPolicy(Configuration.UncertainFormatPolicy)
        })(instrument, format);
    }
}