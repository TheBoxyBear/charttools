using ChartTools.Formatting;
using ChartTools.IO.Chart.Entries;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools.IO.Configuration.Sessions
{
    internal class WritingSession : Session
    {
        public delegate IEnumerable<TrackObjectEntry> ChordEntriesGetter(Chord? previous, Chord current);

        public override WritingConfiguration Configuration { get; }
        public ChordEntriesGetter GetChordEntries { get; private set; }

        public WritingSession(WritingConfiguration config, FormattingRules? formatting) : base(formatting)
        {
            Configuration = config;
            GetChordEntries = (previous, chord) => (GetChordEntries = Configuration.UnsupportedModifierPolicy switch
            {
                UnsupportedModifierPolicy.IgnoreChord => (_, _) => Enumerable.Empty<TrackObjectEntry>(),
                UnsupportedModifierPolicy.ThrowException => (_, chord) => throw new Exception($"Chord at position {chord.Position} as an unsupported modifier for the chart format."),
                UnsupportedModifierPolicy.IgnoreModifier => (_, chord) => chord.GetChartNoteData(),
                UnsupportedModifierPolicy.Convert => (previous, chord) => chord.GetChartModifierData(previous, this),
                _ => throw ConfigurationExceptions.UnsupportedPolicy(Configuration.UnsupportedModifierPolicy)
            })(previous, chord);
            UncertainGuitarBassFormatProcedure = (instrument, format) => (UncertainGuitarBassFormatProcedure = Configuration.UncertainGuitarBassFormatPolicy switch
            {
                UncertainGuitarBassFormatPolicy.ThrowException => (instrument, format) => throw new Exception($"{instrument} has the unknown or conflicting format {format} that cannot be mapped to Midi."),
                UncertainGuitarBassFormatPolicy.UseReadingDefault => (_, format) => format & (MidiInstrumentOrigin)(byte.MaxValue & (byte)MidiInstrumentOrigin.Unknown),
                UncertainGuitarBassFormatPolicy.UseGuitarHero2 => (_, _) => MidiInstrumentOrigin.GuitarHero2Uncertain,
                UncertainGuitarBassFormatPolicy.UseRockBand => (_, _) => MidiInstrumentOrigin.RockBandUncertain,
                _ => throw ConfigurationExceptions.UnsupportedPolicy(Configuration.UncertainGuitarBassFormatPolicy)
            })(instrument, format);
        }
    }

}
