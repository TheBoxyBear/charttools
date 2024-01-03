using ChartTools.Extensions.Collections;
using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Sessions;

namespace ChartTools.IO.Chart.Serializing;

internal abstract class TrackObjectGroupSerializer<T> : GroupSerializer<T, string, TrackObjectEntry>
{
    public TrackObjectGroupSerializer(string header, T content, WritingSession session) : base(header, content, session) { }

    protected override IEnumerable<string> CombineProviderResults(IEnumerable<TrackObjectEntry>[] results) => new OrderedAlternatingEnumerable<TrackObjectEntry, uint>(entry => entry.Position, results).Select(entry => entry.ToString());
}
