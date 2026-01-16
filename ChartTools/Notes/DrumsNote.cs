namespace ChartTools;

/// <summary>
/// Note played by drums
/// </summary>
public readonly record struct DrumsNote(DrumsLane lane) : IDefinedLaneNote<DrumsLane>
{
	public static bool OpenExclusivity => false;
	public static byte MaxLanes => 6;

	private readonly bool m_isCymbal = false;

	/// <summary>
	/// <see langword="true"/> if the cymbal must be hit instead of the pad on supported drum sets
	/// </summary>
	/// <remarks><see cref="DrumsLane.Green5Lane"/> notes cannot be cymbal.</remarks>
	public bool IsCymbal
	{
		get => m_isCymbal;
		init
		{
			if ((Lane == DrumsLane.Red || Lane == DrumsLane.Green5Lane) && value)
				throw new InvalidOperationException("Red and 5-lane green notes cannot be cymbal.");

			m_isCymbal = value;
		}
	}

	/// <summary>
	/// Determines if the note is played by kicking
	/// </summary>
	public bool IsKick => Lane is DrumsLane.Kick or DrumsLane.DoubleKick;

	public uint Sustain { get; init; }

	public DrumsLane Lane { get; init; } = lane;

	public byte Index => (byte)Lane;
}
