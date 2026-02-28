namespace ChartTools;

/// <summary>
/// Marker that alters the time signature
/// </summary>
/// <param name="position">Value of <see cref="Position"/></param>
/// <param name="numerator">Value of <see cref="Numerator"/></param>
/// <param name="denominator">Value of <see cref="Denominator"/></param>
public class TimeSignature(uint position, byte numerator, byte denominator) : ITrackObject
{
	/// <inheritdoc cref="ITrackObject.Position"/>"
	public uint Position { get; set; } = position;

	/// <summary>
	/// Value of a beat
	/// </summary>
	public byte Numerator { get; set; } = numerator;

	/// <summary>
	/// Beats per measure
	/// </summary>
	public byte Denominator { get; set; } = denominator;
}
