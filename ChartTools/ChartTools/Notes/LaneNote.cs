global using StandardNote = ChartTools.LaneNote<ChartTools.StandardLane>;
global using GHLNote = ChartTools.LaneNote<ChartTools.GHLLane>;

using ChartTools.Extensions;
using ChartTools.Extensions.Enums;

namespace ChartTools;

public readonly record struct LaneNote<TLane> : IDefinedLaneNote<TLane>
	where TLane : struct, Enum
{
	public LaneNote(TLane lane)
		=> Lane = lane;

#if NET7_0_OR_GREATER
	public static bool OpenExclusivity => true;

	public static byte MaxLanes => 6;
#else
	public bool OpenExclusivity => true;

	public byte MaxLanes => 6;

	uint IReadOnlyLongObject.Length => Sustain;
#endif

	public uint Sustain { get; init; }

	public TLane Lane
	{
		get;
		init
		{
			Validator.ValidateEnum(value);
			field = value;
		}
	}

	public byte Index => Lane.As<TLane, byte>();
}
