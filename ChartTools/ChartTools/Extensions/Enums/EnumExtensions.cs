#if NET7_0_OR_GREATER
using System.Numerics;
#endif

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ChartTools.Extensions.Enums;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class EnumExtensions
{
	extension<T>(T)
		where T : struct, Enum
	{
		public static T Parse(string value, bool ignoreCase = false)
			=> Enum.Parse<T>(value, ignoreCase);

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
			=> Enum.IsDefined(value);

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
				TypeCode.Byte   => Unsafe.As<int, T>(Unsafe.As<T, byte>(left)    | Unsafe.As<T, byte>(right)),
				TypeCode.SByte  => Unsafe.As<int, T>(Unsafe.As<T, sbyte>(left)   | Unsafe.As<T, sbyte>(right)),
				TypeCode.Int16  => Unsafe.As<int, T>(Unsafe.As<T, short>(left)   | Unsafe.As<T, short>(right)),
				TypeCode.UInt16 => Unsafe.As<int, T>(Unsafe.As<T, ushort>(left)  | Unsafe.As<T, ushort>(right)),
				TypeCode.Int32  => Unsafe.As<int, T>(Unsafe.As<T, int>(left)	 | Unsafe.As<T, int>(right)),
				TypeCode.UInt32 => Unsafe.As<uint, T>(Unsafe.As<T, uint>(left)   | Unsafe.As<T, uint>(right)),
				TypeCode.Int64  => Unsafe.As<long, T>(Unsafe.As<T, long>(left)   | Unsafe.As<T, long>(right)),
				TypeCode.UInt64 => Unsafe.As<ulong, T>(Unsafe.As<T, ulong>(left) | Unsafe.As<T, ulong>(right))
			};

		public static T operator &(T left, T right)
			=> EnumCache<T>.UnderlyingTypeCode switch
			{
				TypeCode.Byte   => Unsafe.As<int, T>(Unsafe.As<T, byte>(left)	 & Unsafe.As<T, byte>(right)),
				TypeCode.SByte  => Unsafe.As<int, T>(Unsafe.As<T, sbyte>(left)   & Unsafe.As<T, sbyte>(right)),
				TypeCode.Int16  => Unsafe.As<int, T>(Unsafe.As<T, short>(left)   & Unsafe.As<T, short>(right)),
				TypeCode.UInt16 => Unsafe.As<int, T>(Unsafe.As<T, ushort>(left)  & Unsafe.As<T, ushort>(right)),
				TypeCode.Int32  => Unsafe.As<int, T>(Unsafe.As<T, int>(left)	 & Unsafe.As<T, int>(right)),
				TypeCode.UInt32 => Unsafe.As<uint, T>(Unsafe.As<T, uint>(left)   & Unsafe.As<T, uint>(right)),
				TypeCode.Int64  => Unsafe.As<long, T>(Unsafe.As<T, long>(left)   & Unsafe.As<T, long>(right)),
				TypeCode.UInt64 => Unsafe.As<ulong, T>(Unsafe.As<T, ulong>(left) & Unsafe.As<T, ulong>(right))
			};

		public static T operator ^(T left, T right)
			=> EnumCache<T>.UnderlyingTypeCode switch
			{
				TypeCode.Byte   => Unsafe.As<int, T>(Unsafe.As<T, byte>(left)    ^ Unsafe.As<T, byte>(right)),
				TypeCode.SByte  => Unsafe.As<int, T>(Unsafe.As<T, sbyte>(left)   ^ Unsafe.As<T, sbyte>(right)),
				TypeCode.Int16  => Unsafe.As<int, T>(Unsafe.As<T, short>(left)   ^ Unsafe.As<T, short>(right)),
				TypeCode.UInt16 => Unsafe.As<int, T>(Unsafe.As<T, ushort>(left)  ^ Unsafe.As<T, ushort>(right)),
				TypeCode.Int32  => Unsafe.As<int, T>(Unsafe.As<T, int>(left)	 ^ Unsafe.As<T, int>(right)),
				TypeCode.UInt32 => Unsafe.As<uint, T>(Unsafe.As<T, uint>(left)   ^ Unsafe.As<T, uint>(right)),
				TypeCode.Int64  => Unsafe.As<long, T>(Unsafe.As<T, long>(left)   ^ Unsafe.As<T, long>(right)),
				TypeCode.UInt64 => Unsafe.As<ulong, T>(Unsafe.As<T, ulong>(left) ^ Unsafe.As<T, ulong>(right)),
			};

		public static T operator ~(T value)
			=> EnumCache<T>.UnderlyingTypeCode switch
			{
				TypeCode.Byte   => Unsafe.As<int, T>(~Unsafe.As<T, byte>(value)),
				TypeCode.SByte  => Unsafe.As<int, T>(~Unsafe.As<T, sbyte>(value)),
				TypeCode.Int16  => Unsafe.As<int, T>(~Unsafe.As<T, short>(value)),
				TypeCode.UInt16 => Unsafe.As<int, T>(~Unsafe.As<T, ushort>(value)),
				TypeCode.Int32  => Unsafe.As<int, T>(~Unsafe.As<T, int>(value)),
				TypeCode.UInt32 => Unsafe.As<uint, T>(~Unsafe.As<T, uint>(value)),
				TypeCode.Int64  => Unsafe.As<long, T>(~Unsafe.As<T, long>(value)),
				TypeCode.UInt64 => Unsafe.As<ulong, T>(~Unsafe.As<T, ulong>(value)),
			};

		public static T operator <<(T left, int shift)
			=> EnumCache<T>.UnderlyingTypeCode switch
			{
				TypeCode.Byte   => Unsafe.As<int, T>(Unsafe.As<T, byte>(left)	 << shift),
				TypeCode.SByte  => Unsafe.As<int, T>(Unsafe.As<T, sbyte>(left)   << shift),
				TypeCode.Int16  => Unsafe.As<int, T>(Unsafe.As<T, short>(left)   << shift),
				TypeCode.UInt16 => Unsafe.As<int, T>(Unsafe.As<T, ushort>(left)  << shift),
				TypeCode.Int32  => Unsafe.As<int, T>(Unsafe.As<T, int>(left)	 << shift),
				TypeCode.UInt32 => Unsafe.As<uint, T>(Unsafe.As<T, uint>(left)   << shift),
				TypeCode.Int64  => Unsafe.As<long, T>(Unsafe.As<T, long>(left)   << shift),
				TypeCode.UInt64 => Unsafe.As<ulong, T>(Unsafe.As<T, ulong>(left) << shift),
			};

		public static T operator >>(T left, int shift)
			=> EnumCache<T>.UnderlyingTypeCode switch
			{
				TypeCode.Byte   => Unsafe.As<int, T>(Unsafe.As<T, byte>(left)    >> shift),
				TypeCode.SByte  => Unsafe.As<int, T>(Unsafe.As<T, sbyte>(left)   >> shift),
				TypeCode.Int16  => Unsafe.As<int, T>(Unsafe.As<T, short>(left)   >> shift),
				TypeCode.UInt16 => Unsafe.As<int, T>(Unsafe.As<T, ushort>(left)  >> shift),
				TypeCode.Int32  => Unsafe.As<int, T>(Unsafe.As<T, int>(left)     >> shift),
				TypeCode.UInt32 => Unsafe.As<uint, T>(Unsafe.As<T, uint>(left)   >> shift),
				TypeCode.Int64  => Unsafe.As<long, T>(Unsafe.As<T, long>(left)   >> shift),
				TypeCode.UInt64 => Unsafe.As<ulong, T>(Unsafe.As<T, ulong>(left) >> shift),
			};

		public static T operator >>>(T left, int shift)
			=> EnumCache<T>.UnderlyingTypeCode switch
			{
				TypeCode.Byte   => Unsafe.As<int, T>(Unsafe.As<T, byte>(left)    >>> shift),
				TypeCode.SByte  => Unsafe.As<int, T>(Unsafe.As<T, sbyte>(left)   >>> shift),
				TypeCode.Int16  => Unsafe.As<int, T>(Unsafe.As<T, short>(left)   >>> shift),
				TypeCode.UInt16 => Unsafe.As<int, T>(Unsafe.As<T, ushort>(left)  >>> shift),
				TypeCode.Int32  => Unsafe.As<int, T>(Unsafe.As<T, int>(left)	 >>> shift),
				TypeCode.UInt32 => Unsafe.As<uint, T>(Unsafe.As<T, uint>(left)   >>> shift),
				TypeCode.Int64  => Unsafe.As<long, T>(Unsafe.As<T, long>(left)   >>> shift),
				TypeCode.UInt64 => Unsafe.As<ulong, T>(Unsafe.As<T, ulong>(left) >>> shift),
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
			return Unsafe.As<T2, T1>(value);
#else
			return Type.GetTypeCode(typeof(T2)) switch
			{
				TypeCode.Byte   => Unsafe.As<int, T1>(Unsafe.As<T1, byte>(left)	   + Unsafe.As<T2, byte>(right)),
				TypeCode.SByte  => Unsafe.As<int, T1>(Unsafe.As<T1, sbyte>(left)   + Unsafe.As<T2, sbyte>(right)),
				TypeCode.Int16  => Unsafe.As<int, T1>(Unsafe.As<T1, short>(left)   + Unsafe.As<T2, short>(right)),
				TypeCode.UInt16 => Unsafe.As<int, T1>(Unsafe.As<T1, ushort>(left)  + Unsafe.As<T2, ushort>(right)),
				TypeCode.Int32  => Unsafe.As<int, T1>(Unsafe.As<T1, int>(left)	   + Unsafe.As<T2, int>(right)),
				TypeCode.UInt32 => Unsafe.As<uint, T1>(Unsafe.As<T1, uint>(left)   + Unsafe.As<T2, uint>(right)),
				TypeCode.Int64  => Unsafe.As<long, T1>(Unsafe.As<T1, long>(left)   + Unsafe.As<T2, long>(right)),
				TypeCode.UInt64 => Unsafe.As<ulong, T1>(Unsafe.As<T1, ulong>(left) + Unsafe.As<T2, ulong>(right)),
				_ => throw new NotSupportedException($"Underlying type of {typeof(T1)} is not supported.")
			};
#endif
		}

		public static T1 operator -(T1 left, T2 right)
		{
#if NET5_0_OR_GREATER
			T2 value = left.As<T1, T2>() - right;
			return Unsafe.As<T2, T1>(value);
#else
			return Type.GetTypeCode(typeof(T2)) switch
			{
				TypeCode.Byte   => Unsafe.As<int, T1>(Unsafe.As<T1, byte>(left)	   - Unsafe.As<T2, byte>(right)),
				TypeCode.SByte  => Unsafe.As<int, T1>(Unsafe.As<T1, sbyte>(left)   - Unsafe.As<T2, sbyte>(right)),
				TypeCode.Int16  => Unsafe.As<int, T1>(Unsafe.As<T1, short>(left)   - Unsafe.As<T2, short>(right)),
				TypeCode.UInt16 => Unsafe.As<int, T1>(Unsafe.As<T1, ushort>(left)  - Unsafe.As<T2, ushort>(right)),
				TypeCode.Int32  => Unsafe.As<int, T1>(Unsafe.As<T1, int>(left)	   - Unsafe.As<T2, int>(right)),
				TypeCode.UInt32 => Unsafe.As<uint, T1>(Unsafe.As<T1, uint>(left)   - Unsafe.As<T2, uint>(right)),
				TypeCode.Int64  => Unsafe.As<long, T1>(Unsafe.As<T1, long>(left)   - Unsafe.As<T2, long>(right)),
				TypeCode.UInt64 => Unsafe.As<ulong, T1>(Unsafe.As<T1, ulong>(left) - Unsafe.As<T2, ulong>(right)),
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
		where T : struct, Enum, IConvertible
		where TAs : unmanaged
#if NET5_0_OR_GREATER
			, IBinaryInteger<TAs>
#endif
	{
		return Type.GetTypeCode(typeof(TAs)) switch
		{
			TypeCode.Byte   => Unsafe.As<byte, TAs>(value.ToByte(null)),
			TypeCode.SByte  => Unsafe.As<sbyte, TAs>(value.ToSByte(null)),
			TypeCode.Int16  => Unsafe.As<short, TAs>(value.ToInt16(null)),
			TypeCode.UInt16 => Unsafe.As<ushort, TAs>(value.ToUInt16(null)),
			TypeCode.Int32  => Unsafe.As<int, TAs>(value.ToInt32(null)),
			TypeCode.UInt32 => Unsafe.As<uint, TAs>(value.ToUInt32(null)),
			TypeCode.Int64  => Unsafe.As<long, TAs>(value.ToInt64(null)),
			TypeCode.UInt64 => Unsafe.As<ulong, TAs>(value.ToUInt64(null)),
			_ => throw new NotSupportedException($"Target type {typeof(TAs)} is not supported."),
		};
	}
}
