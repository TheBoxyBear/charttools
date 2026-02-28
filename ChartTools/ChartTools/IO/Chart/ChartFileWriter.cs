#if !NET8_0_OR_GREATER
using static ChartTools.Extensions.MemoryExtensions;
#endif

using ChartTools.IO.Serializing;
using ChartTools.IO.Sources;

namespace ChartTools.IO.Chart;

internal class ChartFileWriter(
	WritingDataSource source, IEnumerable<string>? removedHeaders, params ReadOnlySpan<Serializer<string>> serializers)
	: TextFileWriter(source, removedHeaders, serializers)
{
	protected override string? PreSerializerContent => "{";

	protected override string? PostSerializerContent => "}";

	protected override bool EndReplace(string line) => line.StartsWith('[');
}
