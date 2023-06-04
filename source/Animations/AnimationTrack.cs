using System.Collections;

namespace ChartTools.Animations;

public enum AnimationTrackIdentity : byte { Guitar, Vocals }
public enum AnimationType : byte { HandPosition, Vocals }

public abstract record AnimationTrack : IReadOnlyList<AnimationEvent>, IEmptyVerifiable
{
    public AnimationTrackIdentity Identity => GetIdentity();
    public abstract AnimationType AnimationType { get; }
    public abstract int Count { get; }
    public bool IsEmpty => Count == 0;

    public AnimationEvent this[int index] => GetItems()[index];

    protected abstract AnimationTrackIdentity GetIdentity();
    protected abstract IReadOnlyList<AnimationEvent> GetItems();

    public abstract AnimationEvent Create(uint position);

    public IEnumerator<AnimationEvent> GetEnumerator() => GetItems().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetItems().GetEnumerator();
}
