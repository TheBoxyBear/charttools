using ChartTools.IO.Chart;
using ChartTools.IO.Chart.Configuration.Sessions;
using ChartTools.IO.Chart.Entries;

namespace ChartTools;

/// <summary>
/// Set of notes played simultaneously by a standard five-fret instrument
/// </summary>
public sealed class StandardChord : LaneChord<LaneNote<StandardLane>, StandardLane, StandardChordModifiers>
{
	public override bool OpenExclusivity => true;

	internal override StandardChordModifiers DefaultModifiers => StandardChordModifiers.None;
	internal override bool ChartSupportedModifiers => !Modifiers.HasFlag(StandardChordModifiers.ExplicitHopo);

	public StandardChord() : base(0) { }

	/// <inheritdoc cref="LaneChord{DrumsNote, DrumsLane, DrumsChordModifiers}(uint)"/>
	public StandardChord(uint position) : base(position) { }

	/// <inheritdoc cref="LaneChord{TNote, TLane, TModifier}(uint)"/>
	/// <param name="notes">Notes to add</param>
	public StandardChord(uint position, params ReadOnlySpan<LaneNote<StandardLane>> notes) : this(position)
		=> Notes.AddRange(notes);

	/// <inheritdoc cref="StandardChord(uint, ReadOnlySpan{LaneNote{StandardLane}})"/>
	public StandardChord(uint position, params ReadOnlySpan<StandardLane> notes) : this(position)
		=> Notes.AddRange(notes);

	protected override IReadOnlyList<ILaneNote<StandardLane>> GetNotes()
		=> (IReadOnlyList<ILaneNote<StandardLane>>)Notes;

	internal override IEnumerable<TrackObjectEntry> GetChartNoteData()
		=> Notes.Select(note => ChartFormatting.NoteEntry(Position, note.Lane == StandardLane.Open ? (byte)7 : (byte)(note.Lane - 1), note.Sustain));

	internal override IEnumerable<TrackObjectEntry> GetChartModifierData(LaneChord? previous, ChartWritingSession session)
	{
		bool isInvert = Modifiers.HasFlag(StandardChordModifiers.HopoInvert);

		if (Modifiers.HasFlag(StandardChordModifiers.ExplicitHopo) &&
			(previous is null || previous.Position <= session.Formatting.TrueHopoFrequency) != isInvert || isInvert)
			yield return ChartFormatting.NoteEntry(Position, 5, 0);
		if (Modifiers.HasFlag(StandardChordModifiers.Tap))
			yield return ChartFormatting.NoteEntry(Position, 6, 0);
	}
}
