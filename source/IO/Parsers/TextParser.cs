using ChartTools.IO.Configuration.Sessions;

using System;

namespace ChartTools.IO.Parsers
{
    internal abstract class TextParser : SectionParser<string>
    {
        protected TextParser(ReadingSession session, string header) : base(header, session) { }

        protected override Exception GetHandleInnerException(string item, Exception innerException) => new LineException(item, innerException);
    }
}
