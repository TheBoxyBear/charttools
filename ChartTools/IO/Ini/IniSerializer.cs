using ChartTools.Meta;
using ChartTools.Meta.Mapping;

namespace ChartTools.IO.Ini;

internal class IniSerializer(Metadata content)
	: Serializer<Metadata, string>(IniFormatting.Header, content)
{
	public override IEnumerable<string> Serialize()
		=> MetadataIniMapper.Shared.GetAll(Content).Select(static entry
			=> IniFormatting.Line(entry.Key.Span, entry.Value.Span));
}
