using ChartTools.IO.Chart.Entries;
using System.Collections.Generic;

namespace ChartTools.IO.Chart.Mapping
{
    internal class TempoMapper : UniqueTrackObjectMapper<Tempo>
    {
        protected override string ObjectType => "tempo marker";

        protected override IEnumerable<TrackObjectEntry> GetEntries(Tempo item)
        {
            if (item.Anchor is not null)
                yield return item.PositionSynced
                    ? new(item.Position, "A", ChartFormatting.Float((float)item.Anchor.Value.TotalSeconds))
                    : throw new DesynchronizedAnchorException(item.Anchor.Value, $"Cannot write desynchronized anchored tempo at {item.Anchor}.");
            yield return new(item.Position, "B", ChartFormatting.Float(item.Value));
        }
    }
}
