using ChartTools.IO.Midi.Configuration.Sessions;

namespace ChartTools.IO.Midi.Mapping;

internal interface IInstrumentWriteMapper<TChord> where TChord : IChord, new()
{
    public MidiWritingSession WritingSession { get; }
    public IEnumerable<NoteMapping> Map(Instrument<TChord> instrument);
}
