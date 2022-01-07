using System;
using System.Collections.Generic;
using System.Text;

namespace ChartTools.IO.Chart
{
    internal static class ChartExceptions
    {
        /// <summary>
        /// Generates an exception to throw when a line cannot be converted.
        /// </summary>
        /// <returns>Instance of <see cref="Exception"/> to throw</returns>
        /// <param name="line">Line that caused the exception</param>
        /// <param name="innerException">Exception caught when interpreting the line</param>
        public static Exception Line(string line, Exception innerException) => new FormatException($"Line \"{line}\": {innerException.Message}", innerException);
        /// <summary>
        /// Gets the <see cref="Exception"/> to throw from an entry.
        /// </summary>
        internal static Exception NewEntry() => new FormatException("Format is invalid.");
    }
}
