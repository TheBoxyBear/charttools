using System;

namespace ChartTools.IO.Chart.Entries
{
    /// <summary>
    /// Line of chart file data representing a <see cref="TrackObject"/>
    /// </summary>
    internal struct TrackObjectEntry : IReadOnlyTrackObject
    {
        /// <summary>
        /// Value of <see cref="TrackObject.Position"/>
        /// </summary>
        public uint Position { get; }
        /// <summary>
        /// Type code of <see cref="TrackObject"/>
        /// </summary>
        public string Type { get; }
        /// <summary>
        /// Additional data
        /// </summary>
        public string Data { get; }

        /// <summary>
        /// Creates an instance of see<see cref="TrackObjectEntry"/>.
        /// </summary>
        /// <param name="line">Line in the file</param>
        /// <exception cref="FormatException"/>
        public TrackObjectEntry(string line)
        {
            TextEntry entry = new(line);

            string[] split = entry.Value.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

            if (split.Length < 2)
                throw new EntryException();

            Type = split[0];
            Data = split[1];

            Position = ValueParser.ParseUint(entry.Key, "position");
        }
        public TrackObjectEntry(uint position, string type, string data)
        {
            Position = position;
            Type = type;
            Data = data;
        }

        public override string ToString() => ChartFormatting.Line(Position.ToString(), $"{Type} {Data}");
    }
}
