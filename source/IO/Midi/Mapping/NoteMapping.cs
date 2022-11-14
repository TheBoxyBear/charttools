using Melanchall.DryWetMidi.Common;

namespace ChartTools.IO.Midi.Mapping;

internal readonly struct NoteMapping : INoteMapping
{
    public uint Position { get; }
    public SevenBitNumber NoteNumber { get; }
    public Difficulty? Difficulty { get; }
    public NoteState State { get; }

    public NoteMapping(uint position, Difficulty? difficulty, NoteState state, SevenBitNumber noteNumber)
    {
        Position = position;
        Difficulty = difficulty;
        State = state;
        NoteNumber = noteNumber;
    }
}
