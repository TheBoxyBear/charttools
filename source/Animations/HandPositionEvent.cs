namespace ChartTools.Animations;

public class HandPositionEvent : AnimationEvent
{
    public byte Index { get; }

    public HandPositionEvent(uint position, byte index)
    {
        Position = position;
        Index = index;
    }
}
