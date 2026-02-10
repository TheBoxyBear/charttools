#if NET7_0_OR_GREATER
using System.Numerics;
#endif
using System.Runtime.CompilerServices;

using UnsafeEx = ChartTools.Extensions.UnsafeExtensions;

namespace ChartTools.Extensions.Enums;

public static class EnumExtensions
{
	extension<T>(T)
		where T : struct, Enum
	{
		public static T Parse(string value, bool ignoreCase = false)
#if NETSTANDARD2_0
			=> (T)Enum.Parse(typeof(T), value, ignoreCase);
#else
			=> Enum.Parse<T>(value, ignoreCase);
#endif

		public static T Parse(in ReadOnlySpan<char> value, bool ignoreCase = false)
#if NETSTANDARD2_0
			=> (T)Enum.Parse(typeof(T), value.ToString(), ignoreCase);
#elif NETSTANDARD2_1
			=> Enum.Parse<T>(value.ToString(), ignoreCase);
#else
			=> Enum.Parse<T>(value, ignoreCase);
#endif

		public static bool TryParse(string value, out T enumValue, bool ignoreCase = false)
			=> Enum.TryParse(value, ignoreCase, out enumValue);

		public static bool TryParse(in ReadOnlySpan<char> value, out T enumValue, bool ignoreCase = false)
#if NETSTANDARD
			=> Enum.TryParse(value.ToString(), ignoreCase, out enumValue);
#else
			=> Enum.TryParse(value, ignoreCase, out enumValue);
#endif

		public static bool IsDefined(T value)
#if NET5_0_OR_GREATER
			=> Enum.IsDefined(value);
#else
			=> Enum.IsDefined(typeof(T), value);
#endif

		public static bool operator ==(T left, T right)
			=> EqualityComparer<T>.Default.Equals(left, right);

		public static bool operator !=(T left, T right)
			=> !EqualityComparer<T>.Default.Equals(left, right);

		public static bool operator <(T left, T right)
			=> Comparer<T>.Default.Compare(left, right) < 0;

		public static bool operator >(T left, T right)
			=> Comparer<T>.Default.Compare(left, right) > 0;

		public static bool operator <=(T left, T right)
			=> Comparer<T>.Default.Compare(left, right) <= 0;

		public static bool operator >=(T left, T right)
			=> Comparer<T>.Default.Compare(left, right) >= 0;

		// There's not really a cleaner way of doing this while being compatible with .net standard
		public static T operator|(T left, T right)
			=> EnumCache<T>.UnderlyingTypeCode switch
			{
				TypeCode.Byte   => UnsafeEx.AsReadonly<int, T>(Unsafe.As<T, byte>(ref left)    | Unsafe.As<T, byte>(ref right)),
				TypeCode.SByte  => UnsafeEx.AsReadonly<int, T>(Unsafe.As<T, sbyte>(ref left)   | Unsafe.As<T, sbyte>(ref right)),
				TypeCode.Int16  => UnsafeEx.AsReadonly<int, T>(Unsafe.As<T, short>(ref left)   | Unsafe.As<T, short>(ref right)),
				TypeCode.UInt16 => UnsafeEx.AsReadonly<int, T>(Unsafe.As<T, ushort>(ref left)  | Unsafe.As<T, ushort>(ref right)),
				TypeCode.Int32  => UnsafeEx.AsReadonly<int, T>(Unsafe.As<T, int>(ref left)	   | Unsafe.As<T, int>(ref right)),
				TypeCode.UInt32 => UnsafeEx.AsReadonly<uint, T>(Unsafe.As<T, uint>(ref left)   | Unsafe.As<T, uint>(ref right)),
				TypeCode.Int64  => UnsafeEx.AsReadonly<long, T>(Unsafe.As<T, long>(ref left)   | Unsafe.As<T, long>(ref right)),
				TypeCode.UInt64 => UnsafeEx.AsReadonly<ulong, T>(Unsafe.As<T, ulong>(ref left) | Unsafe.As<T, ulong>(ref right))
			};

		public static T operator &(T left, T right)
			=> EnumCache<T>.UnderlyingTypeCode switch
			{
				TypeCode.Byte   => UnsafeEx.AsReadonly<int, T>(Unsafe.As<T, byte>(ref left)	   & Unsafe.As<T, byte>(ref right)),
				TypeCode.SByte  => UnsafeEx.AsReadonly<int, T>(Unsafe.As<T, sbyte>(ref left)   & Unsafe.As<T, sbyte>(ref right)),
				TypeCode.Int16  => UnsafeEx.AsReadonly<int, T>(Unsafe.As<T, short>(ref left)   & Unsafe.As<T, short>(ref right)),
				TypeCode.UInt16 => UnsafeEx.AsReadonly<int, T>(Unsafe.As<T, ushort>(ref left)  & Unsafe.As<T, ushort>(ref right)),
				TypeCode.Int32  => UnsafeEx.AsReadonly<int, T>(Unsafe.As<T, int>(ref left)	   & Unsafe.As<T, int>(ref right)),
				TypeCode.UInt32 => UnsafeEx.AsReadonly<uint, T>(Unsafe.As<T, uint>(ref left)   & Unsafe.As<T, uint>(ref right)),
				TypeCode.Int64  => UnsafeEx.AsReadonly<long, T>(Unsafe.As<T, long>(ref left)   & Unsafe.As<T, long>(ref right)),
				TypeCode.UInt64 => UnsafeEx.AsReadonly<ulong, T>(Unsafe.As<T, ulong>(ref left) & Unsafe.As<T, ulong>(ref right))
			};

		public static T operator ^(T left, T right)
			=> EnumCache<T>.UnderlyingTypeCode switch
			{
				TypeCode.Byte   => UnsafeEx.AsReadonly<int, T>(Unsafe.As<T, byte>(ref left)    ^ Unsafe.As<T, byte>(ref right)),
				TypeCode.SByte  => UnsafeEx.AsReadonly<int, T>(Unsafe.As<T, sbyte>(ref left)   ^ Unsafe.As<T, sbyte>(ref right)),
				TypeCode.Int16  => UnsafeEx.AsReadonly<int, T>(Unsafe.As<T, short>(ref left)   ^ Unsafe.As<T, short>(ref right)),
				TypeCode.UInt16 => UnsafeEx.AsReadonly<int, T>(Unsafe.As<T, ushort>(ref left)  ^ Unsafe.As<T, ushort>(ref right)),
				TypeCode.Int32  => UnsafeEx.AsReadonly<int, T>(Unsafe.As<T, int>(ref left)	   ^ Unsafe.As<T, int>(ref right)),
				TypeCode.UInt32 => UnsafeEx.AsReadonly<uint, T>(Unsafe.As<T, uint>(ref left)   ^ Unsafe.As<T, uint>(ref right)),
				TypeCode.Int64  => UnsafeEx.AsReadonly<long, T>(Unsafe.As<T, long>(ref left)   ^ Unsafe.As<T, long>(ref right)),
				TypeCode.UInt64 => UnsafeEx.AsReadonly<ulong, T>(Unsafe.As<T, ulong>(ref left) ^ Unsafe.As<T, ulong>(ref right)),
			};

		public static T operator ~(T value)
			=> EnumCache<T>.UnderlyingTypeCode switch
			{
				TypeCode.Byte   => UnsafeEx.AsReadonly<int, T>(~Unsafe.As<T, byte>(ref value)),
				TypeCode.SByte  => UnsafeEx.AsReadonly<int, T>(~Unsafe.As<T, sbyte>(ref value)),
				TypeCode.Int16  => UnsafeEx.AsReadonly<int, T>(~Unsafe.As<T, short>(ref value)),
				TypeCode.UInt16 => UnsafeEx.AsReadonly<int, T>(~Unsafe.As<T, ushort>(ref value)),
				TypeCode.Int32  => UnsafeEx.AsReadonly<int, T>(~Unsafe.As<T, int>(ref value)),
				TypeCode.UInt32 => UnsafeEx.AsReadonly<uint, T>(~Unsafe.As<T, uint>(ref value)),
				TypeCode.Int64  => UnsafeEx.AsReadonly<long, T>(~Unsafe.As<T, long>(ref value)),
				TypeCode.UInt64 => UnsafeEx.AsReadonly<ulong, T>(~Unsafe.As<T, ulong>(ref value)),
			};

		public static T operator <<(T left, int shift)
			=> EnumCache<T>.UnderlyingTypeCode switch
			{
				TypeCode.Byte   => UnsafeEx.AsReadonly<int, T>(Unsafe.As<T, byte>(ref left)	   << shift),
				TypeCode.SByte  => UnsafeEx.AsReadonly<int, T>(Unsafe.As<T, sbyte>(ref left)   << shift),
				TypeCode.Int16  => UnsafeEx.AsReadonly<int, T>(Unsafe.As<T, short>(ref left)   << shift),
				TypeCode.UInt16 => UnsafeEx.AsReadonly<int, T>(Unsafe.As<T, ushort>(ref left)  << shift),
				TypeCode.Int32  => UnsafeEx.AsReadonly<int, T>(Unsafe.As<T, int>(ref left)	   << shift),
				TypeCode.UInt32 => UnsafeEx.AsReadonly<uint, T>(Unsafe.As<T, uint>(ref left)   << shift),
				TypeCode.Int64  => UnsafeEx.AsReadonly<long, T>(Unsafe.As<T, long>(ref left)   << shift),
				TypeCode.UInt64 => UnsafeEx.AsReadonly<ulong, T>(Unsafe.As<T, ulong>(ref left) << shift),
			};

		public static T operator >>(T left, int shift)
			=> EnumCache<T>.UnderlyingTypeCode switch
			{
				TypeCode.Byte   => UnsafeEx.AsReadonly<int, T>(Unsafe.As<T, byte>(ref left)    >> shift),
				TypeCode.SByte  => UnsafeEx.AsReadonly<int, T>(Unsafe.As<T, sbyte>(ref left)   >> shift),
				TypeCode.Int16  => UnsafeEx.AsReadonly<int, T>(Unsafe.As<T, short>(ref left)   >> shift),
				TypeCode.UInt16 => UnsafeEx.AsReadonly<int, T>(Unsafe.As<T, ushort>(ref left)  >> shift),
				TypeCode.Int32  => UnsafeEx.AsReadonly<int, T>(Unsafe.As<T, int>(ref left)     >> shift),
				TypeCode.UInt32 => UnsafeEx.AsReadonly<uint, T>(Unsafe.As<T, uint>(ref left)   >> shift),
				TypeCode.Int64  => UnsafeEx.AsReadonly<long, T>(Unsafe.As<T, long>(ref left)   >> shift),
				TypeCode.UInt64 => UnsafeEx.AsReadonly<ulong, T>(Unsafe.As<T, ulong>(ref left) >> shift),
			};

		public static T operator >>>(T left, int shift)
			=> EnumCache<T>.UnderlyingTypeCode switch
			{
				TypeCode.Byte   => UnsafeEx.AsReadonly<int, T>(Unsafe.As<T, byte>(ref left)    >>> shift),
				TypeCode.SByte  => UnsafeEx.AsReadonly<int, T>(Unsafe.As<T, sbyte>(ref left)   >>> shift),
				TypeCode.Int16  => UnsafeEx.AsReadonly<int, T>(Unsafe.As<T, short>(ref left)   >>> shift),
				TypeCode.UInt16 => UnsafeEx.AsReadonly<int, T>(Unsafe.As<T, ushort>(ref left)  >>> shift),
				TypeCode.Int32  => UnsafeEx.AsReadonly<int, T>(Unsafe.As<T, int>(ref left)	   >>> shift),
				TypeCode.UInt32 => UnsafeEx.AsReadonly<uint, T>(Unsafe.As<T, uint>(ref left)   >>> shift),
				TypeCode.Int64  => UnsafeEx.AsReadonly<long, T>(Unsafe.As<T, long>(ref left)   >>> shift),
				TypeCode.UInt64 => UnsafeEx.AsReadonly<ulong, T>(Unsafe.As<T, ulong>(ref left) >>> shift),
			};
	}

