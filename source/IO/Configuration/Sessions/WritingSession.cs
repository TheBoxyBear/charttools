using ChartTools.IO.Formatting;
using System;

namespace ChartTools.IO.Configuration.Sessions
{
    internal class WritingSession : Session
    {
        public delegate UnsupportedModifiersResults UnsupportedModifiersHandler(LaneChord chord);

        public override WritingConfiguration Configuration { get; }
        public UnsupportedModifiersHandler UnsupportedModifiersProcedure { get; private set; }

        public WritingSession(WritingConfiguration config, FormattingRules? formatting) : base(formatting)
        {
            Configuration = config;
            UnsupportedModifiersProcedure = chord => (UnsupportedModifiersProcedure = Configuration.UnsupportedModifierPolicy switch
            {
                UnsupportedModifiersPolicy.ThrowException => chord => throw new Exception($"Chord at position {chord.Position} has modifiers not supported by the target file type."),
                UnsupportedModifiersPolicy.IgnoreChord => _ => UnsupportedModifiersResults.None,
                UnsupportedModifiersPolicy.IgnoreModifier => chord => UnsupportedModifiersResults.Chord,
                UnsupportedModifiersPolicy.Convert => chord => UnsupportedModifiersResults.Modifier,
                _ => throw ConfigurationExceptions.UnsupportedPolicy(Configuration.UnsupportedModifierPolicy)
            })(chord);
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