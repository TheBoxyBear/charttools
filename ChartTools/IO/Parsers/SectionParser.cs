namespace ChartTools.IO.Parsing;

internal abstract class SectionParser<T>(string header) : FileParser<T>
{
    public string Header { get; } = header;

    protected override Exception GetHandleException(T item, Exception innerException) => new SectionException(Header, GetHandleInnerException(item, innerException));
    protected abstract Exception GetHandleInnerException(T item, Exception innerException);
    protected override Exception GetFinalizeException(Exception innerException) => new SectionException(Header, innerException);
}
