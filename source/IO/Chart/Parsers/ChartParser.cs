using ChartTools.IO.Configuration.Sessions;

namespace ChartTools.IO.Chart.Parsers
{
    internal abstract class ChartParser : FileParser<string>
    {

        public ChartParser(ReadingSession session) : base(session) { }
    }
}