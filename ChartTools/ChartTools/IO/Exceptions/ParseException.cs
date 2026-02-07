namespace ChartTools.IO;

/// <summary>
/// <see cref="Exception"/> thrown when a parse error is encountered.
/// </summary>
/// <param name="obj"><inheritdoc cref="Object"/></param>
/// <param name="target"><inheritdoc cref="Target"/></param>
/// <param name="type"><inheritdoc cref="Type"/></param>
public class ParseException(string? obj, string target, Type type)
	: FormatException($"Cannot convert {target} \"{obj}\" to {type.Name}")
{
    /// <summary>
    /// Verbal description of the data being parsed
    /// </summary>
    public string? Object { get; } = obj;

    /// <summary>
    /// Verbal description of the parse destination
    /// </summary>
	public string Target { get; } = target;

    /// <summary>
    /// Target type being parsed to
    /// </summary>
    public Type Type { get; } = type;
}