	extension<T1, T2>(T1)
		where T1 : struct, Enum
		where T2 : unmanaged
#if NET5_0_OR_GREATER
		, IBinaryInteger<T2>
#endif
	{
		public static T1 operator +(T1 left, T2 right)
		{
#if NET5_0_OR_GREATER
			T2 value = left.As<T1, T2>() + right;
			return Unsafe.As<T2, T1>(ref value);
#else
			return Type.GetTypeCode(typeof(T2)) switch
			{
				TypeCode.Byte   => UnsafeEx.AsReadonly<int, T1>(Unsafe.As<T1, byte>(ref left)	 + Unsafe.As<T2, byte>(ref right)),
				TypeCode.SByte  => UnsafeEx.AsReadonly<int, T1>(Unsafe.As<T1, byte>(ref left)	 + Unsafe.As<T2, byte>(ref right)),
				TypeCode.Int16  => UnsafeEx.AsReadonly<int, T1>(Unsafe.As<T1, short>(ref left)	 + Unsafe.As<T2, short>(ref right)),
				TypeCode.UInt16 => UnsafeEx.AsReadonly<int, T1>(Unsafe.As<T1, ushort>(ref left)  + Unsafe.As<T2, ushort>(ref right)),
				TypeCode.Int32  => UnsafeEx.AsReadonly<int, T1>(Unsafe.As<T1, int>(ref left)	 + Unsafe.As<T2, int>(ref right)),
				TypeCode.UInt32 => UnsafeEx.AsReadonly<uint, T1>(Unsafe.As<T1, uint>(ref left)	 + Unsafe.As<T2, uint>(ref right)),
				TypeCode.Int64  => UnsafeEx.AsReadonly<long, T1>(Unsafe.As<T1, long>(ref left)	 + Unsafe.As<T2, long>(ref right)),
				TypeCode.UInt64 => UnsafeEx.AsReadonly<ulong, T1>(Unsafe.As<T1, ulong>(ref left) + Unsafe.As<T2, ulong>(ref right)),
				_ => throw new NotSupportedException($"Underlying type of {typeof(T1)} is not supported."),
			};
#endif
		}

		public static T1 operator -(T1 left, T2 right)
		{
#if NET5_0_OR_GREATER
			T2 value = left.As<T1, T2>() - right;
			return Unsafe.As<T2, T1>(ref value);
#else
			return Type.GetTypeCode(typeof(T2)) switch
			{
				TypeCode.Byte   => UnsafeEx.AsReadonly<int, T1>(Unsafe.As<T1, byte>(ref left)	 - Unsafe.As<T2, byte>(ref right)),
				TypeCode.SByte  => UnsafeEx.AsReadonly<int, T1>(Unsafe.As<T1, byte>(ref left)    - Unsafe.As<T2, byte>(ref right)),
				TypeCode.Int16  => UnsafeEx.AsReadonly<int, T1>(Unsafe.As<T1, short>(ref left)   - Unsafe.As<T2, short>(ref right)),
				TypeCode.UInt16 => UnsafeEx.AsReadonly<int, T1>(Unsafe.As<T1, ushort>(ref left)  - Unsafe.As<T2, ushort>(ref right)),
				TypeCode.Int32  => UnsafeEx.AsReadonly<int, T1>(Unsafe.As<T1, int>(ref left)	 - Unsafe.As<T2, int>(ref right)),
				TypeCode.UInt32 => UnsafeEx.AsReadonly<uint, T1>(Unsafe.As<T1, uint>(ref left)   - Unsafe.As<T2, uint>(ref right)),
				TypeCode.Int64  => UnsafeEx.AsReadonly<long, T1>(Unsafe.As<T1, long>(ref left)   - Unsafe.As<T2, long>(ref right)),
				TypeCode.UInt64 => UnsafeEx.AsReadonly<ulong, T1>(Unsafe.As<T1, ulong>(ref left) - Unsafe.As<T2, ulong>(ref right)),
				_ => throw new NotSupportedException($"Underlying type of {typeof(T1)} is not supported."),
			};
#endif
		}
	}

#if NET10_0_OR_GREATER
	extension<T>(ref T value)
		where T : struct, Enum
	{
		public void operator ++()
		{
			switch (Type.GetTypeCode(typeof(T)))
			{
				case TypeCode.Byte:
					Unsafe.As<T, byte>(ref value)++;
					break;
				case TypeCode.SByte:
					Unsafe.As<T, sbyte>(ref value)++;
					break;
				case TypeCode.Int16:
					Unsafe.As<T, short>(ref value)++;
					break;
				case TypeCode.UInt16:
					Unsafe.As<T, ushort>(ref value)++;
					break;
				case TypeCode.Int32:
					Unsafe.As<T, int>(ref value)++;
					break;
				case TypeCode.UInt32:
					Unsafe.As<T, uint>(ref value)++;
					break;
				case TypeCode.Int64:
					Unsafe.As<T, long>(ref value)++;
					break;
				case TypeCode.UInt64:
					Unsafe.As<T, ulong>(ref value)++;
					break;
			}
		}

		public void operator --()
		{
			switch (Type.GetTypeCode(typeof(T)))
			{
				case TypeCode.Byte:
					Unsafe.As<T, byte>(ref value)--;
					break;
				case TypeCode.SByte:
					Unsafe.As<T, sbyte>(ref value)--;
					break;
				case TypeCode.Int16:
					Unsafe.As<T, short>(ref value)--;
					break;
				case TypeCode.UInt16:
					Unsafe.As<T, ushort>(ref value)--;
					break;
				case TypeCode.Int32:
					Unsafe.As<T, int>(ref value)--;
					break;
				case TypeCode.UInt32:
					Unsafe.As<T, uint>(ref value)--;
					break;
				case TypeCode.Int64:
					Unsafe.As<T, long>(ref value)--;
					break;
				case TypeCode.UInt64:
					Unsafe.As<T, ulong>(ref value)--;
					break;
			}
		}
	}
#endif

