namespace ChartTools.IO;

internal readonly struct Anchor(uint position, TimeSpan value) : IReadOnlyTrackObject
{
	public uint Position { get; } = position;

	public TimeSpan Value { get; } = value;
}
