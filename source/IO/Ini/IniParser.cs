using ChartTools.IO.Configuration.Sessions;
using System;

namespace ChartTools.IO.Ini
{
    internal class IniParser : TextParser
    {
        public IniParser(ReadingSession session) : base(session) { }

        public override object? Result => throw new NotImplementedException();

        public override void ApplyResultToSong(Song song)
        {
            throw new NotImplementedException();
        }

        protected override void HandleItem(string item)
        {
            throw new NotImplementedException();
        }
    }
}
