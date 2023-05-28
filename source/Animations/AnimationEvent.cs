namespace ChartTools.Animations;

public class AnimationEvent : TrackObjectBase
{
    public AnimationEvent() : this(0) { }
    public AnimationEvent(uint position) => Position = position;
}
