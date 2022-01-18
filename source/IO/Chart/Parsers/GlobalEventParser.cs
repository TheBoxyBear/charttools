using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Sessions;

using System;
using System.Collections.Generic;

namespace ChartTools.IO.Chart.Parsers
{
    internal class GlobalEventParser : ChartParser
    {
        private List<GlobalEvent>? preResult, result;

        public GlobalEventParser(ReadingSession session) : base(session) { }

        public override List<GlobalEvent>? Result => result;
        protected override void HandleItem(string line)
        {
            TrackObjectEntry entry;
            try { entry = new(line); }
            catch (Exception e) { throw ChartExceptions.Line(line, e); }

            GlobalEvent ev = new(entry.Position, entry.Data.Trim('"'));
            preResult!.Add(ev);
        }

        protected override void PrepareParse() => preResult = new();
        protected override void FinaliseParse() => result = preResult;

        public override void ApplyResultToSong(Song song) => song.GlobalEvents = result;
    }
}
