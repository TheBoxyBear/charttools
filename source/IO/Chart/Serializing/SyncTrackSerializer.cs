using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Chart.Mapping;
using ChartTools.IO.Configuration.Sessions;

using System.Collections.Generic;

namespace ChartTools.IO.Chart.Serializing
{
    internal class SyncTrackSerializer : TrackObjectGroupSerializer<SyncTrack>
    {
        public SyncTrackSerializer(SyncTrack content, WritingSession session) : base(ChartFormatting.SyncTrackHeader, content, session) { }

        protected override IEnumerable<TrackObjectEntry>[] LaunchMappers() => new IEnumerable<TrackObjectEntry>[] { new TempoMapper().Map(Content.Tempo, session), new TimeSignatureMapper().Map(Content.TimeSignatures, session) };
    }
}
