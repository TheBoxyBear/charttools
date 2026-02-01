namespace ChartTools.IO.Parsing;

internal abstract class SectionParser<T>(in ReadOnlyMemory<char> header) : FileParser<T>
{
	public ReadOnlyMemory<char> Header { get; } = header;

	protected override Exception GetHandleException(in T item, Exception innerException)
		=> new SectionException(Header.ToString(), GetHandleInnerException(item, innerException));

	protected abstract Exception GetHandleInnerException(T item, Exception innerException);

	protected override Exception GetFinalizeException(Exception innerException)
		=> new SectionException(Header.ToString(), innerException);
}
