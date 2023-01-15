using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Chart.Providers;
using ChartTools.IO.Configuration.Sessions;

namespace ChartTools.IO.Chart.Serializing;

internal class SyncTrackSerializer : TrackObjectGroupSerializer<SyncTrack>
{
    public SyncTrackSerializer(SyncTrack content, WritingSession session) : base(ChartFormatting.SyncTrackHeader, content, session) { }

    protected override IEnumerable<TrackObjectEntry>[] LaunchProviders() => new IEnumerable<TrackObjectEntry>[] { new TempoProvider().ProvideFor(Content.Tempo, session), new TimeSignatureProvider().ProvideFor(Content.TimeSignatures, session) };
}
