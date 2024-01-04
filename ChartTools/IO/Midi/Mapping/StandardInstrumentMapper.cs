using ChartTools.IO.Midi.Configuration.Sessions;

namespace ChartTools.IO.Midi.Mapping;

internal abstract class StandardInstrumentMapper : LaneInstrumentMapper<StandardChord>
{
    public abstract MidiInstrumentOrigin Format { get; }

    protected StandardInstrumentMapper(MidiReadingSession session) : base(session) { }
    protected StandardInstrumentMapper(MidiWritingSession session) : base(session) { }
}
