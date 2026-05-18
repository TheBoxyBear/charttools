using ChartTools.Extensions;

using System.Runtime.CompilerServices;

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
					return Unsafe.As<bool, T>(false);
				case "1":
					return Unsafe.As<bool, T>(true);
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
				"0" => Unsafe.As<bool, T>(false),
				"1" => Unsafe.As<bool, T>(true),
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
			TypeCode.String => Unsafe.As<string, T>(value),
			_ => throw new ParseException(value.ToString(), target, typeof(T))
		};

		T Parse<TIn>(TryParseString<TIn> parseFunc)
			=> parseFunc(value, out TIn result)
				? Unsafe.As<TIn, T>(result)
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
					return Unsafe.As<bool, T>(false);
				case "1":
					return Unsafe.As<bool, T>(true);
			}

		return T.TryParse(value, null, out T? result) ? result
			: throw new ParseException(value.ToString(), target, typeof(T));
	}
#else
		=> Parse<T>(value.ToString(), target);
#endif
}
