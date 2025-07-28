namespace ChartTools;

public interface ILaneNote : INote
{
	public abstract byte Index { get; }

	/// <summary>
	/// Maximum length the note can be held for extra points
	/// </summary>
	public uint Sustain { get; set; }

	uint ILongObject.Length
	{
		get => Sustain;
		set => Sustain = value;
	}
}

public interface ILaneNote<TLane> : ILaneNote
	where TLane : struct, Enum
{
	public TLane Lane { get; set; }
}
