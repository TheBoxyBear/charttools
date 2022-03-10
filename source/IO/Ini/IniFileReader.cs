using ChartTools.IO.Chart.Parsers;
using ChartTools.IO.Ini.Parsers;

using System;

namespace ChartTools.IO.Ini
{
    internal class IniFileReader : TextFileReader
    {
        public IniFileReader(string path, Func<string, IniParser?> parserGetter) : base(path, parserGetter) { }

        protected override bool IsSectionStart(string line) => !line.StartsWith('[');
        protected override bool IsSectionEnd(string line) => line.StartsWith(']');
    }
}
