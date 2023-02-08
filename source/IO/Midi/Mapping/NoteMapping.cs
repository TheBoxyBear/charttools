using Melanchall.DryWetMidi.Common;

namespace ChartTools.IO.Midi.Mapping;

internal readonly struct NoteMapping : INoteMapping
{
    public uint Position { get; }
    public SevenBitNumber NoteNumber { get; }
    public NoteState State { get; }

    public NoteMapping(uint position, NoteState state, SevenBitNumber noteNumber)
    {
        Position = position;
        State = state;
        NoteNumber = noteNumber;
    }
}
