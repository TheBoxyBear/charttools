using ChartTools.IO.Chart.Parsers;

using System;

namespace ChartTools.IO.Ini
{
    internal class IniFileReader : TextFileReader<ChartParser>
    {
        public IniFileReader(string path, Func<string, ChartParser?> parserGetter) : base(path, parserGetter) { }

        protected override bool IsSectionStart(string line) => !line.StartsWith('[');
        protected override bool IsSectionEnd(string line) => line.StartsWith(']');
    }
}
