using ChartTools.IO.Configuration.Sessions;

using System;
using System.Collections.Generic;
using System.Text;

namespace ChartTools.IO.Ini.Parsers
{
    internal class DifficultyParser : IniParser
    {
        public override object? Result => throw new NotImplementedException();

        public DifficultyParser(ReadingSession session) : base(session)
        {
        }

        public override void ApplyToSong(Song song)
        {
            throw new NotImplementedException();
        }

        protected override void HandleItem(string item)
        {
            throw new NotImplementedException();
        }
    }
}
