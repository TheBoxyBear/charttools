namespace ChartTools.Animations;

public enum HandPositionAnimationTrackIdentity : byte { Guitar }

public record HandPositionAnimationTrack : AnimationTrack<HandPositionEvent>
{
    public new HandPositionAnimationTrackIdentity Identity { get; init; }
    public override AnimationType AnimationType => AnimationType.HandPosition;

    public HandPositionAnimationTrack(HandPositionAnimationTrackIdentity identity) : base() => Validator.ValidateEnum(identity);
    public HandPositionAnimationTrack(HandPositionAnimationTrackIdentity identity, IEnumerable<HandPositionEvent> items) : base(items) => Validator.ValidateEnum(identity);

    protected override AnimationTrackIdentity GetIdentity() => (AnimationTrackIdentity)Identity;
}
