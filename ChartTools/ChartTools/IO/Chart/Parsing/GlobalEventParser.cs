using ChartTools.Events;
using ChartTools.IO.Chart.Configuration.Sessions;
using ChartTools.IO.Chart.Entries;

namespace ChartTools.IO.Chart.Parsing;

internal class GlobalEventParser(ChartReadingSession session)
	: ChartParser(session, ChartFormatting.GlobalEventHeader.AsMemory())
{
#if NET5_0_OR_GREATER
	public override List<GlobalEvent> Result
		=> GetResultIfReady(m_result);
#else
	public new List<GlobalEvent> Result
		=> GetResultIfReady(m_result);

	protected override object? GetResult()
		=> Result;
#endif

	private readonly List<GlobalEvent> m_result = [];

	protected override void HandleItem(in ReadOnlyMemory<char> line)
	{
		TrackObjectEntry entry = new(line);
		m_result.Add(new(entry.Position, entry.Data.Trim('"').ToString()));
	}

	public override void ApplyToSong(Song song)
		=> song.GlobalEvents = Result;
}
