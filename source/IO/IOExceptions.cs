using System;

namespace ChartTools.IO
{
    internal static class IOExceptions
    {
        /// <summary>
        /// Gets the <see cref="Exception"/> to throw when creating a <see cref="TextEntry"/> from an invalid line.
        /// </summary>
        public static Exception Entry(string line, Exception? innerException = null) => new FormatException($"Format of line \"{line}\" is invalid.", innerException);
    }
}
