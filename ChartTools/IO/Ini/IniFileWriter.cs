namespace ChartTools.IO.Ini;

internal class IniFileWriter(string path, params Serializer<string>[] serializers)
    : TextFileWriter(path, Enumerable.Empty<string>(), serializers)
{
    protected override bool EndReplace(string line) => line.StartsWith('[');
}
