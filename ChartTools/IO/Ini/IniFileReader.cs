using ChartTools.IO.Parsing;
using ChartTools.IO.Sources;

namespace ChartTools.IO.Ini;

internal class IniFileReader(ReadingDataSource source, Metadata? existing) : TextFileReader(source)
{
	public override IEnumerable<IniParser> Parsers => base.Parsers.Cast<IniParser>();

	protected override TextParser? GetParser(in ReadOnlyMemory<char> header)
		=> header.Span.Equals(IniFormatting.Header, StringComparison.OrdinalIgnoreCase) ? new IniParser(existing) : null;

	protected override bool IsSectionStart(in ReadOnlySpan<char> line)
		=> !line.StartsWith('[');
}
