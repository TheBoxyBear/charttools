using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Mapping;

using System.Collections.Generic;
using System.Linq;

namespace ChartTools.IO.Chart.Mapping
{
    internal class SpecialPhraseMapper : IWriteMapper<IEnumerable<TrackSpecialPhrase>, TrackObjectEntry>
    {
        public IEnumerable<TrackObjectEntry> Map(IEnumerable<TrackSpecialPhrase> source, WritingSession session) => source.Select(sp => new TrackObjectEntry(sp.Position, "S", $"{sp.TypeCode} {sp.Length}"));
    }
}
