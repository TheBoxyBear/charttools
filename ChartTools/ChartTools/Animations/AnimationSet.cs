using ChartTools.Extensions.Linq;
using System.Collections;

namespace ChartTools.Animations;

public class AnimationSet : IEnumerable<AnimationTrack>
{
    public HandPositionAnimationTrack Guitar
    {
        get => m_guitar;
        set => m_guitar = value with { Identity = HandPositionAnimationTrackIdentity.Guitar };
    }

    private HandPositionAnimationTrack m_guitar = new(HandPositionAnimationTrackIdentity.Guitar);

    public VocalsAnimationTrack Vocals { get; set; } = [];

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
		m_guitar = track.Identity switch
		{
			HandPositionAnimationTrackIdentity.Guitar => track,
			_ => throw new UndefinedEnumException(track.Identity),
		};
	}

    public IEnumerator<AnimationTrack> GetEnumerator()
		=> new AnimationTrack?[] { Guitar, Vocals }.NonNull().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
