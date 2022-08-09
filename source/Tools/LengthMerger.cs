using System.Collections.Generic;
using System.Linq;

namespace ChartTools.Tools
{
    public static class LengthMerger
    {
        public static T MergeLengths<T>(this IEnumerable<T> objects, T? target = null) where T : class, ILongTrackObject
        {
            var start = objects.Min(o => o.Position);
            var end = objects.Max(o => o.EndPosition);
            target ??= objects.First();

            target.Position = start;
            target.Length = end - start;

            return target;
        }
    }
}
