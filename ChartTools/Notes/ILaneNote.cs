namespace ChartTools;

public interface ILaneNote : INote
{
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
