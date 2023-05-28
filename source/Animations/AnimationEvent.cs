namespace ChartTools.Animations;

public abstract class AnimationEvent : TrackObjectBase
{
    public AnimationEvent() : this(0) { }
    public AnimationEvent(uint position) => Position = position;
}
