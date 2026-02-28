using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Midi.Mapping;

internal enum MappingType : byte { Note, Modifier, Special, BigRock }

internal readonly struct NoteEventMapping(uint position, NoteState state, Difficulty? difficulty, MappingType type, byte index)
{
	public uint Position { get; } = position;

	public NoteState State { get; } = state;

	public Difficulty? Difficulty { get; } = difficulty;

	public MappingType Type { get; } = type;

	public byte Index { get; } = index;

	public NoteEventMapping(uint position, NoteEvent e, Difficulty? difficulty, MappingType type, byte index)
		: this(position, GetState(e), difficulty, type, index) { }

	public static NoteState GetState(NoteEvent note)
		=> note is NoteOnEvent ? NoteState.Open : NoteState.Close;
}
