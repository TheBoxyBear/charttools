using ChartTools.IO.Configuration.Sessions;
using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Midi.Mapping;

internal abstract class InstrumentMapper<TChord> where TChord : IChord, new()
{
    public ReadingSession? ReadingSession { get; }
    public WritingSession? WritingSession { get; }

    public InstrumentMapper(ReadingSession? readingSession = null, WritingSession? writingSession = null)
    {
        ReadingSession = readingSession;
        WritingSession = writingSession;
    }

    public abstract IEnumerable<NoteEventMapping> Map(uint position, NoteEvent e);
    public abstract IEnumerable<NoteMapping> Map(Instrument<TChord> instrument);

    protected void HandleInvalidMidiEvent(uint position, NoteEvent e) => (ReadingSession ?? throw new NullReferenceException("Reading session is null")).InvalidMidiEventTypeProcedure(position, e);
    protected T? HandleInvalidMidiEvent<T>(uint position, NoteEvent e)
    {
        HandleInvalidMidiEvent(position, e);
        return default;
    }
}
