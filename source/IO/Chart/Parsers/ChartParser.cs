using ChartTools.IO.Configuration.Sessions;

namespace ChartTools.IO.Chart.Parsers
{
    internal abstract class ChartParser : TextParser
    {

        public ChartParser(ReadingSession session) : base(session) { }
    }
}