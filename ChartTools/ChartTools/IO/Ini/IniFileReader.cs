using ChartTools.IO.Parsing;
using ChartTools.IO.Sources;
using ChartTools.Meta;

namespace ChartTools.IO.Ini;

internal class IniFileReader(ReadingDataSource source, Metadata? existing) : TextFileReader(source)
{
#if NET5_0_OR_GREATER
	public override IEnumerable<IniParser> Parsers
		=> base.Parsers.Cast<IniParser>();
#else
	public new IEnumerable<IniParser> Parsers
		=> base.Parsers.Cast<IniParser>();

	protected override IEnumerable<FileParser<ReadOnlyMemory<char>>> GetFileParsers()
		=> Parsers;
#endif

	protected override TextParser? GetParser(in ReadOnlyMemory<char> header)
		=> header.Span.Equals(IniFormatting.Header, StringComparison.OrdinalIgnoreCase)
			? new IniParser(existing) : null;

	protected override bool IsSectionStart(in ReadOnlySpan<char> line)
		=> !line.StartsWith("[");
}
