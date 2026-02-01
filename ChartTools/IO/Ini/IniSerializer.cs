using ChartTools.IO.Formatting;
using ChartTools.IO.Serializing;
using ChartTools.Meta;

namespace ChartTools.IO.Ini;

internal class IniSerializer(Metadata content)
	: Serializer<Metadata, string>(IniFormatting.Header, content)
{
	public override IEnumerable<string> Serialize()
		=> MetadataIniMapper.Shared.GetAll(Content).Select(static entry
			=> IniFormatting.Line(entry.Key.Span, entry.Value.Span));
}
