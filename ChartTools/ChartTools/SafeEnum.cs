using ChartTools.Extensions;

namespace ChartTools;

/// <summary>
/// Wrapper struct for enum values that automatically validates on set.
/// </summary>
/// <typeparam name="T">enum type to wrap</typeparam>
public record struct SafeEnum<T>
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
    /// Implicit operator for conversion from T SafeEnum (Useful for assignments).
    /// </summary>
    public static implicit operator SafeEnum<T>(T value)
		=> new(value);

    /// <summary>
    /// Implicit operator for conversion from SafeEnum -> T (Useful for dereference/usage).
    /// </summary>
    public static implicit operator T(SafeEnum<T> wrapper)
		=> wrapper.Value;

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
}
