using ChartTools.IO.Parsing;
using ChartTools.Meta;
using ChartTools.Meta.Mapping;

namespace ChartTools.IO.Ini;

internal class IniParser(Metadata? existing = null)
	: TextParser(IniFormatting.Header.AsMemory())
{
#if NET5_0_OR_GREATER
	public override Metadata Result
		=> GetResultIfReady(m_result);
#else
	public new Metadata Result
		=> GetResultIfReady(m_result);

	protected override object? GetResult()
		=> Result;
#endif

	private readonly Metadata m_result = existing ?? new();

	protected override void HandleItem(in ReadOnlyMemory<char> line)
	{
		TextEntry entry = new(line);
		MetadataIniMapper.Shared.Set(m_result, entry.Key.Span, entry.Value.Span.Trim('"'));
	}

	protected override Exception GetHandleException(in ReadOnlyMemory<char> item, Exception innerException)
		=> new SectionException(IniFormatting.Header, GetHandleInnerException(item, innerException));
}
