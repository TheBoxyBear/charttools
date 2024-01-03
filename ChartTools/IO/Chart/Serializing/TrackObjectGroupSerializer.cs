using ChartTools.Extensions.Linq;
using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Sessions;

namespace ChartTools.IO.Chart.Serializing;

internal abstract class TrackObjectGroupSerializer<T> : GroupSerializer<T, string, TrackObjectEntry>
{
    public TrackObjectGroupSerializer(string header, T content, WritingSession session) : base(header, content, session) { }

    protected override IEnumerable<string> CombineProviderResults(IEnumerable<TrackObjectEntry>[] results) => results.AlternateBy(entry => entry.Position).Select(entry => entry.ToString());
}