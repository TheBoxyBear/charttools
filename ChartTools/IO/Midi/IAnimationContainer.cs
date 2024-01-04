using ChartTools.Animations;

namespace ChartTools.IO.Midi;

internal interface IAnimationContainer
{
    public IEnumerable<AnimationEvent> AnimationEvents { get; }
}

internal interface IAnimationContainer<T> : IAnimationContainer where T : AnimationEvent
{
    public new IEnumerable<T> AnimationEvents { get; }
    IEnumerable<AnimationEvent> IAnimationContainer.AnimationEvents => AnimationEvents;
}
