using ChartTools.IO.Configuration.Sessions;

namespace ChartTools.IO.Midi.Mapping;

internal abstract class StandardInstrumentMapper : LaneInstrumentMapper<StandardChord>
{
    public abstract MidiInstrumentOrigin Format { get; }

    protected StandardInstrumentMapper(ReadingSession session) : base(session) { }
    protected StandardInstrumentMapper(WritingSession session) : base(session) { }
}
