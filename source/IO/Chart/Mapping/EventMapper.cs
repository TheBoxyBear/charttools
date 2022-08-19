using ChartTools.Events;
using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Mapping;

using System.Collections.Generic;
using System.Linq;

namespace ChartTools.IO.Chart.Mapping
{
    internal class EventMapper : IWriteMapper<IEnumerable<Event>, TrackObjectEntry>
    {
        public IEnumerable<TrackObjectEntry> Map(IEnumerable<Event> source, WritingSession session) => source.Select(e => new TrackObjectEntry(e.Position, "E", e.EventData));
    }
}
