namespace ChartTools;

/// <summary>
/// Note played by drums
/// </summary>
public readonly record struct DrumsNote : IDefinedLaneNote<DrumsLane>
{
	public static bool OpenExclusivity => false;

	public static byte MaxLanes => 6;

	/// <summary>
	/// <see langword="true"/> if the cymbal must be hit instead of the pad on supported drum sets
	/// </summary>
	/// <remarks><see cref="DrumsLane.Green5Lane"/> notes cannot be cymbal.</remarks>
	public bool IsCymbal
	{
		get;
		init
		{
			ValidateCymbal(Lane, value);
			field = value;
		}
	} = false;

	/// <summary>
	/// Determines if the note is played by kicking
	/// </summary>
	public bool IsKick => Lane is DrumsLane.Kick or DrumsLane.DoubleKick;

	public uint Sustain { get; init; }

	public DrumsLane Lane
	{
		get;
		init
		{
			Validator.ValidateEnum(value);
			ValidateCymbal(value, IsCymbal);

			field = value;
		}
	}

	public byte Index => (byte)Lane;

	public DrumsNote(DrumsLane lane)
		=> Lane = lane;

	private static void ValidateCymbal(DrumsLane lane, bool isCymbal)
	{
		if (lane is DrumsLane.Red or DrumsLane.Green5Lane && isCymbal)
			throw new InvalidOperationException("Red and 5-lane green notes cannot be cymbal.");
	}
}
