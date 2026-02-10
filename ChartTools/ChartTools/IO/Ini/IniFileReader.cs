#if !NET9_0_OR_GREATER
using static ChartTools.Extensions.MemoryExtensions;
#endif

using ChartTools.IO.Parsing;
using ChartTools.IO.Sources;
using ChartTools.Meta;

namespace ChartTools.IO.Ini;

internal class IniFileReader(ReadingDataSource source, Metadata? existing) : TextFileReader(source)
{
	public override IEnumerable<IniParser> Parsers
		=> base.Parsers.Cast<IniParser>();

	protected override TextParser? GetParser(in ReadOnlyMemory<char> header)
		=> header.Span.Equals(IniFormatting.Header, StringComparison.OrdinalIgnoreCase)
			? new IniParser(existing) : null;

	protected override bool IsSectionStart(in ReadOnlySpan<char> line)
		=> !line.StartsWith('[');
}
