using ChartTools.IO.Configuration.Sessions;

using System;

namespace ChartTools.IO
{
    internal abstract class TextParser : FileParser<string>
    {
        public string SectionHeader { get; }

        protected TextParser(ReadingSession session, string header) : base(session) => SectionHeader = header;

        protected override Exception GetHandleException(string item, Exception innerException) => new SectionException(SectionHeader, new LineException(item, innerException));
    }
}
