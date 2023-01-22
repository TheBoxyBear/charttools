using ChartTools.Extensions.Collections;

namespace ChartTools;

/// <summary>
/// Set of track objects where each one must have a different position
/// </summary>
public class UniqueTrackObjectCollection<T> : UniqueCollection<uint, T> where T : ITrackObject
{
    public UniqueTrackObjectCollection(IEnumerable<T>? items = null) : base(t => t.Position, items) { }
}
