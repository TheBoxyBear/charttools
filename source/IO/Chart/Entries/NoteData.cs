using System;
using System.Collections.Generic;
using System.Text;

namespace ChartTools.IO.Chart.Entries
{
    /// <summary>
    /// Line of chart data representing a <see cref="Note"/>
    /// </summary>
    internal ref struct NoteData
    {
        /// <summary>
        /// Value of <see cref="Note.NoteIndex"/>
        /// </summary>
        internal byte NoteIndex { get; }
        /// <summary>
        /// Value of <see cref="Note.Length"/>
        /// </summary>
        internal uint SustainLength { get; }

        /// <summary>
        /// Creates an instance of <see cref="NoteData"/>.
        /// </summary>
        /// <param name="data">Data section of the line in the file</param>
        /// <exception cref="FormatException"/>
        internal NoteData(string data)
        {
            string[] split = data.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

            if (split.Length < 2)
                throw ChartExceptions.NewEntry();

            try
            {
                NoteIndex = byte.Parse(split[0]);
                SustainLength = uint.Parse(split[1]);
            }
            catch { throw ChartExceptions.NewEntry(); }
        }
    }
}
