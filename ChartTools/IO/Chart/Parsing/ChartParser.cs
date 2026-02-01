using ChartTools.IO.Chart.Configuration.Sessions;
using ChartTools.IO.Parsing;

namespace ChartTools.IO.Chart.Parsing;

internal abstract class ChartParser(ChartReadingSession session, string header)
	: TextParser(header), ISongAppliable
{
	public ChartReadingSession Session { get; } = session;

	public abstract void ApplyToSong(Song song);

	protected override Exception GetHandleException(in ReadOnlyMemory<char> item, Exception innerException)
		=> new SectionException(Header.ToString(), GetHandleInnerException(item, innerException));
}
