using ChartTools.IO.Chart.Configuration.Sessions;
using ChartTools.IO.Chart.Entries;

namespace ChartTools;

public abstract class Chord(uint position) : ITrackObject
{
	public uint Position { get; set; } = position;

	public abstract ILaneNoteCollection Notes { get; }

	internal abstract bool ChartSupportedModifiers { get; }

	internal abstract IEnumerable<TrackObjectEntry> GetChartNoteData();

	internal abstract IEnumerable<TrackObjectEntry> GetChartModifierData(Chord? previous, ChartWritingSession session);
}

public abstract class Chord<TNote, TLane> : Chord
	where TNote : struct, IDefinedLaneNote<TLane>
	where TLane : struct, Enum
{
	public override LaneNoteCollection<TNote, TLane> Notes { get; }

	public Chord(uint position) : base(position)
		=> Notes = [];
}

public abstract class Chord<TNote, TLane, TModifiers> : Chord<TNote, TLane>
	where TNote : struct, IDefinedLaneNote<TLane>
	where TLane : struct, Enum
	where TModifiers : Enum
{
	public TModifiers Modifiers { get; set; }

	internal abstract TModifiers DefaultModifiers { get; }

	public Chord(uint position)
		: base(position) { }
}
