using ChartTools.IO.Chart.Providers;
using System.Collections.Generic;

namespace ChartTools.IO.Chart.Serializers
{
    internal class SyncTrackSerializer : TrackObjectSerializer<SyncTrack>
    {
        public SyncTrackSerializer(string header, SyncTrack content) : base(header, content) { }

        protected override IEnumerable<TrackObjectProviderEntry>[] LaunchProviders() => new IEnumerable<TrackObjectProviderEntry>[] { new TempoProvider().ProvideFor(Content.Tempo, session!), new TimeSignatureProvider().ProvideFor(Content.TimeSignatures, session!) };
    }
}
