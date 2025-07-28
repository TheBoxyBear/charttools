using ChartTools.IO.Chart;
using ChartTools.IO.Chart.Configuration.Sessions;
using ChartTools.IO.Chart.Entries;

namespace ChartTools;

/// <summary>
/// Set of notes played simultaneously by a Guitar Hero Live instrument
/// </summary>
public sealed class GHLChord : LaneChord<LaneNote<GHLLane>, GHLLane, GHLChordModifiers>
{
	public override bool OpenExclusivity => true;

	internal override GHLChordModifiers DefaultModifiers => GHLChordModifiers.None;

	internal override bool ChartSupportedModifiers => !Modifiers.HasFlag(GHLChordModifiers.ExplicitHopo);

	public GHLChord() : base(0) { }

	/// <inheritdoc cref="LaneChord{TNote, TLane, TModifier}(uint)"/>
	public GHLChord(uint position) : base(position) { }

	/// <inheritdoc cref="GHLChord(uint)"/>
	/// <param name="notes">Notes to add</param>
	public GHLChord(uint position, params ReadOnlySpan<LaneNote<GHLLane>> notes) : base(position)
		=> Notes.AddRange(notes);

	/// <inheritdoc cref="GHLChord(uint, ReadOnlySpan{LaneNote{GHLLane}})"/>
	public GHLChord(uint position, params ReadOnlySpan<GHLLane> notes) : base(position)
		=> Notes.AddRange(notes);

	protected override IReadOnlyList<ILaneNote<GHLLane>> GetNotes()
		=> (IReadOnlyList<ILaneNote<GHLLane>>)Notes;

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

	internal override IEnumerable<TrackObjectEntry> GetChartModifierData(LaneChord? previous, ChartWritingSession session)
	{
		bool isInvert = Modifiers.HasFlag(GHLChordModifiers.HopoInvert);

		if (Modifiers.HasFlag(GHLChordModifiers.ExplicitHopo) && (previous is null || previous.Position <= session.Formatting.TrueHopoFrequency) != isInvert || isInvert)
			yield return ChartFormatting.NoteEntry(Position, 5, 0);
		if (Modifiers.HasFlag(GHLChordModifiers.Tap))
			yield return ChartFormatting.NoteEntry(Position, 6, 0);
	}
}
