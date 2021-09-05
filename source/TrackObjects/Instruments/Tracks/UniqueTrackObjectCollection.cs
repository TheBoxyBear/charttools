using System.Collections.Generic;

using ChartTools.Collections.Unique;
using ChartTools.SystemExtensions;

namespace ChartTools
{
    /// <summary>
    /// Set of track objects where each one must have a different position
    /// </summary>
    public class UniqueTrackObjectCollection<T> : UniqueList<T> where T : TrackObject
    {
        static readonly EqualityComparison<T> comparison = (a, b) => a is null || b is null || a.Position == b.Position;

        public UniqueTrackObjectCollection(int capacity = 0, IEnumerable<T>? items = null) : base(comparison, capacity, items) { }
    }
}
