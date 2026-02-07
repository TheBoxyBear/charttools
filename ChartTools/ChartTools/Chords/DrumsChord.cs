using ChartTools.Extensions.Enums;
using ChartTools.IO.Chart;
using ChartTools.IO.Chart.Configuration.Sessions;
using ChartTools.IO.Chart.Entries;

namespace ChartTools;

/// <summary>
/// Set of notes played simultaneously by drums
/// </summary>
public sealed class DrumsChord : Chord<DrumsNote, DrumsLane, DrumsChordModifiers>
{
	internal override DrumsChordModifiers DefaultModifiers => DrumsChordModifiers.None;

	internal override bool ChartSupportedModifiers => true;

	public DrumsChord() : base(0) { }

	/// <inheritdoc cref="Chord{DrumsNote, DrumsLane, DrumsChordModifiers}(uint)"/>
	public DrumsChord(uint position) : base(position) { }

	/// <inheritdoc cref="DrumsChord(uint)"/>
	/// <param name="notes">Notes to add</param>
	public DrumsChord(uint position, params ReadOnlySpan<DrumsNote> notes) : base(position)
		=> Notes.AddRange(notes);

	/// <inheritdoc cref="DrumsChord(uint, ReadOnlySpan{DrumsNote})"/>
	public DrumsChord(uint position, params ReadOnlySpan<SafeEnum<DrumsLane>> notes) : base(position)
		=> Notes.AddRange(notes);

	internal override IEnumerable<TrackObjectEntry> GetChartNoteData()
	{
		foreach (DrumsNote note in Notes)
		{
			yield return ChartFormatting.NoteEntry(Position, note.Lane is DrumsLane.DoubleKick ? (byte)32 : note.Index, note.Sustain);

			if (note.IsCymbal)
				yield return ChartFormatting.NoteEntry(Position, (byte)(note.Lane + 64), 0);
		}
	}

	internal override IEnumerable<TrackObjectEntry> GetChartModifierData(Chord? previous, ChartWritingSession session)
	{
		if (Modifiers.HasFlag(DrumsChordModifiers.Flam))
			yield return ChartFormatting.NoteEntry(Position, 109, 0);
	}
}
