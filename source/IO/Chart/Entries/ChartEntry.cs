using System;
using System.Collections.Generic;
using System.Text;

namespace ChartTools.IO.Chart.Entries
{
    /// <summary>
    /// Line of chart file data
    /// </summary>
    internal ref struct ChartEntry
    {
        /// <summary>
        /// Text before the equal sign
        /// </summary>
        internal string Header { get; }
        /// <summary>
        /// Text after the equal sign
        /// </summary>
        internal string Data { get; }

        /// <summary>
        /// Creates an instance of <see cref="ChartEntry"/>.
        /// </summary>
        /// <param name="line">Line in the file</param>
        /// <exception cref="FormatException"/>
        internal ChartEntry(string line)
        {
            string[] split = line.Split('=', 2, StringSplitOptions.RemoveEmptyEntries);

            if (split.Length < 2)
                throw ChartExceptions.NewEntry();

            Header = split[0].Trim();
            Data = split[1].Trim();
        }
    }
}
