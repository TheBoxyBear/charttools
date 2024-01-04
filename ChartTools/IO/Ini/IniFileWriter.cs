namespace ChartTools.IO.Ini;

internal class IniFileWriter : TextFileWriter
{
    public IniFileWriter(string path, params Serializer<string>[] serializers) : base(path, Enumerable.Empty<string>(), serializers) { }

    protected override bool EndReplace(string line) => line.StartsWith('[');
}
