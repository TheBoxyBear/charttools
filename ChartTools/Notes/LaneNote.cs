using System.Runtime.CompilerServices;

namespace ChartTools;

public struct LaneNote<TLane> : ILaneNote<TLane>
	where TLane : struct, Enum
{
	private TLane m_lane;

	public TLane Lane
	{
		readonly get => m_lane;
		set => m_lane = value;
	}

	public readonly byte Index => Unsafe.As<TLane, byte>(ref Unsafe.AsRef(in m_lane));

	/// <summary>
	/// Maximum length the note can be held for extra points
	/// </summary>
	public uint Sustain { readonly get; set; }

	public LaneNote(TLane lane) => Lane = lane;

	uint ILongObject.Length
	{
		readonly get => Sustain;
		set => Sustain = value;
	}
}
