global using StandardNote = ChartTools.LaneNote<ChartTools.StandardLane>;
global using GHLNote = ChartTools.LaneNote<ChartTools.GHLLane>;

namespace ChartTools;

public readonly record struct LaneNote<TLane>(TLane lane) : ILaneNote<TLane>
	where TLane : Enum
{
	public uint Sustain { get; init; }

	public TLane Lane { get; init; } = lane;

	public byte Index => Convert.ToByte(Lane);
}
