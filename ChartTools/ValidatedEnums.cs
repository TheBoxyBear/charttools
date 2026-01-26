namespace ChartTools;

/// <summary>
/// Wrapper struct for enum values that automatically validates on set.
/// </summary>
/// <typeparam name="T">enum type to wrap</typeparam>
public readonly struct ValidatedEnum<T> : IEquatable<ValidatedEnum<T>> where T : struct, Enum
{
    private readonly T _value;

    /// <summary>
    /// Gets or sets the enum value with validation on set
    /// </summary>
    public T Value
    {
        get => _value;
        init
        {
            Validator.ValidateEnum(value);
            _value = value;
        }
    }

    /// <summary>
    /// Initializes a new instance with enum value post validation
    /// </summary>
    public ValidatedEnum(T value)
    {
        Validator.ValidateEnum(value);
        _value = value;
    }

    /// <summary>
    /// Implicit operator for conversion from T ValidatedEnum (Useful for assignments)
    /// </summary>
    public static implicit operator ValidatedEnum<T>(T value) => new(value);

    /// <summary>
    /// Implicit operator for conversion from ValidatedEnum -> T (Useful for dereference/usage)
    /// </summary>
    public static implicit operator T(ValidatedEnum<T> wrapper) => wrapper._value;

    /// <summary>
    /// Determines if two validated enums are equal or not.
    /// </summary>
    public bool Equals(ValidatedEnum<T> other)
    {
        return _value.Equals(other._value);
    }

    /// <summary>
    /// Determines if a generic object is of type ValidatedEnum and enum value is equal to the caller.
    /// </summary>
    public override bool Equals(object? obj)
    {
        return obj is ValidatedEnum<T> other && Equals(other);
    }

    /// <summary>
    /// Gets HashCode of the enum value.
    /// </summary>
	public override int GetHashCode()
    {
        return _value.GetHashCode();
    }

    /// <summary>
    /// Equality operator for ValidatedEnum type.
    /// </summary>
    public static bool operator ==(ValidatedEnum<T> lhs, ValidatedEnum<T> rhs) => lhs.Equals(rhs);

    /// <summary>
    /// Inequality operator for ValidatedEnum type.
    /// </summary>
    public static bool operator !=(ValidatedEnum<T> lhs, ValidatedEnum<T> rhs) => !lhs.Equals(rhs);

    /// <summary>
    /// ToString() implementation of ValidatedEnum.
    /// </summary>
	public override string ToString()
	{
		return _value.ToString();
	}

}