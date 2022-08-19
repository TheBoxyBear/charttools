using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Mapping;

namespace ChartTools.IO.Chart.Mapping
{
    internal class SpecialPhraseMapper : IWriteMapper<TrackSpecialPhrase, TrackObjectEntry>
    {
        public TrackObjectEntry Map(TrackSpecialPhrase phrase, WritingSession session) => new(phrase.Position, "S", $"{phrase.TypeCode} {phrase.Length}");
    }
}
