using ChartTools.IO.Chart.Entries;

namespace ChartTools.IO.Chart.Providers;

internal class TimeSignatureProvider : SyncTrackProvider<TimeSignature>
{
	protected override string ObjectType => "time signature";

	protected override IEnumerable<TrackObjectEntry> GetEntries(TimeSignature item)
	{
		byte writtenDenominator =
#if NETCOREAPP3_0_OR_GREATER
			(byte)Math.Log2(item.Denominator);
#else
			(byte)Math.Log(item.Denominator, 2);
#endif
		string data = item.Numerator.ToString();

		if (writtenDenominator == 1)
			data += ' ' + writtenDenominator.ToString();

		yield return new(item.Position, "TS", data);
	}
}
