using ChartTools.IO.Chart.Providers;
using ChartTools.IO.Configuration.Sessions;

using System.Collections.Generic;

namespace ChartTools.IO.Chart.Serializers
{
    internal class SyncTrackSerializer : TrackObjectGroupSerializer<SyncTrack>
    {
        public SyncTrackSerializer(SyncTrack content, WritingSession session) : base(ChartFormatting.SyncTrackHeader, content, session) { }

        protected override IEnumerable<TrackObjectProviderEntry>[] LaunchProviders() => new IEnumerable<TrackObjectProviderEntry>[] { new TempoProvider().ProvideFor(Content.Tempo, session!), new TimeSignatureProvider().ProvideFor(Content.TimeSignatures, session!) };
    }
}
