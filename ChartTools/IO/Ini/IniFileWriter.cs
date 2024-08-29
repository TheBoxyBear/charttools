using ChartTools.IO.Sources;

namespace ChartTools.IO.Ini;

internal class IniFileWriter(WritingDataSource source, params Serializer<string>[] serializers)
    : TextFileWriter(source, [], serializers)
{
    protected override bool EndReplace(string line) => line.StartsWith('[');
}
