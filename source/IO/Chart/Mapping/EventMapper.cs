using ChartTools.Events;
using ChartTools.IO.Chart.Entries;

namespace ChartTools.IO.Chart.Mapping
{
    internal static class EventMapper
    {
        public static TrackObjectEntry Map(Event e) => new(e.Position, "E", e.EventData);
    }
}
