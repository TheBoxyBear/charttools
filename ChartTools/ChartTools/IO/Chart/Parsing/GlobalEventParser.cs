#if !NETCOREAPP3_0_OR_GREATER
using static ChartTools.Extensions.MemoryExtensions;
#endif

using ChartTools.Events;
using ChartTools.IO.Chart.Configuration.Sessions;
using ChartTools.IO.Chart.Entries;

namespace ChartTools.IO.Chart.Parsing;

internal class GlobalEventParser(ChartReadingSession session)
	: ChartParser(session, ChartFormatting.GlobalEventHeader.AsMemory())
{
	public override List<GlobalEvent> Result
		=> GetResult(m_result);

	private readonly List<GlobalEvent> m_result = [];

	protected override void HandleItem(in ReadOnlyMemory<char> line)
	{
		TrackObjectEntry entry = new(line);
		m_result.Add(new(entry.Position, entry.Data.Trim('"').ToString()));
	}

	public override void ApplyToSong(Song song)
		=> song.GlobalEvents = Result;
}
