using System.Linq;
using System.Reflection;

namespace ChartTools.IO.Chart.Serializers
{
    internal class MetadataSerializer : ChartSerializer<Metadata>
    {
        public MetadataSerializer(string header, Metadata content) : base(header, content) { }

        protected override void GenerateLines()
        {
            if (Content is null)
                return;

            if (Content.Title is not null)
                preResult!.Add(GetLine("Name", $"\"{Content.Title}\""));
            if (Content.Artist is not null)
                preResult!.Add(GetLine("Artist", $"\"{Content.Artist}\""));
            if (Content.Charter is not null && Content.Charter.Name is not null)
                preResult!.Add(GetLine("Charter", $"\"{Content.Charter.Name}\""));
            if (Content.Album is not null)
                preResult!.Add(GetLine("Album", $"\"{Content.Album}\""));
            if (Content.Year is not null)
                preResult!.Add(GetLine("Year", $"\", {Content.Year}\""));
            if (Content.AudioOffset is not null)
                preResult!.Add(GetLine("Offset", Content.AudioOffset.ToString()));
            if (Content.Resolution is not null)
                preResult!.Add(GetLine("Resolution", Content.Resolution.ToString()));
            if (Content.Difficulty is not null)
                preResult!.Add(GetLine("Difficulty", Content.Difficulty.ToString()));
            if (Content.PreviewStart is not null)
                preResult!.Add(GetLine("PreviewStart", Content.PreviewStart.ToString()));
            if (Content.PreviewEnd is not null)
                preResult!.Add(GetLine("PreviewEnd", Content.PreviewEnd.ToString()));
            if (Content.Genre is not null)
                preResult!.Add(GetLine("Genre", $"\"{Content.Genre}\""));
            if (Content.MediaType is not null)
                preResult!.Add(GetLine("MetiaType", $"\"{Content.MediaType}\""));

            // Audio streams
            if (Content.Streams is not null)
                foreach (PropertyInfo property in typeof(StreamCollection).GetProperties())
                {
                    string value = (string)property.GetValue(Content.Streams)!;

                    if (value is not null)
                        preResult!.Add(GetLine($"{property.Name}Stream", $"\"{value}\""));
                }

            if (Content.UnidentifiedData is not null)
                foreach (var data in Content.UnidentifiedData.Where(d => d.Origin == FileFormat.Chart))
                    preResult!.Add(GetLine(data.Key, data.Data));
        }
    }
}
