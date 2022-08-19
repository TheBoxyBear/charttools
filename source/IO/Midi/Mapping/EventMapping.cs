using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Midi.Mapping
{
    internal struct EventMapping : IMidiEventMapping<TextEvent>
    {
        public uint Position { get; }
        public string Text { get; }

        public TextEvent ToMidiEvent(uint delta) => new(Text) { DeltaTime = delta };
    }
}
