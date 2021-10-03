using ChartTools.SystemExtensions;

using System;
using System.Collections.Generic;

namespace ChartTools.IO.Chart
{
    public static partial class ChartParser
    {
        internal delegate bool IncludeNotePolicy(uint position, byte noteIndex, ICollection<byte> ignored);

        /// <summary>
        /// Part names of <see cref="Instruments"/> without the difficulty
        /// </summary>
        private static readonly Dictionary<Instruments, string> partNames = new()
        {
            { Instruments.Drums, "Drums" },
            { Instruments.GHLGuitar, "GHLGuitar" },
            { Instruments.GHLBass, "GHLBass" },
            { Instruments.LeadGuitar, "Single" },
            { Instruments.RhythmGuitar, "DoubleRhythm" },
            { Instruments.CoopGuitar, "DoubleGuitar" },
            { Instruments.Bass, "DoubleBass" },
            { Instruments.Keys, "Keyboard" }
        };

        /// <summary>
        /// Gets the full part name for a track.
        /// </summary>
        /// <exception cref="ArgumentException"/>
        /// <param name="instrument">Instrument to include in the part name</param>
        /// <param name="difficulty">Difficulty to include in the part name</param>
        private static string GetFullPartName(Instruments instrument, Difficulty difficulty) => Enum.IsDefined(typeof(Difficulty), difficulty)
                ? $"{difficulty}{partNames[instrument]}"
                : throw new ArgumentException("Difficulty is undefined.");

        private static bool IncludeNoteAllPolicy(uint position, byte index, ICollection<byte> ignored) => true;
        private static bool IncludeNoteFirstPolicy(uint position, byte index, ICollection<byte> ignored)
        {
            if (ignored.Contains(index))
                return false;

            ignored.Add(index);
            return true;
        }
        private static bool IncludeNoteExceptionPolicy(uint position, byte index, ICollection<byte> ignored)
        {
            if (ignored.Contains(index))
                throw new Exception($"Duplicate note at position {position}");
            else
            {
                ignored.Add(index);
                return true;
            }
        }



        private static bool IncludeSyncTrackAllPolicy(uint position, ICollection<uint> ignored, string objectName) => true;
        private static bool IncludeSyncTrackFirstPolicy(uint position, ICollection<uint> ignored, string objectName)
        {
            if (ignored.Contains(position))
                return false;

            ignored.Add(position);
            return true;
        }
        private static bool IncludeSyncTrackExceptionPolicy(uint position, ICollection<uint> ignored, string objectName)
        {
            if (ignored.Contains(position))
                throw new Exception($"Duplicate {objectName} on position {position}");

            ignored.Add(position);
            return true;
        }
    }
}
