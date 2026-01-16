using ChartTools.Events;
using ChartTools.IO.Chart.Configuration.Sessions;
using ChartTools.IO.Chart.Entries;

namespace ChartTools.IO.Chart.Parsing;

internal class GlobalEventParser(ChartReadingSession session)
	: ChartParser(session, ChartFormatting.GlobalEventHeader.AsMemory())
{
	public override List<GlobalEvent> Result
		=> GetResult(result);
	private readonly List<GlobalEvent> result = [];

	protected override void HandleItem(in ReadOnlyMemory<char> line)
	{
		TrackObjectEntry entry = new(line);
		result.Add(new(entry.Position, entry.Data.Trim('"').ToString()));
	}

	public override void ApplyToSong(Song song)
		=> song.GlobalEvents = Result;
}
