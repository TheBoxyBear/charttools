using ChartTools.IO.Chart.Entries;

namespace ChartTools.IO.Chart.Mapping
{
    internal static class SpecialPhraseMapper
    {
        public static TrackObjectEntry Map(TrackSpecialPhrase phrase) => new(phrase.Position, "S", $"{phrase.TypeCode} {phrase.Length}");
    }
}
