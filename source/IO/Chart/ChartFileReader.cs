using ChartTools.IO.Chart.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools.IO.Chart
{
    /// <summary>
    /// Reader of text file that sends read lines to subscribers of its events.
    /// </summary>
    internal class ChartFileReader : TextFileReader
    {
        public override IEnumerable<ChartParser> Parsers => base.Parsers.Cast<ChartParser>();
        public override bool DefinedSectionEnd => true;

        public ChartFileReader(string path, Func<string, ChartParser?> parserGetter) : base(path, parserGetter) { }

        protected override bool IsSectionStart(string line) => line == "{";
        protected override bool IsSectionEnd(string line) => ChartFormatting.IsSectionEnd(line);
    }
}
