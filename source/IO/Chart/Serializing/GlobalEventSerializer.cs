using ChartTools.Events;
using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Chart.Providers;
using ChartTools.IO.Configuration.Sessions;

using System.Collections.Generic;

namespace ChartTools.IO.Chart.Serializing
{
    internal class GlobalEventSerializer : TrackObjectGroupSerializer<IEnumerable<GlobalEvent>>
    {
        public GlobalEventSerializer(IEnumerable<GlobalEvent> content, WritingSession session) : base(ChartFormatting.GlobalEventHeader, content, session) { }

        protected override IEnumerable<TrackObjectEntry>[] LaunchProviders() => new IEnumerable<TrackObjectEntry>[] { new EventProvider().ProvideFor(Content, session!) };
    }
}
