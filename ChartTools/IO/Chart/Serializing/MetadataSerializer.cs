using ChartTools.IO.Serializing;
using ChartTools.Meta;
using ChartTools.Meta.Mapping;

namespace ChartTools.IO.Chart.Serializing;

internal class MetadataSerializer(Metadata content)
	: Serializer<Metadata, string>(ChartFormatting.MetadataHeader, content)
{
	public override IEnumerable<string> Serialize()
		=> MetadataChartMapper.Shared.GetAll(Content).Select(static entry
			=> ChartFormatting.Line(entry.Key.Span, $"\"{entry.Value}\""));
}
