using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Sessions;

using System.Collections.Generic;
using System.Linq;

namespace ChartTools.IO.Chart.Providers
{
    internal class SpeicalPhraseProvider : ISerializerDataProvider<SpecialPhrase, TrackObjectEntry>
    {
        public IEnumerable<TrackObjectEntry> ProvideFor(IEnumerable<SpecialPhrase> source, WritingSession session) => source.Select(sp => new TrackObjectEntry(sp.Position, "S", $"{sp.TypeCode} {sp.Length}"));
    }
}
