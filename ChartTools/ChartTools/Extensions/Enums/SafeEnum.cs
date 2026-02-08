using ChartTools.IO;

using System.ComponentModel;
using System.Numerics;

namespace ChartTools.Extensions.Enums;

/// <summary>
/// Wrapper struct for enum values that automatically validates on set.
/// </summary>
/// <typeparam name="T">Enum type to wrap</typeparam>
public record struct SafeEnum<T> : IEnumWrapper<SafeEnum<T>, T>,
	IEquatable<T>, IEquatable<T?>,
	IComparable<T>,
	IEqualityOperators<SafeEnum<T>, T, bool>, IEqualityOperators<SafeEnum<T>, T?, bool>,
	IComparisonOperators<SafeEnum<T>, T, bool>,
	IBitwiseOperators<SafeEnum<T>, T, T>
	where T : struct, Enum
{
	/// <summary>
	/// Gets or sets the enum value with validation on set (field-backed property).
	/// </summary>
	public T Value
	{
		readonly get => field;
		set
		{
			Validator.ValidateEnum(value);
			field = value;
		}
	}

	/// <summary>
	/// Initializes a new instance with enum value post validation.
	/// </summary>
	public SafeEnum(T value)
	{
		Validator.ValidateEnum(value);
		Value = value;
	}

	/// <summary>
	/// Gets HashCode of the enum value.
	/// </summary>
	public override readonly int GetHashCode()
		=> Value.GetHashCode();

	/// <summary>
	/// ToString() implementation of SafeEnum.
	/// </summary>
	public override readonly string ToString()
		=> Value.ToString();

	public bool Equals(SafeEnum<T>? nullable)
		=> nullable is T value && Value == value;

	public bool Equals(T other)
		=> Value == other;

	public bool Equals(T? nullable)
		=> nullable is T value && Value == value;

	public int CompareTo(SafeEnum<T> other)
		=> Value.CompareTo<T>(other.Value);

	public int CompareTo(T other)
		=> Value.CompareTo<T>(other);

	static SafeEnum<T> IParsable<SafeEnum<T>>.Parse(string s, IFormatProvider? _)
		=> Enum.Parse<T>(s);

	static bool IParsable<SafeEnum<T>>.TryParse(string s, IFormatProvider? _, out SafeEnum<T> result)
		=> Enum.TryParse(s, out result);

	static SafeEnum<T> ISpanParsable<SafeEnum<T>>.Parse(ReadOnlySpan<char> s, IFormatProvider? _)
		=> Enum.Parse<T>(s);

	static bool ISpanParsable<SafeEnum<T>>.TryParse(ReadOnlySpan<char> s, IFormatProvider? _, out SafeEnum<T> result)
		=> Enum.TryParse(s, out result);

	public static implicit operator T(SafeEnum<T> wrapper)
		=> wrapper.Value;

	public static implicit operator SafeEnum<T>(T value)
		=> new(value);

	#region Operators
	public static bool operator ==(SafeEnum<T> left, SafeEnum<T>? right)
		=> right.HasValue && left == right.Value;

	public static bool operator ==(SafeEnum<T> left, T right)
		=> left.Value == right;

	public static bool operator ==(SafeEnum<T> left, T? right)
		=> right.HasValue && left.Value == right.Value;

	public static bool operator !=(SafeEnum<T> left, SafeEnum<T>? right)
		=> right.HasValue && left != right.Value;

	public static bool operator !=(SafeEnum<T> left, T right)
		=> left.Value != right;

	public static bool operator !=(SafeEnum<T> left, T? right)
		=> right.HasValue && left.Value != right.Value;

	public static bool operator >(SafeEnum<T> left, SafeEnum<T> right)
	=> left.Value > right.Value;

	public static bool operator >(SafeEnum<T> left, SafeEnum<T>? right)
		=> right.HasValue && left > right.Value;

	public static bool operator >(SafeEnum<T> left, T right)
		=> left.Value > right;

	public static bool operator >(SafeEnum<T> left, T? right)
		=> right.HasValue && left.Value > right.Value;

	public static bool operator >=(SafeEnum<T> left, SafeEnum<T> right)
		=> left.Value >= right.Value;

	public static bool operator >=(SafeEnum<T> left, SafeEnum<T>? right)
		=> right.HasValue && left >= right.Value;

	public static bool operator >=(SafeEnum<T> left, T right)
		=> left.Value >= right;

	public static bool operator >=(SafeEnum<T> left, T? right)
		=> right.HasValue && left.Value >= right.Value;

	public static bool operator <(SafeEnum<T> left, SafeEnum<T> right)
		=> left.Value < right.Value;

	public static bool operator <(SafeEnum<T> left, SafeEnum<T>? right)
		=> right.HasValue && left < right.Value;

	public static bool operator <(SafeEnum<T> left, T right)
		=> left.Value < right;

	public static bool operator <(SafeEnum<T> left, T? right)
		=> right.HasValue && left.Value < right.Value;

	public static bool operator <=(SafeEnum<T> left, SafeEnum<T> right)
		=> left.Value <= right.Value;

	public static bool operator <=(SafeEnum<T> left, SafeEnum<T>? right)
		=> right.HasValue && left <= right.Value;

	public static bool operator <=(SafeEnum<T> left, T right)
		=> left.Value <= right;

	public static bool operator <=(SafeEnum<T> left, T? right)
		=> right.HasValue && left.Value <= right.Value;

	public static T operator ~(SafeEnum<T> value)
		=> ~value.Value;

	public static T operator <<(SafeEnum<T> value, int shiftAmount)
		=> value.Value << shiftAmount;

	public static T operator >>(SafeEnum<T> value, int shiftAmount)
		=> value.Value >> shiftAmount;

	public static T operator >>>(SafeEnum<T> value, int shiftAmount)
		=> value.Value >>> shiftAmount;

	public static T operator &(SafeEnum<T> left, SafeEnum<T> right)
		=> left.Value & right.Value;

	public static T operator &(SafeEnum<T> left, T right)
		=> left.Value & right;

	public static T operator |(SafeEnum<T> left, SafeEnum<T> right)
		=> left.Value | right.Value;

	public static T operator |(SafeEnum<T> left, T right)
		=> left.Value | right;

	public static T operator ^(SafeEnum<T> left, SafeEnum<T> right)
		=> left.Value ^ right.Value;

	public static T operator ^(SafeEnum<T> left, T right)
		=> left.Value ^ right;
	#endregion
}

[EditorBrowsable(EditorBrowsableState.Never)]
public static class SafeEnumExtenxions
{
	extension<T1, T2>(SafeEnum<T1>)
	where T1 : struct, Enum
	where T2 : unmanaged, IBinaryInteger<T2>
	{
		public static T1 operator +(SafeEnum<T1> left, T2 right)
			=> left.Value + right;

		public static T1 operator -(SafeEnum<T1> left, T2 right)
			=> left.Value - right;
	}
}
