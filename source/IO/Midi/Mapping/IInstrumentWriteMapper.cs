using ChartTools.IO.Configuration.Sessions;

namespace ChartTools.IO.Midi.Mapping;

internal interface IInstrumentWriteMapper<TChord> where TChord : IChord, new()
{
    public WritingSession WritingSession { get; }
    public IEnumerable<NoteMapping> Map(Instrument<TChord> instrument);
}
