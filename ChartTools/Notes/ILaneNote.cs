namespace ChartTools;

public interface ILaneNote : INote
{
	/// <summary>
	/// Maximum length the note can be held for extra points
	/// </summary>
	public uint Sustain { get; init; }

	uint IReadOnlyLongObject.Length => Sustain;
}

public interface ILaneNote<TLane> : ILaneNote
	where TLane : Enum
{
	public TLane Lane { get; init; }
}
