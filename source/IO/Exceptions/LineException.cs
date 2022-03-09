using System;

namespace ChartTools.IO
{
    public class LineException : FormatException
    {
        public string Line { get; }
        public LineException(string line, Exception innerException) : base($"Line \"{line}\": {innerException.Message}", innerException) => Line = line;
    }
}
