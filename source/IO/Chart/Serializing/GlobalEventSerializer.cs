using ChartTools.Events;
using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Chart.Mapping;
using ChartTools.IO.Configuration.Sessions;

using System.Collections.Generic;

namespace ChartTools.IO.Chart.Serializing
{
    internal class GlobalEventSerializer : TrackObjectGroupSerializer<IEnumerable<GlobalEvent>>
    {
        public GlobalEventSerializer(IEnumerable<GlobalEvent> content, WritingSession session) : base(ChartFormatting.GlobalEventHeader, content, session) { }

        protected override IEnumerable<TrackObjectEntry>[] LaunchMappers() => new IEnumerable<TrackObjectEntry>[] { new EventMapper().Map(Content, session) };
    }
}
