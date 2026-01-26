using ChartTools.IO.Chart;
using ChartTools.IO.Chart.Configuration.Sessions;
using ChartTools.IO.Chart.Entries;

namespace ChartTools;

/// <summary>
/// Set of notes played simultaneously by a standard five-fret instrument
/// </summary>
public class StandardChord : Chord<StandardNote, StandardLane, StandardChordModifiers>
{
	internal override StandardChordModifiers DefaultModifiers => StandardChordModifiers.None;

	internal override bool ChartSupportedModifiers => !Modifiers.HasFlag(StandardChordModifiers.ExplicitHopo);

	public StandardChord() : base(0) { }

	public StandardChord(uint position) : base(position) { }

	public StandardChord(uint position, params ReadOnlySpan<StandardNote> notes) : this(position)
		=> Notes.AddRange(notes);

	public StandardChord(uint position, params ReadOnlySpan<StandardLane> notes) : this(position)
		=> Notes.AddRange(notes);

	internal override IEnumerable<TrackObjectEntry> GetChartNoteData()
		=> Notes.Select(note => ChartFormatting.NoteEntry(Position, note.Lane == StandardLane.Open ? (byte)7 : (byte)(note.Lane - 1), note.Sustain));

	internal override IEnumerable<TrackObjectEntry> GetChartModifierData(Chord? previous, ChartWritingSession session)
	{
		bool isInvert = Modifiers.HasFlag(StandardChordModifiers.HopoInvert);

		if (Modifiers.HasFlag(StandardChordModifiers.ExplicitHopo) &&
			(previous is null || previous.Position <= session.Formatting.ChartHopoFrequency) != isInvert || isInvert)
			yield return ChartFormatting.NoteEntry(Position, 5, 0);
		if (Modifiers.HasFlag(StandardChordModifiers.Tap))
			yield return ChartFormatting.NoteEntry(Position, 6, 0);
	}
}
