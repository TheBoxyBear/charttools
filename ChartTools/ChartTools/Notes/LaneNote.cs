global using StandardNote = ChartTools.LaneNote<ChartTools.StandardLane>;
global using GHLNote = ChartTools.LaneNote<ChartTools.GHLLane>;

using ChartTools.Extensions;

namespace ChartTools;

public readonly record struct LaneNote<TLane> : IDefinedLaneNote<TLane>
	where TLane : struct, Enum
{
	public static bool OpenExclusivity => true;

	public static byte MaxLanes => 6;

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
