namespace ChartTools.Animations;

public enum VocalistMouthState : byte { Open, Close }

public class VocalistMouthControl : ITrackObject
{
    public uint Position { get; set; }
    public VocalistMouthState State { get; set; }
}
