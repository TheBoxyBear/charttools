using System;

namespace ChartTools.IO
{
    internal static class IOExceptions
    {
        /// <summary>
        /// Gets the exception to throw when a <see cref="Instruments"/> value is not defined.
        /// </summary>
        /// <returns>Instance of <see cref="ArgumentException"/> to throw</returns>
        internal static ArgumentException GetUndefinedInstrumentException() => new ArgumentException($"Instrument is not defined.");
    }
}
