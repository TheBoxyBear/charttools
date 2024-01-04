using ChartTools.IO.Midi.Configuration.Sessions;

namespace ChartTools.IO.Midi.Mapping;

internal abstract class LaneInstrumentMapper<TChord> : InstrumentMapper<TChord>, ILaneInstrumentReadMapper where TChord : IChord, new()
{
    public virtual byte BigRockCount { get; }

    protected LaneInstrumentMapper(MidiReadingSession session) : base(session) { }
    protected LaneInstrumentMapper(MidiWritingSession session) : base(session) { }
}
