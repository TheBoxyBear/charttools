using ChartTools.IO.Chart.Entries;
using System.Collections.Generic;

namespace ChartTools.IO.Chart.Providers
{
    internal class TempoProvider : SyncTrackProvider<Tempo>
    {
        protected override string ObjectType => "tempo marker";

        protected override IEnumerable<TrackObjectEntry> GetEntries(Tempo item)
        {
            if (item.Anchor is not null)
                yield return new(item.Position, "A", ChartFormatting.Float((float)item.Anchor));
            yield return new(item.Position, "B", ChartFormatting.Float(item.Value));
        }
    }
}
