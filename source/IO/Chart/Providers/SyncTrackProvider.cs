using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Sessions;

using System.Collections.Generic;

namespace ChartTools.IO.Chart.Providers
{
    internal abstract class SyncTrackProvider<T> : ISerializerDataProvider<T, TrackObjectEntry> where T : TrackObject
    {
        protected abstract string ObjectType { get; }

        public IEnumerable<TrackObjectEntry> ProvideFor(IEnumerable<T> source, WritingSession session)
        {
            HashSet<uint> existingPositions = new();

            foreach (var item in source)
            {
                if (session.DuplicateTrackObjectProcedure(item.Position, ObjectType, () => existingPositions.Contains(item.Position)))
                    foreach (var entry in GetEntries(item))
                        yield return entry;

                existingPositions.Add(item.Position);
            }
        }

        protected abstract IEnumerable<TrackObjectEntry> GetEntries(T item);
    }
}
