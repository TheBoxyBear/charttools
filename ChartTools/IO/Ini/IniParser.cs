using ChartTools.IO.Parsing;
using ChartTools.Meta;
using ChartTools.Meta.Mapping;
using ChartTools.Tools;

namespace ChartTools.IO.Ini;

internal class IniParser(Metadata? existing = null)
	: TextParser(IniFormatting.Header.AsMemory()), ISongAppliable
{
	public override Metadata Result => GetResult(result);
	private readonly Metadata result = existing ?? new();

	protected override void HandleItem(in ReadOnlyMemory<char> line)
	{
		TextEntry entry = new(line);
		MetadataIniMapper.Shared.Set(result, entry.Key.Span, entry.Value.Span.Trim('"'));
	}

	protected override Exception GetHandleException(in ReadOnlyMemory<char> item, Exception innerException)
		=> new SectionException(IniFormatting.Header, GetHandleInnerException(item, innerException));

	public void ApplyToSong(Song song)
	{
		if (song.Metadata is null)
			song.Metadata = Result;
		else
			PropertyMerger.Merge(song.Metadata, false, true, Result);
	}
}
