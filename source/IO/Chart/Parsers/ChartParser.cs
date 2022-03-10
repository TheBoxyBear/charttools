using ChartTools.IO.Configuration.Sessions;

using System;
using System.Collections.Generic;
using System.Text;

namespace ChartTools.IO.Chart.Parsers
{
    internal abstract class ChartParser : TextParser, ISongAppliable
    {
        protected ChartParser(ReadingSession session) : base(session) { }

        public abstract void ApplyToSong(Song song);
    }
}
