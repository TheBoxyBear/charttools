using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Midi.Mapping;

internal readonly struct NoteMapping : IMidiEventMapping
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

    public MidiEvent ToMidiEvent(uint delta)
    {
        var e = State switch
        {
            NoteState.Open => new NoteOnEvent(),
            NoteState.Close => new NoteOnEvent(),
            _ => throw new UndefinedEnumException(State)
        };

        e.DeltaTime = delta;
        e.NoteNumber = NoteNumber;

        return e;
    }
}
