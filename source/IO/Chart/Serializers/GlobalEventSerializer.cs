using ChartTools.IO.Chart.Providers;
using System.Collections.Generic;

namespace ChartTools.IO.Chart.Serializers
{
    internal class GlobalEventSerializer : TrackObjectSerializer<IEnumerable<GlobalEvent>>
    {
        public GlobalEventSerializer(string header, IEnumerable<GlobalEvent> content) : base(header, content) { }

        protected override IEnumerable<TrackObjectProviderEntry>[] LaunchProviders() => new IEnumerable<TrackObjectProviderEntry>[] { new EventProvider().ProvideFor(Content, session!) };
    }
}
