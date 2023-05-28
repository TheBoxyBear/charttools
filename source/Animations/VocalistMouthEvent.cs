namespace ChartTools.Animations;

public enum VocalistMouthState : byte { Open, Close }

public class VocalistMouthEvent : AnimationEvent
{
    public VocalistMouthState State { get; set; }

    public VocalistMouthEvent(uint position, VocalistMouthState state) : base(position) => State = state;
}
