using ChartTools.IO.Chart.Entries;

namespace ChartTools.IO.Chart.Providers;

internal class TimeSignatureProvider : SyncTrackProvider<TimeSignature>
{
    protected override string ObjectType => "time signature";

    protected override IEnumerable<TrackObjectEntry> GetEntries(TimeSignature item)
    {
        byte writtenDenominator = (byte)Math.Log2(item.Denominator);
        string data = item.Numerator.ToString();

        if (writtenDenominator == 1)
            data += ' ' + writtenDenominator.ToString();

        yield return new(item.Position, "TS", data);
    }
}
