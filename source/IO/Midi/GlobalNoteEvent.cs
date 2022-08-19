using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Midi
{
    internal struct GlobalNoteEvent : IReadOnlyTrackObject
    {
        public uint Position { get; }
        public NoteEvent Event { get; }

        public GlobalNoteEvent(uint position, NoteEvent e)
        {
            Position = position;
            Event = e;
        }
    }
}
