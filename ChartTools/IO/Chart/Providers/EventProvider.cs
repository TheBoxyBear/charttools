using ChartTools.Events;
using ChartTools.IO.Chart.Configuration.Sessions;
using ChartTools.IO.Chart.Entries;

namespace ChartTools.IO.Chart.Providers;

internal class EventProvider : ISerializerDataProvider<Event, TrackObjectEntry, ChartWritingSession>
{
	public IEnumerable<TrackObjectEntry> ProvideFor(IEnumerable<Event> source, ChartWritingSession session)
		=> source.Select(static e => new TrackObjectEntry(e.Position, "E", $"\"{e.EventData}\""));
}
