using ChartTools.Events;
using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Sessions;

using System.Collections.Generic;

namespace ChartTools.IO.Chart.Parsers
{
    internal class GlobalEventParser : ChartParser
    {
        public override List<GlobalEvent> Result => GetResult(result);
        private readonly List<GlobalEvent> result = new();

        public GlobalEventParser(ReadingSession session) : base(session) { }

        protected override void HandleItem(string line)
        {
            TrackObjectEntry entry = new(line);
            result.Add(new(entry.Position, entry.Data.Trim('"')));
        }

        public override void ApplyToSong(Song song) => song.GlobalEvents = Result;
    }
}
