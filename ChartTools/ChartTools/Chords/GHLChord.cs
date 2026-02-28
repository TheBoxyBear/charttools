using ChartTools.Extensions.Enums;
using ChartTools.IO.Chart;
using ChartTools.IO.Chart.Configuration.Sessions;
using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Formatting;

namespace ChartTools;

/// <summary>
/// Set of notes played simultaneously by a Guitar Hero Live instrument
/// </summary>
public sealed class GHLChord : Chord<GHLNote, GHLLane, GHLChordModifiers>
{
	internal override GHLChordModifiers DefaultModifiers => GHLChordModifiers.None;

	internal override bool ChartSupportedModifiers => !Modifiers.HasFlag(GHLChordModifiers.ExplicitHopo);

	/// <summary>
	/// Creates an instance of <see cref="GHLChord"/> at position 0.
	/// </summary>
	public GHLChord() : base(0) { }

	/// <summary>
	/// Creates an instance of <see cref="GHLChord"/> at the specified position.
	/// </summary>
	/// <param name="position">Position of the chord</param>
	public GHLChord(uint position) : base(position) { }

	/// <summary>
	/// Creates an instance of <see cref="GHLChord"/> with a specified position and notes.
	/// </summary>
	/// <param name="position">Position of the chord</param>
	/// <param name="notes">Set of notes to add</param>
	public GHLChord(uint position, params ReadOnlySpan<GHLNote> notes) : base(position)
		=> Notes.AddRange(notes);

	/// <summary>
	/// Creates an instance of <see cref="GHLChord"/> with a specified position and notes.
	/// </summary>
	/// <param name="position">Position of the chord</param>
	/// <param name="notes">Set of notes to add by lane</param>
	public GHLChord(uint position, params ReadOnlySpan<SafeEnum<GHLLane>> notes) : base(position)
		=> Notes.AddRange(notes);

	internal override IEnumerable<TrackObjectEntry> GetChartNoteData()
		=> Notes.Select(note => ChartFormatting.NoteEntry(Position, note.Lane switch
	{
		GHLLane.Open   => 7,
		GHLLane.Black1 => 3,
		GHLLane.Black2 => 4,
		GHLLane.Black3 => 8,
		GHLLane.White1 => 0,
		GHLLane.White2 => 1,
		GHLLane.White3 => 2,
	}, note.Sustain));

	internal override IEnumerable<TrackObjectEntry> GetChartModifierData(Chord? previous, ChartWritingSession session)
	{
		bool isInvert = Modifiers.HasFlag(GHLChordModifiers.HopoInvert);

		if (Modifiers.HasFlag(GHLChordModifiers.ExplicitHopo) && (previous is null || previous.Position <= session.Formatting.ChartHopoFrequency) != isInvert || isInvert)
			yield return ChartFormatting.NoteEntry(Position, 5, 0);
		if (Modifiers.HasFlag(GHLChordModifiers.Tap))
			yield return ChartFormatting.NoteEntry(Position, 6, 0);
	}
}
