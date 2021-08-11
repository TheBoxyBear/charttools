using System.Collections.Generic;

using ChartTools.Collections.Unique;
using ChartTools.SystemExtensions;

namespace ChartTools
{
    /// <summary>
    /// Set of star power phrases in a single track
    /// </summary>
    public class StarPowerCollection : UniqueList<StarPowerPhrase>
    {
        static readonly EqualityComparison<StarPowerPhrase> comparison = (a, b) => a is null || b is null || a.Position == b.Position;

        public StarPowerCollection(int capacity = 0, IEnumerable<StarPowerPhrase>? items = null) : base(comparison, capacity, items) { }
    }
}
