using System.Runtime.CompilerServices;

namespace ChartTools;

/// <summary>
/// Base class for notes
/// </summary>
public class LaneNote<TLane> : LaneNote where TLane : struct, Enum
{
    public override byte Index => Unsafe.As<TLane, byte>(ref _lane);

    public TLane Lane
    {
        get => _lane;
        init => _lane = value;
    }
    private TLane _lane;

    public LaneNote() { }

    public LaneNote(TLane lane, uint sustain = 0)
    {
        Lane = lane;
        Sustain = sustain;
    }
}
