using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Parsing;

namespace ChartTools.IO.Chart.Parsing
{
    internal abstract class ChartParser : TextParser, ISongAppliable
    {
        protected ChartParser(ReadingSession session, string header) : base(session, header) { }

        public abstract void ApplyToSong(Song song);
    }
}
