namespace ChartTools.IO.Sections;

public readonly struct ReservedSectionHeader(string header, string dataSource)
{
	public string Header { get; } = header;

	public string DataSource { get; } = dataSource;
}
