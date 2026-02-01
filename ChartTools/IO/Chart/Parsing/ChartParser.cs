using ChartTools.IO.Chart.Configuration.Sessions;

namespace ChartTools.IO.Chart.Parsing;

internal abstract class ChartParser(ChartReadingSession session, in ReadOnlyMemory<char> header)
	: TextParser(header), ISongAppliable
{
	public ChartReadingSession Session { get; } = session;

	public abstract void ApplyToSong(Song song);

	protected override Exception GetHandleException(in ReadOnlyMemory<char> item, Exception innerException)
		=> new SectionException(Header.ToString(), GetHandleInnerException(item, innerException));
}
