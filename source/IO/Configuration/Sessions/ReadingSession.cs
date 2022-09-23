using ChartTools.IO.Formatting;

using ChartTools.IO.Formatting;
using ChartTools.IO.Midi;
using ChartTools.Tools;

using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Configuration.Sessions
{
    internal class ReadingSession : Session
    {
        public delegate void InvalidMidiEventTypeHandler(uint position, MidiEvent e);
        public delegate InstrumentSpecialPhrase? MisalignedBigRockHandler(IEnumerable<InstrumentSpecialPhrase> endings);
        public delegate bool TempolessAnchorHandler(Anchor anchor);
        public delegate void UnopenedUnclosedObjectHandler(uint position, Action createOrInclude);
        public delegate MidiInstrumentOrigin UncertainGuitarBassFormatHandler(StandardInstrumentIdentity instrument);

        public override ReadingConfiguration Configuration { get; }
        public InvalidMidiEventTypeHandler HandleInvalidMidiEventType { get; private set; }
        public MisalignedBigRockHandler HandleMisalignedBigRock { get; private set; }
        public Func<bool> HandleMissingBigRock { get; private set; }
        public UnopenedUnclosedObjectHandler HandleUnopened { get; private set; }
        public UnopenedUnclosedObjectHandler HandleUnclosed { get; private set; }
        public TempolessAnchorHandler TempolessAnchorProcedure { get; private set; }
        public UncertainGuitarBassFormatHandler UncertainGuitarBassFormatProcedure { get; private set; }

        public ReadingSession(ReadingConfiguration config, FormattingRules? formatting) : base(formatting)
        {
            Configuration = config;

            HandleInvalidMidiEventType = (position, e) => (HandleInvalidMidiEventType = Configuration.IgnoreInvalidMidiEventType
            ? (position, e) => throw new InvalidMidiEventTypeException(position, e)
            : (_, _) => { })(position, e);
            HandleMisalignedBigRock = endings => (HandleMisalignedBigRock = Configuration.MisalignedBigRockMarkersPolicy switch
            {
                MisalignedBigRockMarkersPolicy.ThrowException => _ => throw new Exception("Big rock endings are misaligned."),
                MisalignedBigRockMarkersPolicy.IgnoreAll => _ => null,
                MisalignedBigRockMarkersPolicy.IncludeFirst => endings => endings.First(),
                MisalignedBigRockMarkersPolicy.Combine => endings => LengthMerger.MergeLengths(endings)
            })(endings);
            HandleMissingBigRock = () => (HandleMissingBigRock = Configuration.MissingBigRockMarkerPolicy switch
            {
                MissingBigRockMarkerPolicy.ThrowException => () => throw new Exception("One or more big rock ending marker is missing."),
                MissingBigRockMarkerPolicy.IgnoreAll => () => false,
                MissingBigRockMarkerPolicy.IgnoreMissing => () => true,
                _ => throw ConfigurationExceptions.UnsupportedPolicy(Configuration.MissingBigRockMarkerPolicy)
            })();
            HandleUnopened = (position, create) => (HandleUnopened = Configuration.UnopenedTrackObjectPolicy switch
            {
                UnopenedTrackObjectPolicy.ThrowException => (position, _) => throw new Exception($"Object at position {position} closed before being opened."), // TODO Create exception
                UnopenedTrackObjectPolicy.Ignore => (_, _) => { },
                UnopenedTrackObjectPolicy.Create => (_, create) => create(),
                _ => throw ConfigurationExceptions.UnsupportedPolicy(Configuration.UnopenedTrackObjectPolicy)
            })(position, create);
            HandleUnclosed = (position, include) => (HandleUnclosed = Configuration.UnclosedTracjObjectPolicy switch
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
            UncertainGuitarBassFormatProcedure = instrument => (UncertainGuitarBassFormatProcedure = Configuration.UncertainGuitarBassFormatPolicy switch
            {
                UncertainGuitarBassFormatPolicy.UseGuitarHero2 => _ => MidiInstrumentOrigin.GuitarHero2Uncertain,
                UncertainGuitarBassFormatPolicy.UseRockBand => _ => MidiInstrumentOrigin.RockBandUncertain,
                UncertainGuitarBassFormatPolicy.ThrowException => instrument => throw new Exception($"{instrument} has an unknown or conflicting format that cannot be mapped from Midi."),
                _ => throw ConfigurationExceptions.UnsupportedPolicy(Configuration.UncertainGuitarBassFormatPolicy)
            })(instrument);
         }
    }
}
