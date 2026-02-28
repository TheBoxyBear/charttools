namespace ChartTools.Animations;

public record VocalsAnimationTrack : AnimationTrack<VocalistMouthEvent>
{
    public override AnimationType AnimationType => AnimationType.Vocals;

    public VocalsAnimationTrack() : base() { }
    public VocalsAnimationTrack(IEnumerable<VocalistMouthEvent> items) : base(items) { }

    protected override AnimationTrackIdentity GetIdentity() => AnimationTrackIdentity.Vocals;
}
