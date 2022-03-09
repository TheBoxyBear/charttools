using System;

namespace ChartTools.IO
{
    public class EntryException : Exception
    {
        public EntryException() : base("Cannot convert divide line into entry elements.") { }
    }
}
