using ChartTools.Events;
using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Chart.Mapping;
using ChartTools.IO.Configuration.Sessions;

using System.Collections.Generic;
using System.Linq;

namespace ChartTools.IO.Chart.Serialization;

internal class GlobalEventSerializer : TrackObjectGroupSerializer<IEnumerable<GlobalEvent>>
{
    public GlobalEventSerializer(IEnumerable<GlobalEvent> content, WritingSession session) : base(ChartFormatting.GlobalEventHeader, content, session) { }

    protected override IEnumerable<TrackObjectEntry>[] LaunchMappers() => new IEnumerable<TrackObjectEntry>[] { Content.Select(e => EventMapper.Map(e)) };
}
