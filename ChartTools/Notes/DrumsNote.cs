using System.Runtime.CompilerServices;

/// <summary>
/// Note played by drums
/// </summary>
public class DrumsNote(DrumsLane lane) : ILaneNote<DrumsLane>
{
    public DrumsNote() : this(default) { }

    public DrumsLane Lane { get; set; } = lane;

	public uint Sustain { get; set; }

	public byte Index
    {
        get => (byte)Lane;
        set => Lane = (DrumsLane)value;
    }

	private bool m_isCymbal = false;

	/// <summary>
	/// <see langword="true"/> if the cymbal must be hit instead of the pad on supported drum sets
	/// </summary>
	/// <remarks><see cref="DrumsLane.Green5Lane"/> notes cannot be cymbal.</remarks>
	public bool IsCymbal
	{
	    get => m_isCymbal;
		set
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
}
