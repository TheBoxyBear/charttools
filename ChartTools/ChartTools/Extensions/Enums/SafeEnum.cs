namespace ChartTools.Extensions.Enums;

/// <summary>
/// Wrapper struct for enum values that automatically validates on set.
/// </summary>
/// <typeparam name="T">Enum type to wrap</typeparam>
public record struct SafeEnum<T> : IEnumWrapper<SafeEnum<T>>, IEnumWrapper<SafeEnum<T>, T>
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

	public static implicit operator T(SafeEnum<T> wrapper)
		=> wrapper.Value;

	public static implicit operator SafeEnum<T>(T value)
		=> new(value);

	#region Operators
	public static bool operator ==(SafeEnum<T> left, SafeEnum<T>? right)
		=> right.HasValue && left == right.Value;

	public static bool operator !=(SafeEnum<T> left, SafeEnum<T>? right)
		=> right.HasValue && left == right.Value;

	public static bool operator >(SafeEnum<T> left, SafeEnum<T> right)
		=> left.Value > right.Value;

	public static bool operator >=(SafeEnum<T> left, SafeEnum<T> right)
		=> left.Value >= right.Value;

	public static bool operator <(SafeEnum<T> left, SafeEnum<T> right)
		=> left.Value < right.Value;

	public static bool operator <=(SafeEnum<T> left, SafeEnum<T> right)
		=> left.Value <= right.Value;

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

	public static T operator |(SafeEnum<T> left, SafeEnum<T> right)
		=> left.Value | right.Value;

	public static T operator ^(SafeEnum<T> left, SafeEnum<T> right)
		=> left.Value ^ right.Value;
	#endregion
}
