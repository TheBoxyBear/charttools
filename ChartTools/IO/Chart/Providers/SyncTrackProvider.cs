using ChartTools.IO.Chart.Entries;
using ChartTools.Extensions.Linq;
using ChartTools.IO.Chart.Configuration.Sessions;

namespace ChartTools.IO.Chart.Providers;

internal abstract class SyncTrackProvider<T> : ISerializerDataProvider<T, TrackObjectEntry, ChartWritingSession> where T : ITrackObject
{
    protected abstract string ObjectType { get; }

    public IEnumerable<TrackObjectEntry> ProvideFor(IEnumerable<T> source, ChartWritingSession session)
    {
        List<uint> orderedPositions = [];

        foreach (var item in source)
        {
            if (session.HandleDuplicate(item.Position, ObjectType, () =>
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
