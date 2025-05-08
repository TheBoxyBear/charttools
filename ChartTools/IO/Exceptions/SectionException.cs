namespace ChartTools.IO;

public class SectionException(string header, Exception innerException) : Exception($"Section \"{header}\" {innerException.Message}")
{
	public string Header { get; } = header;

	public static SectionException EarlyEnd(string header) => new(header, new InvalidDataException("Section did not end within the provided lines"));

	public static SectionException MissingRequired(string header) => new(header, new InvalidDataException("Required section could not be found."));
}
