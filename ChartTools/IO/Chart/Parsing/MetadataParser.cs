using ChartTools.Meta;
using ChartTools.Meta.Mapping;

namespace ChartTools.IO.Chart.Parsing;

internal class MetadataParser(Metadata? existing = null)
	: ChartParser(null! /* Session not used */, ChartFormatting.MetadataHeader.AsMemory())
{
	public override Metadata Result => GetResult(result);
	private readonly Metadata result = existing ?? new();

	protected override void HandleItem(in ReadOnlyMemory<char> line)
	{
		TextEntry entry = new(line);
		MetadataChartMapper.Set(result, entry.Key.Span, entry.Value.Span);
	}

	public override void ApplyToSong(Song song) => song.Metadata = Result;
}
