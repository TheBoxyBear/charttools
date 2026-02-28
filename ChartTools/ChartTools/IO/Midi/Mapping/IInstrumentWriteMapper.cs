using ChartTools.IO.Midi.Configuration.Sessions;

namespace ChartTools.IO.Midi.Mapping;

internal interface IInstrumentWriteMapper<TChord>
	where TChord : Chord, new()
{
	public MidiWritingSession WritingSession { get; }

	public IEnumerable<NoteMapping> Map(Instrument<TChord> instrument);
}
