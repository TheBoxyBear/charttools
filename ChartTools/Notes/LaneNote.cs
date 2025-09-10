using System.Runtime.CompilerServices;

namespace ChartTools;

public class LaneNote<TLane>(TLane lane) : ILaneNote<TLane>
	where TLane : struct, Enum
{
    public LaneNote() : this(default) { }

    public TLane Lane
    {
        get => lane;
        set => lane = value;
    }

	public byte Index
    {
        get => Unsafe.As<TLane, byte>(ref lane);
        set => lane = Unsafe.As<byte, TLane>(ref value);
    }

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
