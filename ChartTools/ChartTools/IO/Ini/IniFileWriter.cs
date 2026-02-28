#if !NET8_0_OR_GREATER
using static ChartTools.Extensions.MemoryExtensions;
#endif

using ChartTools.IO.Serializing;
using ChartTools.IO.Sources;

namespace ChartTools.IO.Ini;

internal class IniFileWriter(WritingDataSource source, params ReadOnlySpan<Serializer<string>> serializers)
	: TextFileWriter(source, [], serializers)
{
	protected override bool EndReplace(string line)
		=> line.StartsWith('[');
}
