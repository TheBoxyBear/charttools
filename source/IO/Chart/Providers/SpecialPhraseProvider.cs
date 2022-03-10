using ChartTools.IO.Configuration.Sessions;

using System.Collections.Generic;
using System.Linq;

namespace ChartTools.IO.Chart.Providers
{
    internal class SpeicalPhraseProvider : ISerializerDataProvider<SpecialPhrase, TrackObjectProviderEntry>
    {
        public IEnumerable<TrackObjectProviderEntry> ProvideFor(IEnumerable<SpecialPhrase> source, WritingSession session) => source.Select(sp => new TrackObjectProviderEntry(sp.Position, ChartFormatting.Line(sp.Position.ToString(), $"S {sp.TypeCode} {sp.Length}")));
    }
}
