using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ChartTools.IO.Chart.Serializers
{
    internal class MetadataSerializer : Serializer<Metadata, string>
    {
        public MetadataSerializer(Metadata content) : base("[Metadata]", content, new(ChartFile.DefaultWriteConfig)) { }

        public override IEnumerable<string> Serialize()
        {
            if (Content is null)
                yield break;

            if (Content.Title is not null)
                yield return ChartFormatting.Line("Name", $"\"{Content.Title}\"");
            if (Content.Artist is not null)
                yield return ChartFormatting.Line("Artist", $"\"{Content.Artist}\"");
            if (Content.Charter is not null && Content.Charter.Name is not null)
                yield return ChartFormatting.Line("Charter", $"\"{Content.Charter.Name}\"");
            if (Content.Album is not null)
                yield return ChartFormatting.Line("Album", $"\"{Content.Album}\"");
            if (Content.Year is not null)
                yield return ChartFormatting.Line("Year", $"\", {Content.Year}\"");
            if (Content.AudioOffset is not null)
                yield return ChartFormatting.Line("Offset", Content.AudioOffset.ToString());
            if (Content.Formatting.Resolution is not null)
                yield return ChartFormatting.Line("Resolution", Content.Formatting.Resolution.ToString());
            if (Content.Difficulty is not null)
                yield return ChartFormatting.Line("Difficulty", Content.Difficulty.ToString());
            if (Content.PreviewStart is not null)
                yield return ChartFormatting.Line("PreviewStart", Content.PreviewStart.ToString());
            if (Content.PreviewEnd is not null)
                yield return ChartFormatting.Line("PreviewEnd", Content.PreviewEnd.ToString());
            if (Content.Genre is not null)
                yield return ChartFormatting.Line("Genre", $"\"{Content.Genre}\"");
            if (Content.MediaType is not null)
                yield return ChartFormatting.Line("MetiaType", $"\"{Content.MediaType}\"");

            // Audio streams
            if (Content.Streams is not null)
                foreach (PropertyInfo property in typeof(StreamCollection).GetProperties())
                {
                    string value = (string)property.GetValue(Content.Streams)!;

                    if (value is not null)
                        yield return ChartFormatting.Line($"{property.Name}Stream", $"\"{value}\"");
                }

            if (Content.UnidentifiedData is not null)
                foreach (var data in Content.UnidentifiedData.Where(d => d.Origin == FileFormat.Chart))
                    yield return ChartFormatting.Line(data.Key, data.Value);
        }
    }
}
