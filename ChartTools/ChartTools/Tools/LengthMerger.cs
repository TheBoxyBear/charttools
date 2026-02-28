namespace ChartTools.Tools;

public static class LengthMerger
{
	public static T MergeLengths<T>(this IEnumerable<T> objects, T? target = null)
		where T : class, ILongTrackObject
	{
		uint
			start = objects.Min(static o => o.Position),
			end   = objects.Max(static o => o.EndPosition);

		target ??= objects.First();

		target.Position = start;
		target.Length   = end - start;

		return target;
	}
}
