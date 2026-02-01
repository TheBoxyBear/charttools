using ChartTools.IO;

internal abstract class TextParser(in ReadOnlyMemory<char> header)
	: FileParser<ReadOnlyMemory<char>>
{
	public ReadOnlyMemory<char> Header { get; } = header;

	protected override Exception GetFinalizeException(Exception innerException)
		=> new SectionException(Header.ToString(), innerException);

	protected static Exception GetHandleInnerException(in ReadOnlyMemory<char> item, Exception innerException)
		=> new LineException(item.ToString(), innerException);
}
