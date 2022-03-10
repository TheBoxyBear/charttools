using ChartTools.IO.Configuration.Sessions;

namespace ChartTools.IO.Ini.Parsers
{
    internal abstract class IniParser : TextParser, ISongAppliable
    {
        protected IniParser(ReadingSession session) : base(session) { }

        public abstract void ApplyToSong(Song song);
    }
}
