using System;
using System.Collections.Generic;
using System.Text;

namespace ChartTools.IO.Chart
{
    public partial class ChartParser
    {
        /// <summary>
        /// Gets the <see cref="Exception"/> to throw from an entry.
        /// </summary>
        internal static Exception GetNewEntryException() => new FormatException("Format is invalid.");
    }
}
