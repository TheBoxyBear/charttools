using ChartTools.IO.Configuration.Sessions;

using System.Collections.Generic;
using System.Linq;

namespace ChartTools.IO.Chart.Providers
{
    internal class EventProvider : ISerializerDataProvider<Event, TrackObjectProviderEntry>
    {
        public IEnumerable<TrackObjectProviderEntry> ProvideFor(IEnumerable<Event> source, WritingSession session) => source.Select(e => new TrackObjectProviderEntry(e.Position, ChartFormatting.Line(e.Position.ToString(), $"E \"{e.EventData}\"")));
    }
}
