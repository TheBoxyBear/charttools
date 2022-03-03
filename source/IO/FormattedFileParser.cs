using ChartTools.Formatting;
using ChartTools.IO.Configuration.Sessions;

namespace ChartTools.IO
{
    internal abstract class FormattedFileParser<T> : FileParser<T>
    {
        protected FormattedFileParser(ReadingSession session, FormattingRules formatting) : base(session) { }
    }
}
