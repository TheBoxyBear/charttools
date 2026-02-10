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

public interface IDefinedLaneNote<TLane> : ILaneNote<TLane>
	where TLane : Enum
{
#if NET7_0_OR_GREATER
	public static abstract bool OpenExclusivity { get; }

	public static abstract byte MaxLanes { get; }
#else
	public bool OpenExclusivity { get; }

	public byte MaxLanes { get; }
#endif
}
