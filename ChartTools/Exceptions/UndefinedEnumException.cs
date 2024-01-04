namespace ChartTools;

/// <summary>
/// Exception thrown when using an <see cref="Enum"/> value that is not defined
/// </summary>
public class UndefinedEnumException(Enum value) : ArgumentException($"{value.GetType().Name} \"{value}\" is not defined.")
{
    /// <summary>
    /// Value used
    /// </summary>
    public Enum Value { get; } = value;

    public UndefinedEnumException(Enum value) : base($"{value.GetType().Name} \"{value}\" is not defined.") => Value = value;
}
