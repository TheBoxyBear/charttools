namespace ChartTools.Animations;

public enum VocalistMouthState : byte { Open, Close }

public class VocalistMouthEvent : AnimationEvent
{
    public VocalistMouthState State { get; set; }
}
