using ChartTools.IO.Configuration.Sessions;

using System.Collections.Generic;
using System.Linq;

namespace ChartTools.IO.Chart.Providers
{
    internal class TempoProvider : ITrackObjectProvider<Tempo>
    {
        public IEnumerable<TrackObjectProviderEntry> ProvideFor(IEnumerable<Tempo> source, WritingSession session)
        {
            HashSet<uint> ignored = new();

            foreach (var tempo in source.Where(t => session.DuplicateTrackObjectProcedure(t.Position, ignored, "tempo marker")))
            {
                if (tempo.Anchor is not null)
                    yield return new(tempo.Position, $"A {ChartFormatting.Float((float)tempo.Anchor)}");
                yield return new(tempo.Position, $"B {ChartFormatting.Float(tempo.Value)}");
            }
        }
    }
}
