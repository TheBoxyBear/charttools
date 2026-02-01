global using StandardNote = ChartTools.LaneNote<ChartTools.StandardLane>;
global using GHLNote = ChartTools.LaneNote<ChartTools.GHLLane>;

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

	public LaneNote(TLane lane)
		=> Lane = lane;

	public byte Index => Convert.ToByte(Lane);
}
