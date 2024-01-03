using ChartTools.IO.Chart.Entries;

namespace ChartTools.IO.Chart.Providers;

internal class TempoProvider : SyncTrackProvider<Tempo>
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
