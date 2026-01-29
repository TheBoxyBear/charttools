using ChartTools.IO.Chart.Configuration.Sessions;
using ChartTools.IO.Chart.Entries;

using System.Runtime.CompilerServices;

namespace ChartTools;

public abstract class Chord(uint position) : ITrackObject
{
	public uint Position { get; set; } = position;

	public abstract IReadOnlyList<LaneNote> Notes { get; }

	public abstract bool OpenExclusivity { get; }

	internal abstract bool ChartSupportedModifiers { get; }

	public abstract LaneNote CreateNote(byte index, uint length);

	internal abstract IEnumerable<TrackObjectEntry> GetChartNoteData();

	internal abstract IEnumerable<TrackObjectEntry> GetChartModifierData(Chord? previous, ChartWritingSession session);
}

public abstract class Chord<TNote, TLane, TModifiers> : Chord
	where TNote : LaneNote<TLane>, new()
	where TLane : struct, Enum
	where TModifiers : struct, Enum
{
	public override LaneNoteCollection<TNote, TLane> Notes { get; }

	public TModifiers Modifiers { get; set; }

	internal abstract TModifiers DefaultModifiers { get; }

	public Chord(uint position) : base(position)
		=> Notes = new(OpenExclusivity);

	public override LaneNote CreateNote(byte index, uint sustain)
	{
		TNote note = new()
		{
			Lane = Unsafe.As<byte, TLane>(ref index),
			Sustain = sustain
		};

		Notes.Add(note);
		return note;
	}
}
