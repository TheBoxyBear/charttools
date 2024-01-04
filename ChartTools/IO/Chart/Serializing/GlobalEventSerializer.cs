using ChartTools.Events;
using ChartTools.IO.Chart.Configuration.Sessions;
using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Chart.Providers;

namespace ChartTools.IO.Chart.Serializing;

internal class GlobalEventSerializer(IEnumerable<GlobalEvent> content, ChartWritingSession session)
    : TrackObjectGroupSerializer<IEnumerable<GlobalEvent>>(ChartFormatting.GlobalEventHeader, content, session)
{
    protected override IEnumerable<TrackObjectEntry>[] LaunchProviders() => new IEnumerable<TrackObjectEntry>[] { new EventProvider().ProvideFor(Content, session!) };
}
