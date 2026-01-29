namespace ChartTools;

/// <summary>
/// Wrapper struct for enum values that automatically validates on set.
/// </summary>
/// <typeparam name="T">enum type to wrap</typeparam>
public readonly struct SafeEnum<T> : IEquatable<SafeEnum<T>> where T : struct, Enum
{
    /// <summary>
    /// Gets or sets the enum value with validation on set (field-backed property).
    /// </summary>
    public T Value
    {
        get => field;
        init
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
    /// Implicit operator for conversion from T SafeEnum (Useful for assignments).
    /// </summary>
    public static implicit operator SafeEnum<T>(T value) => new(value);

    /// <summary>
    /// Implicit operator for conversion from SafeEnum -> T (Useful for dereference/usage).
    /// </summary>
    public static implicit operator T(SafeEnum<T> wrapper) => wrapper.Value;

    /// <summary>
    /// Determines if two validated enums are equal or not.
    /// </summary>
    public bool Equals(SafeEnum<T> other)
    {
        return Value.Equals(other.Value);
    }

    /// <summary>
    /// Determines if a generic object is of type SafeEnum and enum value is equal to the caller.
    /// </summary>
    public override bool Equals(object? obj)
    {
        return obj is SafeEnum<T> other && Equals(other);
    }

    /// <summary>
    /// Gets HashCode of the enum value.
    /// </summary>
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    /// <summary>
    /// Equality operator for SafeEnum type.
    /// </summary>
    public static bool operator ==(SafeEnum<T> lhs, SafeEnum<T> rhs) => lhs.Equals(rhs);

    /// <summary>
    /// Inequality operator for SafeEnum type.
    /// </summary>
    public static bool operator !=(SafeEnum<T> lhs, SafeEnum<T> rhs) => !lhs.Equals(rhs);

    /// <summary>
    /// ToString() implementation of SafeEnum.
    /// </summary>
    public override string ToString()
    {
        return Value.ToString();
    }

}