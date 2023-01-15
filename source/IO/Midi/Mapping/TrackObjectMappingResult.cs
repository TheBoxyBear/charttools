namespace ChartTools.IO.Midi.Mapping;

internal readonly struct TrackObjectMappingResult : IReadOnlyTrackObject
{
    public uint Position { get; }
    public Difficulty Difficulty { get; }
    public byte NoteNumber { get; }

    public TrackObjectMappingResult(uint position, Difficulty difficulty, byte noteNumber)
    {
        Position = position;
        Difficulty = difficulty;
        NoteNumber = noteNumber;
    }
}
