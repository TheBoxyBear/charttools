using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ChartTools.IO.Chart.Serializers
{
    internal class MetadataSerializer : ChartSerializer<Metadata>
    {
        public MetadataSerializer(Metadata content) : base("[Metadata]", content) { }

        protected override IEnumerable<string> GenerateLines()
        {
            if (Content is null)
                yield break;

            if (Content.Title is not null)
                yield return GetLine("Name", $"\"{Content.Title}\"");
            if (Content.Artist is not null)
                yield return GetLine("Artist", $"\"{Content.Artist}\"");
            if (Content.Charter is not null && Content.Charter.Name is not null)
                yield return GetLine("Charter", $"\"{Content.Charter.Name}\"");
            if (Content.Album is not null)
                yield return GetLine("Album", $"\"{Content.Album}\"");
            if (Content.Year is not null)
                yield return GetLine("Year", $"\", {Content.Year}\"");
            if (Content.AudioOffset is not null)
                yield return GetLine("Offset", Content.AudioOffset.ToString());
            if (Content.Resolution is not null)
                yield return GetLine("Resolution", Content.Resolution.ToString());
            if (Content.Difficulty is not null)
                yield return GetLine("Difficulty", Content.Difficulty.ToString());
            if (Content.PreviewStart is not null)
                yield return GetLine("PreviewStart", Content.PreviewStart.ToString());
            if (Content.PreviewEnd is not null)
                yield return GetLine("PreviewEnd", Content.PreviewEnd.ToString());
            if (Content.Genre is not null)
                yield return GetLine("Genre", $"\"{Content.Genre}\"");
            if (Content.MediaType is not null)
                yield return GetLine("MetiaType", $"\"{Content.MediaType}\"");

            // Audio streams
            if (Content.Streams is not null)
                foreach (PropertyInfo property in typeof(StreamCollection).GetProperties())
                {
                    string value = (string)property.GetValue(Content.Streams)!;

                    if (value is not null)
                        yield return GetLine($"{property.Name}Stream", $"\"{value}\"");
                }

            if (Content.UnidentifiedData is not null)
                foreach (var data in Content.UnidentifiedData.Where(d => d.Origin == FileFormat.Chart))
                    yield return (GetLine(data.Key, data.Data));
        }
    }
}
