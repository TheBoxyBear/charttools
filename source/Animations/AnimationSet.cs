namespace ChartTools.Animations;

public class AnimationSet
{
    public List<HandPositionEvent> Guitar { get; set; } = new();
    public List<VocalistMouthEvent> Vocals { get; set; } = new();
}
