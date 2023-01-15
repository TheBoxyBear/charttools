using ChartTools.Extensions;
using ChartTools.Extensions.Collections;

namespace ChartTools;

/// <summary>
/// Set of track objects where each one must have a different position
/// </summary>
public class UniqueTrackObjectCollection<T> : UniqueList<T> where T : ITrackObject
{
    static readonly EqualityComparison<T> comparison = (a, b) => a is null || b is null || a.Position == b.Position;

    public UniqueTrackObjectCollection(int capacity = 0, IEnumerable<T>? items = null) : base(comparison, capacity, items) { }
}
