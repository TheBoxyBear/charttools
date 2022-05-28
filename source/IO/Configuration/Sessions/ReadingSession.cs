using System;
using System.Collections.Generic;

using ChartTools.Formatting;
using ChartTools.IO.Chart;
using ChartTools.IO.Midi;
using ChartTools.SystemExtensions;

using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Configuration.Sessions
{
    internal class ReadingSession : Session
    {
        public delegate void InvalidMidiEventTypeHandler(uint position, MidiEvent e);
        public delegate void UnopenedUnclosedObjectHandler(uint position, Action createOrInclude);
        public delegate bool TempolessAnchorHandler(Anchor anchor);

        public override ReadingConfiguration Configuration { get; }

        public InvalidMidiEventTypeHandler HandleInvalidMidiEventType { get; private set; }
        public UnopenedUnclosedObjectHandler HandleUnopened { get; private set; }
        public UnopenedUnclosedObjectHandler HandleUnclosed { get; private set; }
        public TempolessAnchorHandler TempolessAnchorProcedure { get; private set; }

        public ReadingSession(ReadingConfiguration config, FormattingRules? formatting) : base(formatting)
        {
            Configuration = config;

            HandleInvalidMidiEventType = (position, e) => (HandleInvalidMidiEventType = Configuration.InvalidMidiEventTypePolicy switch
            {
                InvalidMidiEventTypePolicy.ThrowException => (position, e) => throw new InvalidMidiEventTypeException(position, e),
                InvalidMidiEventTypePolicy.Ignore => (_, _) => { },
                _ => throw ConfigurationExceptions.UnsupportedPolicy(Configuration.InvalidMidiEventTypePolicy)
            })(position, e);
            HandleUnopened = (position, create) => (HandleUnopened = Configuration.UnopenedTrackObjectPolicy switch
            {
                UnopenedTrackObjectPolicy.ThrowException => (position, _) => throw new Exception($"Object at position {position} closed before being opened. Consider using a different {nameof(UnopenedTrackObjectPolicy)} to avoid this error."), // TODO Create exception
                UnopenedTrackObjectPolicy.Ignore => (_, _) => { },
                UnopenedTrackObjectPolicy.Create => (_, create) => create(),
                _ => throw ConfigurationExceptions.UnsupportedPolicy(Configuration.UnopenedTrackObjectPolicy)
            })(position, create);
            HandleUnclosed = (position, include) => (HandleUnclosed = Configuration.UnclosedTracjObjectPolicy switch
            {
                UnclosedTrackObjectPolicy.ThrowException => throw new Exception($"Object at position {position} opened but never closed. Consider using a different {nameof(UnclosedTrackObjectPolicy)} to avoid this error."), // TODO Create exception
                UnclosedTrackObjectPolicy.Ignore => (_, _) => { },
                UnclosedTrackObjectPolicy.Include => (_, include) => include()
            })(position, include);
            TempolessAnchorProcedure = anchor => (TempolessAnchorProcedure = Configuration.TempolessAnchorPolicy switch
            {
                TempolessAnchorPolicy.ThrowException => anchor => throw new Exception($"Tempo anchor at position {anchor.Position} does not have a parent tempo marker. Consider using a different {nameof(TempolessAnchorPolicy)} to avoid this error."),
                TempolessAnchorPolicy.Ignore => anchor => false,
                TempolessAnchorPolicy.Create => anchor => true,
                _ => throw ConfigurationExceptions.UnsupportedPolicy(Configuration.TempolessAnchorPolicy)
            })(anchor);
         }
    }
}
