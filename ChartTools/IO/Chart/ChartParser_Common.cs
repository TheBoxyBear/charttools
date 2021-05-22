using System;
using System.Collections.Generic;

namespace ChartTools.IO.Chart
{
    internal static partial class ChartParser
    {
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
    }
}
