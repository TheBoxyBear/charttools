using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Midi.Mapping;

internal enum MappingType : byte { Note, Modifier, Special, Animation, BigRock }

internal readonly struct NoteEventMapping : INoteMapping
{
    public uint Position { get; }
    public SevenBitNumber NoteNumber { get; }
    public NoteState State { get; }
    public Difficulty? Difficulty { get; }
    public MappingType Type { get; }
    public byte Index { get; }

    public NoteEventMapping(uint position, NoteEvent e, Difficulty? difficulty, MappingType type, byte index) : this(position, GetState(e), difficulty, type, index, e.NoteNumber) { }
    public NoteEventMapping(uint position, NoteState state, Difficulty? difficulty, MappingType type, byte index, SevenBitNumber noteNumber)
    {
        Position = position;
        State = state;
        Difficulty = difficulty;
        Type = type;
        Index = index;
        NoteNumber = noteNumber;
    }

    public static NoteState GetState(NoteEvent note) => note is NoteOnEvent ? NoteState.Open : NoteState.Close;
}
