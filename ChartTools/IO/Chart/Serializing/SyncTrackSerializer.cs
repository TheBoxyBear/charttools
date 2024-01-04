﻿using ChartTools.IO.Chart.Configuration.Sessions;
using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Chart.Providers;
namespace ChartTools.IO.Chart.Serializing;

internal class SyncTrackSerializer(SyncTrack content, ChartWritingSession session)
    : TrackObjectGroupSerializer<SyncTrack>(ChartFormatting.SyncTrackHeader, content, session)
{
    protected override IEnumerable<TrackObjectEntry>[] LaunchProviders() => new IEnumerable<TrackObjectEntry>[] { new TempoProvider().ProvideFor(Content.Tempo, session), new TimeSignatureProvider().ProvideFor(Content.TimeSignatures, session) };
}
