using ChartTools.Extensions.Linq;
using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Sessions;

using System.Collections.Generic;

namespace ChartTools.IO.Chart.Mapping
{
    internal abstract class UniqueTrackObjectMapper<T> where T : ITrackObject
    {
        protected abstract string ObjectType { get; }

        public IEnumerable<TrackObjectEntry> Map(IEnumerable<T> source, WritingSession session)
        {
            List<uint> orderedPositions = new();

            foreach (var item in source)
            {
                if (session.DuplicateTrackObjectProcedure(item.Position, ObjectType, () =>
                {
                    var index = orderedPositions.BinarySearchIndex(item.Position, out bool exactMatch);

                    if (!exactMatch)
                        orderedPositions.Insert(index, item.Position);

                    return exactMatch;
                }))
                    foreach (var entry in GetEntries(item))
                        yield return entry;

                orderedPositions.Add(item.Position);
            }
        }

        protected abstract IEnumerable<TrackObjectEntry> GetEntries(T item);
    }
}
