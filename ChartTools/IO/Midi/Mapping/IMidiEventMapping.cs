using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Midi.Mapping;

internal interface IMidiEventMapping : IReadOnlyTrackObject
{
    public MidiEvent ToMidiEvent(uint delta);
}
