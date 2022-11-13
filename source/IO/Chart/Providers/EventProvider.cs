using ChartTools.Events;
using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Sessions;

namespace ChartTools.IO.Chart.Providers;

internal class EventProvider : ISerializerDataProvider<Event, TrackObjectEntry>
{
    public IEnumerable<TrackObjectEntry> ProvideFor(IEnumerable<Event> source, WritingSession session) => source.Select(e => new TrackObjectEntry(e.Position, "E", e.EventData));
}
