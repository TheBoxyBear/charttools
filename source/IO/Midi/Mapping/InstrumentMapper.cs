using ChartTools.IO.Configuration.Sessions;
using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Midi.Mapping;

internal abstract class InstrumentMapper<TChord> : IReadMapper, IInstrumentWriteMapper<TChord> where TChord : IChord, new()
{
    public ReadingSession ReadingSession { get; }
    public WritingSession WritingSession { get; }

    protected InstrumentMapper(ReadingSession session) : this(session, new(MidiFile.DefaultWriteConfig, new())) { }
    protected InstrumentMapper(WritingSession session) : this(new(MidiFile.DefaultReadConfig, new()), session) { }
    private InstrumentMapper(ReadingSession readingSession, WritingSession writingSession)
    {
        ReadingSession = readingSession;
        WritingSession = writingSession;
    }

    public abstract IEnumerable<NoteEventMapping> Map(uint position, NoteEvent e);
    public abstract IEnumerable<NoteMapping> Map(Instrument<TChord> instrument);

    protected void HandleInvalidMidiEvent(uint position, NoteEvent e) => ReadingSession.InvalidMidiEventTypeProcedure(position, e);
    protected T? HandleInvalidMidiEvent<T>(uint position, NoteEvent e)
    {
        HandleInvalidMidiEvent(position, e);
        return default;
    }
}
