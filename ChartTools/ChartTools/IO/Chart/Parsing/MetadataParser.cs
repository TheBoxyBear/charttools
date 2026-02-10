using ChartTools.Meta;
using ChartTools.Meta.Mapping;

namespace ChartTools.IO.Chart.Parsing;

internal class MetadataParser(Metadata? existing = null)
	: ChartParser(null! /* Session not used */, ChartFormatting.MetadataHeader.AsMemory())
{
#if NET5_0_OR_GREATER
	public override Metadata Result
		=> GetResultIfReady(m_result);
#else
	public new Metadata Result
		=> GetResultIfReady(m_result);

	protected override object? GetResult()
		=> Result;
#endif

	private readonly Metadata m_result = existing ?? new();

	protected override void HandleItem(in ReadOnlyMemory<char> line)
	{
		TextEntry entry = new(line);
		MetadataChartMapper.Shared.Set(m_result, entry.Key.Span, entry.Value.Span.Trim('"'));
	}

	public override void ApplyToSong(Song song)
		=> song.Metadata = Result;
}
