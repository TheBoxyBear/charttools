using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Midi.Mapping;

internal interface IReadInstrumentMapper
{
    public IEnumerable<NoteEventMapping> Map(uint position, NoteEvent e);
}
