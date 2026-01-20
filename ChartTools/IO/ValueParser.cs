namespace ChartTools.IO;

internal static class ValueParser
{
	public static T Parse<T>(in ReadOnlySpan<char> value, string target)
			where T : ISpanParsable<T>
			=> T.TryParse(value, null, out T? result) ? result
		: throw new ParseException(value.ToString(), target, typeof(T));

	[Obsolete("Use Parse<bool> instead.")]
	public static bool ParseBool(in ReadOnlySpan<char> value, string target)
		=> Parse<bool>(value, target);

	[Obsolete("Use Parse<byte> instead.")]
	public static byte ParseByte(in ReadOnlySpan<char> value, string target)
		=> Parse<byte>(value, target);

	[Obsolete("Use Parse<sbyte> instead.")]
	public static sbyte ParseSbyte(in ReadOnlySpan<char> value, string target)
		=> Parse<sbyte>(value, target);

	[Obsolete("Use Parse<short> instead.")]
	public static short ParseShort(in ReadOnlySpan<char> value, string target)
		=> Parse<short>(value, target);

	[Obsolete("Use Parse<ushort> instead.")]
	public static ushort ParseUshort(in ReadOnlySpan<char> value, string target)
		=> Parse<ushort>(value, target);

	[Obsolete("Use Parse<int> instead.")]
	public static int ParseInt(in ReadOnlySpan<char> value, string target)
		=> Parse<int>(value, target);

	[Obsolete("Use Parse<uint> instead.")]
	public static uint ParseUint(in ReadOnlySpan<char> value, string target)
		=> Parse<uint>(value, target);

	[Obsolete("Use Parse<float> instead.")]
	public static float ParseFloat(in ReadOnlySpan<char> value, string target)
		=> Parse<float>(value, target);
}
