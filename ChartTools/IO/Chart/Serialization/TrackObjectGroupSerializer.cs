using ChartTools.Extensions.Linq;
using ChartTools.IO.Chart.Configuration.Sessions;
using ChartTools.IO.Chart.Entries;

namespace ChartTools.IO.Chart.Serialization;

internal abstract class TrackObjectGroupSerializer<T>(string header, T content, ChartWritingSession session)
    : GroupSerializer<T, string, TrackObjectEntry>(header, content)
{
    public ChartWritingSession Session { get; } = session;

    protected override IEnumerable<string> CombineProviderResults(IEnumerable<TrackObjectEntry>[] results) => results.AlternateBy(entry => entry.Position).Select(entry => entry.ToString());
}