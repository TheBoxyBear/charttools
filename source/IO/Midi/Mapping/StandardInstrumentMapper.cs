namespace ChartTools.IO.Midi.Mapping;

internal abstract class StandardInstrumentMapper : LaneInstrumentMapper<StandardChord>
{
    public abstract MidiInstrumentOrigin Format { get; }
}
