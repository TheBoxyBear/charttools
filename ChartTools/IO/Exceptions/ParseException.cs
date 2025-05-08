namespace ChartTools.IO;

public class ParseException(string? obj, string target, Type type) : FormatException($"Cannot convert {target} \"{obj}\" to {type.Name}")
{
	public string? Object { get; } = obj;

	public string Target { get; } = target;

	public Type Type { get; } = type;
}
