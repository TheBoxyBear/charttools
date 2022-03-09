using System;
using System.IO;

namespace ChartTools.IO
{
    public class SectionException : Exception
    {
        public string Header { get; }

        public SectionException(string header, Exception innerException) : base($"Section {header}: {innerException.Message}") => Header = header;

        public static SectionException EarlyEnd(string header) => new(header, new InvalidDataException("Section did not end within the provided lines"));
    }
}
