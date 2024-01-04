namespace ChartTools.Animations;

public abstract class AnimationEvent : ITrackObject
{
    public uint Position { get; set; }

    public AnimationEvent() { }
    public AnimationEvent(uint position) => Position = position;
}
