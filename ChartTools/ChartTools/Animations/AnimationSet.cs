using ChartTools.Extensions.Linq;
using System.Collections;

namespace ChartTools.Animations;

public class AnimationSet : IEnumerable<AnimationTrack>
{
    public HandPositionAnimationTrack Guitar
    {
        get => _guitar;
        set => _guitar = value with { Identity = HandPositionAnimationTrackIdentity.Guitar };
    }
    private HandPositionAnimationTrack _guitar = new(HandPositionAnimationTrackIdentity.Guitar);

    public VocalsAnimationTrack Vocals { get; set; } = new();

    public AnimationTrack Get(AnimationTrackIdentity identity) => identity switch
    {
        AnimationTrackIdentity.Guitar => Guitar,
        AnimationTrackIdentity.Vocals => Vocals,
        _ => throw new UndefinedEnumException(identity)
    };
    public HandPositionAnimationTrack Get(HandPositionAnimationTrackIdentity identity) => identity switch
    {
        HandPositionAnimationTrackIdentity.Guitar => Guitar,
        _ => throw new UndefinedEnumException(identity)
    };

    public void Set(HandPositionAnimationTrack track)
    {
        switch (track.Identity)
        {
            case HandPositionAnimationTrackIdentity.Guitar:
                _guitar = track;
                break;
            default:
                throw new UndefinedEnumException(track.Identity);
        }
    }

    public IEnumerator<AnimationTrack> GetEnumerator() => new AnimationTrack?[] { Guitar, Vocals }.NonNull().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
