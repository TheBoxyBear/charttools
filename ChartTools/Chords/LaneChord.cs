using ChartTools.IO.Chart.Configuration.Sessions;
using ChartTools.IO.Chart.Entries;

using System.Runtime.CompilerServices;

namespace ChartTools;

public abstract class LaneChord(uint position) : IChord
{
	public uint Position { get; set; } = position;

	public IReadOnlyList<ILaneNote> Notes => GetNotes();
	IReadOnlyList<INote> IChord.Notes => Notes;

	public abstract bool OpenExclusivity { get; }

	internal abstract bool ChartSupportedModifiers { get; }

	public abstract ILaneNote CreateNote(byte index, uint sustain);
	INote IChord.CreateNote(byte index, uint length) => CreateNote(index, length);

	protected abstract IReadOnlyList<ILaneNote> GetNotes();

	internal abstract IEnumerable<TrackObjectEntry> GetChartNoteData();
	internal abstract IEnumerable<TrackObjectEntry> GetChartModifierData(LaneChord? previous, ChartWritingSession session);
}

public abstract class LaneChord<TNote, TLane, TModifiers> : LaneChord
	where TNote : struct, ILaneNote<TLane>
	where TLane : struct, Enum
	where TModifiers : struct, Enum
{
	public new LaneNoteCollection<TNote, TLane> Notes { get; }

	public TModifiers Modifiers { get; set; }

	internal abstract TModifiers DefaultModifiers { get; }

	public LaneChord(uint position) : base(position)
		=> Notes = new(OpenExclusivity);

	protected override IReadOnlyList<ILaneNote> GetNotes() => (IReadOnlyList<ILaneNote>)Notes;

	public override ILaneNote CreateNote(byte index, uint sustain)
	{
		TNote note = new()
		{
			Lane = Unsafe.As<byte, TLane>(ref index),
			Sustain = sustain
		};

		Notes.Add(in note);
		return note;
	}
}
