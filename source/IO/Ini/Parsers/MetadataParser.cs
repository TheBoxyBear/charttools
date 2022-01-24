using ChartTools.IO.Configuration.Sessions;

using System;
using System.Collections.Generic;
using System.Text;

namespace ChartTools.IO.Ini.Parsers
{
    internal class MetadataParser : IniParser
    {
        public MetadataParser(ReadingSession session) : base(session)
        {
        }

        public override object? Result => throw new NotImplementedException();

        public override void ApplyResultToSong(Song song)
        {
            throw new NotImplementedException();
        }

        protected override void FinaliseParse()
        {
            throw new NotImplementedException();
        }

        protected override void HandleItem(string item)
        {
            throw new NotImplementedException();
        }
    }
}
