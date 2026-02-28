namespace ChartTools;

/// <summary>
/// Interface for objects with a defined length in ticks where the length is read-only
/// </summary>
public interface IReadOnlyLongObject
{
	/// <summary>
	/// Length of the object in ticks
	/// </summary>
	public uint Length { get; }
}
