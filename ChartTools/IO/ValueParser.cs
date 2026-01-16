namespace ChartTools.IO;

internal static class ValueParser
{
	public delegate bool TryParse<T>(ReadOnlySpan<char> input, out T result);
	public static T Parse<T>(in ReadOnlySpan<char> value, string target, TryParse<T> tryParse) where T : struct
		=> tryParse(value, out T result) ? result : throw new ParseException(value.ToString(), target, typeof(T));

	public static bool ParseBool(in ReadOnlySpan<char> value, string target)     => Parse<bool>(value, target, bool.TryParse);
	public static byte ParseByte(in ReadOnlySpan<char> value, string target)     => Parse<byte>(value, target, byte.TryParse);
	public static sbyte ParseSbyte(in ReadOnlySpan<char> value, string target)   => Parse<sbyte>(value, target, sbyte.TryParse);
	public static short ParseShort(in ReadOnlySpan<char> value, string target)   => Parse<short>(value, target, short.TryParse);
	public static ushort ParseUshort(in ReadOnlySpan<char> value, string target) => Parse<ushort>(value, target, ushort.TryParse);
	public static int ParseInt(in ReadOnlySpan<char> value, string target)       => Parse<int>(value, target, int.TryParse);
	public static uint ParseUint(in ReadOnlySpan<char> value, string target)     => Parse<uint>(value, target, uint.TryParse);
	public static float ParseFloat(in ReadOnlySpan<char> value, string target)   => Parse<float>(value, target, float.TryParse);
}
