namespace ChartTools.IO.Parsing;

internal abstract class TextParser(string header) : SectionParser<string>(header)
{
    protected override Exception GetHandleInnerException(string item, Exception innerException) => new LineException(item, innerException);
}
