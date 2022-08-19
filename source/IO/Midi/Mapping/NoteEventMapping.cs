namespace ChartTools.IO.Midi.Mapping
{
    internal enum MappingType : byte { Note, Modifier, Special, Animation, BigRock }

    internal struct NoteEventMapping
    {
        public uint Position { get; }
        public NoteState State { get; }
        public Difficulty? Difficulty { get; }
        public MappingType Type { get; }
        public byte Index { get; }

        public NoteEventMapping(uint position, NoteState state, Difficulty? difficulty, MappingType type, byte index)
        {
            Position = position;
            State = state;
            Difficulty = difficulty;
            Type = type;
            Index = index;
        }
    }
}
