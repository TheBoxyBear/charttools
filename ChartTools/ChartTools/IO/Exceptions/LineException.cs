namespace ChartTools.IO;

/// <summary>
/// <see cref="Exception"/> thrown when a line of a text file cannot be parsed correctly.
/// </summary>
/// <param name="line">Invalid line string</param>
/// <param name="innerException">Error with the line</param>
public class LineException(string line, Exception innerException) : FormatException($"Line \"{line}\" {innerException.Message}", innerException)
{
	/// <summary>
	/// Line that caused the exception
	/// </summary>
	public string Line { get; } = line;
}
