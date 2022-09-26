using System;

namespace ChartTools.IO.Chart.Entries
{
    /// <summary>
    /// Line of chart data representing a <see cref="LaneNote{TLane}"/>
    /// </summary>
    internal ref struct NoteData
    {
        /// <summary>
        /// Value of <see cref="LaneNote{TLane}.Index"/>
        /// </summary>
        internal byte Index { get; }
        /// <summary>
        /// Value of <see cref="LaneNote{TLane}.Length"/>
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
                throw new EntryException();

            Index = ValueParser.ParseByte(split[0], "note index");
            SustainLength = ValueParser.ParseUint(split[1], "sustain length");
        }
    }
}
