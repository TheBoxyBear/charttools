using ChartTools.Events;
using ChartTools.IO.Chart.Serializers;
using ChartTools.IO.Configuration.Sessions;

using System.Collections.Generic;
using System.Linq;

namespace ChartTools.IO.Chart.Providers
{
    internal class EventProvider : ITrackObjectProvider<Event>
    {
        public IEnumerable<TrackObjectProviderEntry> ProvideFor(IEnumerable<Event> source, WritingSession session) => source.Select(e => new TrackObjectProviderEntry(e.Position, ChartSerializer.GetLine(e.Position.ToString(), $"E \"{e.EventData}\"")));
    }
}
