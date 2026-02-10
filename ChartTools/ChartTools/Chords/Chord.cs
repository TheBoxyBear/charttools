using ChartTools.IO.Chart.Configuration.Sessions;
using ChartTools.IO.Chart.Entries;

namespace ChartTools;

public abstract class Chord(uint position) : ITrackObject
{
	public uint Position { get; set; } = position;

#if NET5_0_OR_GREATER
	public abstract ILaneNoteCollection Notes { get; }
#else
	public ILaneNoteCollection Notes
		=> GetNotes();

	protected abstract ILaneNoteCollection GetNotes();
#endif

	internal abstract bool ChartSupportedModifiers { get; }

	internal abstract IEnumerable<TrackObjectEntry> GetChartNoteData();

	internal abstract IEnumerable<TrackObjectEntry> GetChartModifierData(Chord? previous, ChartWritingSession session);
}

public abstract class Chord<TNote, TLane>(uint position)
	: Chord(position)
	where TNote : struct, IDefinedLaneNote<TLane>
	where TLane : struct, Enum
{
#if NET5_0_OR_GREATER
	public override LaneNoteCollection<TNote, TLane> Notes { get; } = [];
#else
	public new LaneNoteCollection<TNote, TLane> Notes { get; } = [];

	protected override ILaneNoteCollection GetNotes()
		=> Notes;
#endif
}

public abstract class Chord<TNote, TLane, TModifiers>(uint position)
	: Chord<TNote, TLane>(position)
	where TNote : struct, IDefinedLaneNote<TLane>
	where TLane : struct, Enum
	where TModifiers : Enum
{
	public TModifiers Modifiers { get; set; }

	internal abstract TModifiers DefaultModifiers { get; }
}
