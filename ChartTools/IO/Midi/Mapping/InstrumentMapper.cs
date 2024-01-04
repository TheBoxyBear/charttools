using ChartTools.IO.Midi.Configuration.Sessions;
using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Midi.Mapping;

internal abstract class InstrumentMapper<TChord> : IReadMapper, IInstrumentWriteMapper<TChord> where TChord : IChord, new()
{
    public MidiReadingSession ReadingSession { get; }
    public MidiWritingSession WritingSession { get; }

    protected InstrumentMapper(MidiReadingSession session) : this(session, new(MidiFile.DefaultWriteConfig, new())) { }
    protected InstrumentMapper(MidiWritingSession session) : this(new(MidiFile.DefaultReadConfig, new()), session) { }
    private InstrumentMapper(MidiReadingSession readingSession, MidiWritingSession writingSession)
    {
        ReadingSession = readingSession;
        WritingSession = writingSession;
    }

    public abstract IEnumerable<NoteEventMapping> Map(uint position, NoteEvent e);
    public abstract IEnumerable<NoteMapping> Map(Instrument<TChord> instrument);

    protected void HandleInvalidMidiEvent(uint position, NoteEvent e) => ReadingSession.HandleInvalidMidiEvent(position, e);
    protected T? HandleInvalidMidiEvent<T>(uint position, NoteEvent e)
    {
        HandleInvalidMidiEvent(position, e);
        return default;
    }
}
