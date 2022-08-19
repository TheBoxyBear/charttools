using ChartTools.Events;
using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Mapping;

namespace ChartTools.IO.Chart.Mapping
{
    internal class EventMapper : IWriteMapper<Event, TrackObjectEntry>
    {
        public TrackObjectEntry Map(Event e, WritingSession session) => new(e.Position, "E", e.EventData);
    }
}
