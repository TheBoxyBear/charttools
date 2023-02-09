using ChartTools.IO.Configuration.Sessions;

using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Midi.Mapping;

internal interface IReadMapper
{
    public ReadingSession ReadingSession { get; }
    public IEnumerable<NoteEventMapping> Map(uint position, NoteEvent e);
}
