using System.Collections.Generic;

namespace ChartTools.IO.Sections
{
    public class Section<T> : List<T>
    {
        public string Header { get; }
        public Section(string header) => Header = header;
    }
}
