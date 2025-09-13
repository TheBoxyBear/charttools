namespace ChartTools;

/// <summary>
/// Interface for notes defined by lanes as part of a <see cref="LaneChord"/>
/// </summary>
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

/// <summary>
/// Interface for notes defined by lanes as part of a <see cref="LaneChord"/> where the lane is defined as an <see cref="Enum"/>
/// </summary>
/// <typeparam name="TLane">Enum type to use as lane</typeparam>
public interface ILaneNote<TLane> : ILaneNote
	where TLane : struct, Enum
{
    /// <summary>
    /// Enum value of the lane matching the value of <see cref="INote.Index"/>
    /// </summary>
    public TLane Lane { get; set; }
}
