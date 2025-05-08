namespace ChartTools.IO;

public class LineException(string line, Exception innerException) : FormatException($"Line \"{line}\" {innerException.Message}", innerException)
{
	public string Line { get; } = line;
}
