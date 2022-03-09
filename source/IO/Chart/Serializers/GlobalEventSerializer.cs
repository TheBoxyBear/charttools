using ChartTools.Events;
using ChartTools.IO.Chart.Providers;
using ChartTools.IO.Configuration.Sessions;

using System.Collections.Generic;

namespace ChartTools.IO.Chart.Serializers
{
    internal class GlobalEventSerializer : TrackObjectGroupSerializer<IEnumerable<GlobalEvent>>
    {
        public GlobalEventSerializer(IEnumerable<GlobalEvent> content, WritingSession session) : base(ChartFormatting.GlobalEventHeader, content, session) { }

        protected override IEnumerable<TrackObjectProviderEntry>[] LaunchProviders() => new IEnumerable<TrackObjectProviderEntry>[] { new EventProvider().ProvideFor(Content, session!) };
    }
}
