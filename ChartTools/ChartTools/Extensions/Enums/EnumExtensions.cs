using System.Numerics;
using System.Runtime.CompilerServices;

namespace ChartTools.Extensions.Enums;

public static class EnumExtensions
{
	extension<T>(T)
		where T : struct, Enum
	{
		public static T Parse(in ReadOnlySpan<char> value, bool ignoreCase = false)
			=> Enum.Parse<T>(value, ignoreCase);

		public static bool TryParse(in ReadOnlySpan<char> value, out T enumValue, bool ignoreCase = false)
			=> Enum.TryParse(value, ignoreCase, out enumValue);
	}

	extension<T>(T)
		where T : Enum
	{
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

		public static int operator -(T left, T right)
			=> Unsafe.As<T, int>(ref left) - Unsafe.As<T, int>(ref right);

		public static T operator|(T left, T right)
		{
			return Type.GetTypeCode(typeof(T)) switch
			{
				TypeCode.Byte   => Apply<byte>(),
				TypeCode.SByte  => Apply<sbyte>(),
				TypeCode.Int16  => Apply<short>(),
				TypeCode.UInt16 => Apply<ushort>(),
				TypeCode.Int32  => Apply<int>(),
				TypeCode.UInt32 => Apply<uint>(),
				TypeCode.Int64  => Apply<long>(),
				TypeCode.UInt64 => Apply<ulong>(),
			};

			T Apply<TTarget>()
				where TTarget : IBitwiseOperators<TTarget, TTarget, TTarget>
			{
				TTarget result = Unsafe.As<T, TTarget>(ref left) | Unsafe.As<T, TTarget>(ref right);
				return Unsafe.As<TTarget, T>(ref result);
			}
		}

		public static T operator &(T left, T right)
		{
			return Type.GetTypeCode(typeof(T)) switch
			{
				TypeCode.Byte   => Apply<byte>(),
				TypeCode.SByte  => Apply<sbyte>(),
				TypeCode.Int16  => Apply<short>(),
				TypeCode.UInt16 => Apply<ushort>(),
				TypeCode.Int32  => Apply<int>(),
				TypeCode.UInt32 => Apply<uint>(),
				TypeCode.Int64  => Apply<long>(),
				TypeCode.UInt64 => Apply<ulong>(),
			};

			T Apply<TTarget>()
				where TTarget : IBitwiseOperators<TTarget, TTarget, TTarget>
			{
				TTarget result = Unsafe.As<T, TTarget>(ref left) & Unsafe.As<T, TTarget>(ref right);
				return Unsafe.As<TTarget, T>(ref result);
			}
		}

		public static T operator ^(T left, T right)
		{
			return Type.GetTypeCode(typeof(T)) switch
			{
				TypeCode.Byte   => Apply<byte>(),
				TypeCode.SByte  => Apply<sbyte>(),
				TypeCode.Int16  => Apply<short>(),
				TypeCode.UInt16 => Apply<ushort>(),
				TypeCode.Int32  => Apply<int>(),
				TypeCode.UInt32 => Apply<uint>(),
				TypeCode.Int64  => Apply<long>(),
				TypeCode.UInt64 => Apply<ulong>(),
			};

			T Apply<TTarget>()
				where TTarget : IBitwiseOperators<TTarget, TTarget, TTarget>
			{
				TTarget result = Unsafe.As<T, TTarget>(ref left) ^ Unsafe.As<T, TTarget>(ref right);
				return Unsafe.As<TTarget, T>(ref result);
			}
		}

		public static T operator~(T value)
		{
			return Type.GetTypeCode(typeof(T)) switch
			{
				TypeCode.Byte   => Apply<byte>(),
				TypeCode.SByte  => Apply<sbyte>(),
				TypeCode.Int16  => Apply<short>(),
				TypeCode.UInt16 => Apply<ushort>(),
				TypeCode.Int32  => Apply<int>(),
				TypeCode.UInt32 => Apply<uint>(),
				TypeCode.Int64  => Apply<long>(),
				TypeCode.UInt64 => Apply<ulong>(),
			};

			T Apply<TTarget>()
				where TTarget : IBitwiseOperators<TTarget, TTarget, TTarget>
			{
				TTarget result = ~Unsafe.As<T, TTarget>(ref value);
				return Unsafe.As<TTarget, T>(ref result);
			}
		}
	}

	extension<T1, T2>(T1)
		where T1 : Enum
		where T2 : unmanaged, IBinaryInteger<T2>
	{
		public static T1 operator+(T1 left, T2 right)
		{
			T2 value = left.As<T1, T2>() + right;
			return Unsafe.As<T2, T1>(ref value);
		}

		public static T1 operator -(T1 left, T2 right)
		{
			T2 value = left.As<T1, T2>() - right;
			return Unsafe.As<T2, T1>(ref value);
		}

		public static T1 operator *(T1 left, T2 right)
		{
			T2 value = left.As<T1, T2>() + right;
			return Unsafe.As<T2, T1>(ref value);
		}

		public static T1 operator /(T1 left, T2 right)
		{
			T2 value = left.As<T1, T2>() + right;
			return Unsafe.As<T2, T1>(ref value);
		}
	}

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

	public static T AddFlags<T>(this T value, T flags)
		where T : Enum
		=> value | flags;

	public static T RemoveFlags<T>(this T value, T flags)
		where T : Enum
		=> value & ~flags;

	public static T Validate<T>(this T value)
		where T : struct, Enum
	{
		Validator.ValidateEnum(value);
		return value;
	}

	// TODO Add deprecated empty SafeEnum overload to bypass check

	public static bool Equals<T>(this T value, T other)
		where T : Enum
		=> EqualityComparer<T>.Default.Equals(value, other);

	public static int CompareTo<T>(this T value, T other)
		where T : Enum
		=> Comparer<T>.Default.Compare(value, other);

	public static TAs As<T, TAs>(this T value)
		where T : Enum
		where TAs : unmanaged, IBinaryInteger<TAs>
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
