using ChartTools.IO.Chart.Configuration.Sessions;
using ChartTools.IO.Chart.Entries;

using System.Numerics;

namespace ChartTools;

public abstract class Chord(uint position) : ITrackObject
{
	public uint Position { get; set; } = position;

	public abstract ILaneNoteCollection Notes { get; }

	public abstract bool OpenExclusivity { get; }

	internal abstract bool ChartSupportedModifiers { get; }

	internal abstract IEnumerable<TrackObjectEntry> GetChartNoteData();
	internal abstract IEnumerable<TrackObjectEntry> GetChartModifierData(Chord? previous, ChartWritingSession session);
}

public abstract class Chord<TNote, TLane, TModifiers> : Chord
	where TNote : ILaneNote<TLane>, new()
	where TLane : Enum
	where TModifiers : struct, Enum
{
	public override LaneNoteCollection<TNote, TLane> Notes { get; }

	public TModifiers Modifiers { get; set; }

	internal abstract TModifiers DefaultModifiers { get; }

	public Chord(uint position) : base(position)
		=> Notes = new(OpenExclusivity);
}
