using ChartTools.IO.Configuration;
using ChartTools.IO.Formatting;

namespace ChartTools.IO.Midi.Configuration.Sessions;

internal abstract class MidiSession(FormattingRules? formatting) : Session(formatting)
{
    public override abstract CommonMidiConfiguration Configuration { get; }

    public abstract MidiInstrumentOrigin HandleUncertainFormat(StandardInstrumentIdentity instrument, MidiInstrumentOrigin format);
}
