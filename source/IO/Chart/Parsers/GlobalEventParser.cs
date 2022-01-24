﻿using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Sessions;

using System;
using System.Collections.Generic;

namespace ChartTools.IO.Chart.Parsers
{
    internal class GlobalEventParser : ChartParser
    {
        public GlobalEventParser(ReadingSession session) : base(session) { }

        public override List<GlobalEvent> Result => GetResult(result);
        private readonly List<GlobalEvent> result = new();

        protected override void HandleItem(string line)
        {
            TrackObjectEntry entry;
            try { entry = new(line); }
            catch (Exception e) { throw ChartExceptions.Line(line, e); }

            GlobalEvent ev = new(entry.Position, entry.Data.Trim('"'));
            result.Add(ev);
        }

        public override void ApplyResultToSong(Song song)
        {
            if (result is not null)
                song.GlobalEvents = Result;
        }
    }
}
