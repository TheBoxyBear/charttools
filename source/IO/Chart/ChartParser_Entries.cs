using System;

namespace ChartTools.IO.Chart
{
    /// <summary>
    /// Provides methods for reading and writing chart files
    /// </summary>
    internal static partial class ChartParser
    {
        /// <summary>
        /// Line of chart file data
        /// </summary>
        private struct ChartEntry
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
                    throw GetNewEntryException();

                Header = split[0].Trim();
                Data = split[1].Trim();
            }
        }
        /// <summary>
        /// Line of chart file data representing a <see cref="TrackObject"/>
        /// </summary>
        private struct TrackObjectEntry
        {
            /// <summary>
            /// Value of <see cref="TrackObject.Position"/>
            /// </summary>
            internal uint Position { get; }
            /// <summary>
            /// Type code of <see cref="TrackObject"/>
            /// </summary>
            internal string Type { get; }
            /// <summary>
            /// Additional data
            /// </summary>
            internal string Data { get; }

            /// <summary>
            /// Creates an instance of see<see cref="TrackObjectEntry"/>.
            /// </summary>
            /// <param name="line">Line in the file</param>
            /// <exception cref="FormatException"/>
            internal TrackObjectEntry(string line)
            {
                ChartEntry entry = new(line);

                string[] split = entry.Data.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

                if (split.Length < 2)
                    throw GetNewEntryException();

                Type = split[0];
                Data = split[1];

                try { Position = uint.Parse(entry.Header); }
                catch { throw GetNewEntryException(); }
            }
        }

        /// <summary>
        /// Line of chart data representing a <see cref="Note"/>
        /// </summary>
        private struct NoteData
        {
            /// <summary>
            /// Value of <see cref="Note.NoteIndex"/>
            /// </summary>
            internal byte NoteIndex { get; }
            /// <summary>
            /// Value of <see cref="Note.SustainLength"/>
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
                    throw GetNewEntryException();

                try
                {
                    NoteIndex = byte.Parse(split[0]);
                    SustainLength = uint.Parse(split[1]);
                }
                catch { throw GetNewEntryException(); }
            }
        }

        /// <summary>
        /// Gets the <see cref="Exception"/> to throw from an entry.
        /// </summary>
        private static Exception GetNewEntryException() => new FormatException("Format is invalid.");
    }
}
