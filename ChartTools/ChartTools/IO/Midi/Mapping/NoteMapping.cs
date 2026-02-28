using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Midi.Mapping;

internal readonly struct NoteMapping(uint position, NoteState state, SevenBitNumber noteNumber)
	: IMidiEventMapping
{
	public uint Position { get; } = position;

	public SevenBitNumber NoteNumber { get; } = noteNumber;

	public NoteState State { get; } = state;

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