	public static T AddFlags<T>(this T value, T flags)
		where T : struct, Enum
		=> value | flags;

	public static T RemoveFlags<T>(this T value, T flags)
		where T : struct, Enum
		=> value & ~flags;

	public static T Validate<T>(this T value)
		where T : struct, Enum
	{
		Validator.ValidateEnum(value);
		return value;
	}

	public static bool Equals<T>(this T value, T other)
		where T : struct, Enum
		=> EqualityComparer<T>.Default.Equals(value, other);

	public static int CompareTo<T>(this T value, T other)
		where T : struct, Enum
		=> Comparer<T>.Default.Compare(value, other);

	public static TAs As<T, TAs>(this T value)
		where T : struct, Enum
		where TAs : unmanaged
#if NET5_0_OR_GREATER
			, IBinaryInteger<TAs>
#endif
	{
		Type underlyingType = typeof(T).GetEnumUnderlyingType();

		return Type.GetTypeCode(typeof(TAs)) switch
		{
			// Using this lambda syntax over a delegate taking object in case .net eventually adds a generic overload for IConvertible
			TypeCode.Byte   => ConvertValue(() => Convert.ToByte(value)),
			TypeCode.SByte  => ConvertValue(() => Convert.ToSByte(value)),
			TypeCode.Int16  => ConvertValue(() => Convert.ToInt16(value)),
			TypeCode.UInt16 => ConvertValue(() => Convert.ToUInt16(value)),
			TypeCode.Int32  => ConvertValue(() => Convert.ToInt32(value)),
			TypeCode.UInt32 => ConvertValue(() => Convert.ToUInt32(value)),
			TypeCode.Int64  => ConvertValue(() => Convert.ToInt64(value)),
			TypeCode.UInt64 => ConvertValue(() => Convert.ToUInt64(value)),
			_ => throw new NotSupportedException($"Target type {typeof(TAs)} is not supported."),
		};

		TAs ConvertValue<TTarget>(Func<TTarget> convert)
		{
			if (typeof(TTarget) == underlyingType)
				return Unsafe.As<T, TAs>(ref value);

			TTarget targetValue = convert();
			return Unsafe.As<TTarget, TAs>(ref targetValue);
		}
	}
}
