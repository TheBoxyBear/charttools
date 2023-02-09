using ChartTools.IO.Configuration.Sessions;

namespace ChartTools.IO.Midi.Mapping;

internal abstract class LaneInstrumentMapper<TChord> : InstrumentMapper<TChord>, ILaneInstrumentReadMapper where TChord : IChord, new()
{
    public virtual byte BigRockCount { get; }

    protected LaneInstrumentMapper(ReadingSession session) : base(session) { }
    protected LaneInstrumentMapper(WritingSession session) : base(session) { }
}
