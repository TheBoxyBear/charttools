namespace ChartTools.Animations;

public abstract class AnimationEvent : ITrackObject
{
    public uint Position { get; set; }

    public AnimationEvent() : this(0) { }
    public AnimationEvent(uint position) => Position = position;
}
