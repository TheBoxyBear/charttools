namespace ChartTools.IO.Midi.Mapping
{
    internal struct TrackObjectMapping : IReadOnlyTrackObject
    {
        public uint Position { get; }
        public Difficulty Difficulty { get; }
        public NoteState State { get; }
        public byte NoteNumber { get; }

        public TrackObjectMapping(uint position, Difficulty difficulty, NoteState state, byte noteNumber)
        {
            Position = position;
            Difficulty = difficulty;
            State = state;
            NoteNumber = noteNumber;
        }
    }
}
