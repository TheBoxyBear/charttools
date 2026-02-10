using ChartTools.IO.Chart.Configuration.Sessions;
using ChartTools.IO.Sections;

namespace ChartTools.IO.Chart.Parsing;

internal class UnknownSectionParser(ChartReadingSession session, in ReadOnlyMemory<char> header)
	: ChartParser(session, in header)
{
#if NET5_0_OR_GREATER
	public override Section<string> Result
		=> GetResultIfReady(m_result);
#else
	public new Section<string> Result
		=> GetResultIfReady(m_result);

	protected override object? GetResult()
		=> Result;
#endif

	private readonly Section<string> m_result = new(header.ToString());

	public override void ApplyToSong(Song song)
		=> (song.UnknownChartSections ??= []).Add(Result);

	protected override void HandleItem(in ReadOnlyMemory<char> item)
		=> m_result.Add(item.ToString());
}
