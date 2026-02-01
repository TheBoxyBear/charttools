using ChartTools.IO.Chart.Configuration.Sessions;
using ChartTools.IO.Sections;

namespace ChartTools.IO.Chart.Parsing;

internal class UnknownSectionParser(ChartReadingSession session, in ReadOnlyMemory<char> header)
	: ChartParser(session, in header)
{
	public override Section<string> Result
	=> GetResult(m_result);

	private readonly Section<string> m_result = new(header.ToString());

	public override void ApplyToSong(Song song)
		=> (song.UnknownChartSections ??= []).Add(Result);

	protected override void HandleItem(in ReadOnlyMemory<char> item)
		=> m_result.Add(item.ToString());
}
