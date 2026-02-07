namespace ChartTools;

/// <summary>
/// Interface for objects with a defined length in ticks
/// </summary>
public interface ILongObject : IReadOnlyLongObject
{
	/// <inheritdoc cref="IReadOnlyTrackObject.Position"/>
	public new uint Length { get; set; }

	uint IReadOnlyLongObject.Length => Length;
}
