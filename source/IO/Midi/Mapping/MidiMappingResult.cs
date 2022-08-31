namespace ChartTools.IO.Midi.Mapping
{
    internal enum MappingType : byte { Note, Modifier, Special, Animation, BigRock }
    internal enum NoteState : byte { Open, Close }

    internal struct MidiMappingResult
    {
        public uint Position { get; }
        public NoteState State { get; }
        public Difficulty? Difficulty { get; }
        public MappingType Type { get; }
        public byte Index { get; }

        public MidiMappingResult(uint position, NoteState state, Difficulty? difficulty, MappingType type, byte index)
        {
            Position = position;
            State = state;
            Difficulty = difficulty;
            Type = type;
            Index = index;
        }
    }
}
