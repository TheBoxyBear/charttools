namespace ChartTools.IO.Chart;

internal class ChartFileWriter(string path, IEnumerable<string>? removedHeaders, params Serializer<string>[] serializers)
    : TextFileWriter(path, removedHeaders, serializers)
{
    protected override string? PreSerializerContent => "{";
    protected override string? PostSerializerContent => "}";

    protected override bool EndReplace(string line) => line.StartsWith('[');
}
