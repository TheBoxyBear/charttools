using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Midi.Mapping
{
    internal interface IMidiEventMapping<out T> : IReadOnlyTrackObject where T : MidiEvent
    {
        public T ToMidiEvent(uint delta);
    }
}
