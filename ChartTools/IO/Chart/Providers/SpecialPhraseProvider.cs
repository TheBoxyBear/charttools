using ChartTools.IO.Chart.Configuration.Sessions;
using ChartTools.IO.Chart.Entries;

namespace ChartTools.IO.Chart.Providers;

internal class SpeicalPhraseProvider : ISerializerDataProvider<TrackSpecialPhrase, TrackObjectEntry, ChartWritingSession>
{
    public IEnumerable<TrackObjectEntry> ProvideFor(IEnumerable<TrackSpecialPhrase> source, ChartWritingSession session) => source.Select(sp => new TrackObjectEntry(sp.Position, "S", $"{sp.TypeCode} {sp.Length}"));
}
