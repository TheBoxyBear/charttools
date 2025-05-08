namespace ChartTools;

/// <summary>
/// Base class for phrases that define an in-game event with a duration such as star power.
/// </summary>
/// <remarks>
/// Base constructor of special phrases.
/// </remarks>
/// <param name="position">Position of the phrase</param>
/// <param name="typeCode">Effect of the phrase</param>
/// <param name="length">Duration in ticks</param>
public abstract class SpecialPhrase(uint position, byte typeCode, uint length = 0) : ILongTrackObject
{
	public uint Position { get; set; } = position;

	/// <summary>
	/// Numerical value of the phrase type
	/// </summary>
	public byte TypeCode { get; set; } = typeCode;

	/// <summary>
	/// Duration of the phrase in ticks
	/// </summary>
	public uint Length { get; set; } = length;
}
