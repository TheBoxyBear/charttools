using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Midi.Mapping
{
    internal struct NoteMapping : IMidiEventMapping<NoteEvent>
    {
        public uint Position { get; }
        public Difficulty Difficulty { get; }
        public NoteState State { get; }
        public byte NoteNumber { get; }

        public NoteMapping(uint position, Difficulty difficulty, NoteState state, byte noteNumber)
        {
            Position = position;
            Difficulty = difficulty;
            State = state;
            NoteNumber = noteNumber;
        }

        public NoteEvent ToMidiEvent(uint delta)
        {
            var e = State switch
            {
                NoteState.Open => new NoteOnEvent(),
                NoteState.Close => new NoteOnEvent(),
                _ => throw new UndefinedEnumException(State)
            };

            e.DeltaTime = delta;
            e.NoteNumber = new(NoteNumber);

            return e;
        }
    }
}
