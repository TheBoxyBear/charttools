using System;
using System.Collections.Generic;

namespace ChartTools.IO.Chart
{
    internal static class ChartFormatting
    {
        internal const string DrumsHeaderName = "Drums";

        /// <summary>
        /// Part names of <see cref="Instruments"/> without the difficulty
        /// </summary>
        internal static readonly Dictionary<Instruments, string> instrumentHeaderNames = new()
        {
            { Instruments.Drums, DrumsHeaderName },
            { Instruments.GHLGuitar, "GHLGuitar" },
            { Instruments.GHLBass, "GHLBass" },
            { Instruments.LeadGuitar, "Single" },
            { Instruments.RhythmGuitar, "DoubleRhythm" },
            { Instruments.CoopGuitar, "DoubleGuitar" },
            { Instruments.Bass, "DoubleBass" },
            { Instruments.Keys, "Keyboard" }
        };

        internal static string Header(Enum instrument, Difficulty difficulty) => Header((Instruments)instrument, difficulty);
        internal static string Header(Instruments instrument, Difficulty difficulty) => Header(instrumentHeaderNames[instrument], difficulty);
        internal static string Header(string instrumentName, Difficulty difficulty) => Header(difficulty.ToString() + instrumentName);
        internal static string Header(string name) => $"[{name}]";

        internal static string Line(string header, string? value) => value is null ? string.Empty : $"  {header} = {value}";

        /// <summary>
        /// Gets the written data for a note.
        /// </summary>
        /// <param name="index">Value of <see cref="Note.NoteIndex"/></param>
        /// <param name="sustain">Value of <see cref="Note.Length"/></param>
        internal static string NoteData(byte index, uint sustain) => $"N {index} {sustain}";

        /// <summary>
        /// Gets the written value of a float.
        /// </summary>
        /// <param name="value">Value to get the written equivalent of</param>
        internal static string Float(float value) => ((int)(value * 1000)).ToString().Replace(".", "").Replace(",", "");
    }
}
