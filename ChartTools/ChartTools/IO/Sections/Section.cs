namespace ChartTools.IO.Sections;

public class Section<T>(string header) : List<T>
{
	public string Header { get; } = header;
}
