using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Midi.Mapping;

internal interface INoteMapping : IMidiEventMapping
{
    public SevenBitNumber NoteNumber { get; }
    public NoteState State { get; }

    MidiEvent IMidiEventMapping.ToMidiEvent(uint delta)
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
