using ChartTools.IO.Chart.Providers;
using System.Collections.Generic;

namespace ChartTools.IO.Chart.Serializers
{
    internal class GlobalEventSerializer : TrackObjectGroupSerializer<IEnumerable<GlobalEvent>>
    {
        public GlobalEventSerializer(IEnumerable<GlobalEvent> content) : base(ChartFormatting.GlobalEventHeader, content) { }

        protected override IEnumerable<TrackObjectProviderEntry>[] LaunchProviders() => new IEnumerable<TrackObjectProviderEntry>[] { new EventProvider().ProvideFor(Content, session!) };
    }
}
