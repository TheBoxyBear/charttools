using ChartTools.IO.Chart.Parsers;
using System;

namespace ChartTools.IO.Chart
{
    /// <summary>
    /// Reader of text file that sends read lines to subscribers of its events.
    /// </summary>
    internal class ChartFileReader : TextFileReader
    {
        public ChartFileReader(string path, Func<string, TextParser?> parserGetter) : base(path, parserGetter) { }

        protected override bool IsSectionStart(string line) => line == "{";
        protected override bool IsSectionEnd(string line) => ChartFormatting.IsSectionEnd(line);
    }
}
