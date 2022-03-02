using ChartTools.Internal;
using ChartTools.Internal.Collections.Delayed;
using ChartTools.IO.Chart.Parsers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChartTools.IO.Chart
{
    /// <summary>
    /// Reader of text file that sends read lines to subscribers of its events.
    /// </summary>
    internal class ChartFileReader : TextFileReader<ChartParser>
    {
        public ChartFileReader(string path, Func<string, ChartParser?> parserGetter) : base(path, parserGetter) { }

        protected override bool IsSectionStart(string line) => line == "{";
        protected override bool IsSectionEnd(string line) => line == "}";
    }
}
