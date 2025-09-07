using System.Runtime.CompilerServices;

namespace ChartTools;

/// <summary>
/// Note played by drums
/// </summary>
public struct DrumsNote(DrumsLane lane) : ILaneNote<DrumsLane>
{
	public DrumsLane Lane
	{
		readonly get => lane;
		set => lane = value;
	}

	public uint Sustain { readonly get; set; }

	public readonly byte Index => Unsafe.As<DrumsLane, byte>(ref Unsafe.AsRef(in lane));


	private bool m_isCymbal = false;

	/// <summary>
	/// <see langword="true"/> if the cymbal must be hit instead of the pad on supported drum sets
	/// </summary>
	/// <remarks><see cref="DrumsLane.Green5Lane"/> notes cannot be cymbal.</remarks>
	public bool IsCymbal
	{
		readonly get => m_isCymbal;
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
	public readonly bool IsKick => Lane is DrumsLane.Kick or DrumsLane.DoubleKick;
}
