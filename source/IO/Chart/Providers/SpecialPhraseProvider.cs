using ChartTools.IO.Chart.Sessions;

using System.Collections.Generic;
using System.Linq;

namespace ChartTools.IO.Chart.Providers
{
    internal class StarPowerProvider : ITrackObjectProvider<SpecicalPhrase>
    {
        public IEnumerable<TrackObjectProviderEntry> ProvideFor(IEnumerable<SpecicalPhrase> source, WritingSession session) => source.Select(sp => new TrackObjectProviderEntry(sp.Position, ChartFormatting.Line(sp.Position.ToString(), $"S 2 {sp.Length}")));
    }
}
