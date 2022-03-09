using ChartTools.IO.Configuration.Sessions;

using System;

namespace ChartTools.IO
{
    internal abstract class TextParser : FileParser<string>
    {
        protected TextParser(ReadingSession session) : base(session) { }

        protected override Exception GetHandleException(string item, Exception innerException) => new LineException(item, innerException);
    }
}
