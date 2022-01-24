using ChartTools.IO.Configuration.Sessions;

namespace ChartTools.IO.Ini.Parsers
{
    internal abstract class IniParser : FileParser<string>
    {
        protected IniParser(ReadingSession session) : base(session) { }
    }
}
