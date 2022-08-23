using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Midi.Mapping
{
    internal enum MappingType : byte { Note, Modifier, Special, Animation, BigRock }

    internal readonly struct NoteEventMapping
    {
        public uint Position { get; }
        public NoteState State { get; }
        public Difficulty? Difficulty { get; }
        public MappingType Type { get; }
        public byte Index { get; }

        public NoteEventMapping(GlobalNoteEvent e, Difficulty? difficulty, MappingType type, byte index) : this(e.Position, GetState(e.Event), difficulty, type, index) { }
        public NoteEventMapping(uint position, NoteState state, Difficulty? difficulty, MappingType type, byte index)
        {
            Position = position;
            State = state;
            Difficulty = difficulty;
            Type = type;
            Index = index;
        }

        public static NoteState GetState(NoteEvent note) => note is NoteOnEvent ? NoteState.Open : NoteState.Close;
    }
}
