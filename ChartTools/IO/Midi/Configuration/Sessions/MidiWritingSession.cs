using ChartTools.IO.Formatting;

namespace ChartTools.IO.Midi.Configuration.Sessions;

internal class MidiWritingSession(MidiWritingConfiguration? config, FormattingRules? formatting) : MidiSession(formatting)
{
    public override MidiWritingConfiguration Configuration { get; } = config ?? MidiFile.DefaultWriteConfig;

    public override MidiInstrumentOrigin HandleUncertainFormat(StandardInstrumentIdentity instrument, MidiInstrumentOrigin format)
    {
        throw new NotImplementedException();
    }
}
