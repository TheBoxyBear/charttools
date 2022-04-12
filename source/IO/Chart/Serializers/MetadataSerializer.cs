using System.Collections.Generic;
using System.Linq;

namespace ChartTools.IO.Chart.Serializers
{
    internal class MetadataSerializer : Serializer<Metadata, string>
    {
        public MetadataSerializer(Metadata content) : base(ChartFormatting.MetadataHeader, content, new(ChartFile.DefaultWriteConfig, content.Formatting)) { }

        public override IEnumerable<string> Serialize()
        {
            if (Content is null)
                yield break;

            var props = ChartKeySerializableAttribute.GetSerializable(Content)
                .Concat(ChartKeySerializableAttribute.GetSerializable(Content.Formatting))
                .Concat(ChartKeySerializableAttribute.GetSerializable(Content.Charter)
                .Concat(ChartKeySerializableAttribute.GetSerializable(Content.InstrumentDifficulties))
                .Concat(ChartKeySerializableAttribute.GetSerializable(Content.Streams)));

            foreach ((var key, var value) in props)
                yield return ChartFormatting.Line(key, value);

            if (Content.Year is not null)
                yield return ChartFormatting.Line("Year", $"\", {Content.Year}\"");

            foreach (var data in Content.UnidentifiedData.Where(d => d.Origin == FileFormat.Chart))
                yield return ChartFormatting.Line(data.Key, data.Value);
        }
    }
}
