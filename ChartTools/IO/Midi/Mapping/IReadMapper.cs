using ChartTools.IO.Midi.Configuration.Sessions;
using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Midi.Mapping;

internal interface IReadMapper
{
    public MidiReadingSession ReadingSession { get; }
    public IEnumerable<NoteEventMapping> Map(uint position, NoteEvent e);
}
