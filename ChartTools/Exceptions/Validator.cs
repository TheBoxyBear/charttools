namespace ChartTools;

internal static class Validator
{
	/// <summary>
	/// Validates that an <see cref="Enum"/> value is defined.
	/// </summary>
	/// <exception cref="UndefinedEnumException"></exception>
	public static void ValidateEnum<T>(T value)
		where T : struct, Enum
	{
		if (!Enum.IsDefined(value))
			throw new UndefinedEnumException(value);
	}
}
