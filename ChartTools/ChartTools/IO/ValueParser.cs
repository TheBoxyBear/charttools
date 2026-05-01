using ChartTools.Extensions;

namespace ChartTools.IO;

internal static class ValueParser
{
#if !NET7_0_OR_GREATER
	private delegate bool TryParseString<T>(string value, out T result);
	private delegate bool TryParseSpan<T>(in ReadOnlySpan<char> value, out T result);
#endif

	public static T Parse<T>(string value, string target)
#if NET7_0_OR_GREATER
		where T : IParsable<T>
	{
		if (typeof(T) == typeof(bool))
			switch (value)
			{
				case "0":
					return UnsafeExtensions.AsReadonly<bool, T>(false);
				case "1":
					return UnsafeExtensions.AsReadonly<bool, T>(true);
			}

		return T.TryParse(value, null, out T? result) ? result
			: throw new ParseException(value.ToString(), target, typeof(T));
	}
#else
	{
		Type targetType = typeof(T);

		if (targetType == typeof(TimeSpan))
			return Parse<TimeSpan>(TimeSpan.TryParse);

		return Type.GetTypeCode(targetType) switch
		{
			TypeCode.Boolean => value switch
			{
				"0" => UnsafeExtensions.AsReadonly<bool, T>(false),
				"1" => UnsafeExtensions.AsReadonly<bool, T>(true),
				_ => Parse<bool>(bool.TryParse)
			},
			TypeCode.Byte    => Parse<byte>(byte.TryParse),
			TypeCode.SByte   => Parse<sbyte>(sbyte.TryParse),
			TypeCode.Int16   => Parse<short>(short.TryParse),
			TypeCode.UInt16  => Parse<ushort>(ushort.TryParse),
			TypeCode.Int32   => Parse<int>(int.TryParse),
			TypeCode.UInt32  => Parse<uint>(uint.TryParse),
			TypeCode.Int64   => Parse<long>(long.TryParse),
			TypeCode.UInt64  => Parse<ulong>(ulong.TryParse),
			TypeCode.Single  => Parse<float>(float.TryParse),
			TypeCode.Double  => Parse<double>(double.TryParse),
			TypeCode.Decimal => Parse<decimal>(decimal.TryParse),
			TypeCode.String => UnsafeExtensions.AsReadonly<string, T>(value),
			_ => throw new ParseException(value.ToString(), target, typeof(T))
		};

		T Parse<TIn>(TryParseString<TIn> parseFunc)
			=> parseFunc(value, out TIn result)
				? UnsafeExtensions.AsReadonly<TIn, T>(result)
				: throw new ParseException(value.ToString(), target, typeof(T));
	}
#endif

	public static T Parse<T>(in ReadOnlySpan<char> value, string target)
#if NET7_0_OR_GREATER
		where T : ISpanParsable<T>
	{
		if (typeof(T) == typeof(bool))
			switch (value)
			{
				case "0":
					return UnsafeExtensions.AsReadonly<bool, T>(false);
				case "1":
					return UnsafeExtensions.AsReadonly<bool, T>(true);
			}

		return T.TryParse(value, null, out T? result) ? result
			: throw new ParseException(value.ToString(), target, typeof(T));
	}
#else
		=> Parse<T>(value.ToString(), target);
#endif

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
