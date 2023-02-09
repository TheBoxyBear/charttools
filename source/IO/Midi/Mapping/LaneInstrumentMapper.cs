namespace ChartTools.IO.Midi.Mapping;

internal abstract class LaneInstrumentMapper<TChord> : InstrumentMapper<TChord> where TChord : IChord, new()
{
    public virtual byte BigRockCount { get; }
}
