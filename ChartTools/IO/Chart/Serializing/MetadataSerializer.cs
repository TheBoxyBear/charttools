using ChartTools.Meta;

namespace ChartTools.IO.Chart.Serializing;

internal class MetadataSerializer(Metadata content) : Serializer<Metadata, string>(ChartFormatting.MetadataHeader, content)
{
	public override IEnumerable<string> Serialize()
	{
		if (Content is null)
			yield break;

		//IEnumerable<(string key, string value)> props = MetadataChartKeyAttribute.GetSerializable(Content)
		//	.Concat(MetadataChartKeyAttribute.GetSerializable(Content.Formatting))
		//	.Concat(MetadataChartKeyAttribute.GetSerializable(Content.Charter)
		//	.Concat(MetadataChartKeyAttribute.GetSerializable(Content.InstrumentDifficulties))
		//	.Concat(MetadataChartKeyAttribute.GetSerializable(Content.Streams)));

		//foreach ((string key, string value) in props)
		//	yield return ChartFormatting.Line(key, value);

		if (Content.Year is not null)
			yield return ChartFormatting.Line("Year", $"\", {Content.Year}\"");

		foreach (UnidentifiedMetadata data in Content.UnidentifiedData.Where(static d => d.Origin is FileType.Chart))
			yield return ChartFormatting.Line(data.Key, data.Value);
	}
}
