namespace ChartTools.Animations;

public class HandPositionEvent : AnimationEvent
{
    public byte Index { get; }

    public HandPositionEvent() : base() { }
    public HandPositionEvent(uint position, byte index) : base(position) => Index = index;
}
