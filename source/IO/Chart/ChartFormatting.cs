using System;
using System.Collections.Generic;

namespace ChartTools.IO.Chart
{
    internal static class ChartFormatting
    {
        public const string DrumsHeaderName = "Drums";
        public const string MetadataHeader = "[Song]";
        public const string SyncTrackHeader = "[SyncTrack]";
        public const string GlobalEventHeader = "[Events]";

        /// <summary>
        /// Part names of <see cref="InstrumentIdentity"/> without the difficulty
        /// </summary>
        public static readonly Dictionary<InstrumentIdentity, string> InstrumentHeaderNames = new()
        {
            { InstrumentIdentity.Drums, DrumsHeaderName },
            { InstrumentIdentity.GHLGuitar, "GHLGuitar" },
            { InstrumentIdentity.GHLBass, "GHLBass" },
            { InstrumentIdentity.LeadGuitar, "Single" },
            { InstrumentIdentity.RhythmGuitar, "DoubleRhythm" },
            { InstrumentIdentity.CoopGuitar, "DoubleGuitar" },
            { InstrumentIdentity.Bass, "DoubleBass" },
            { InstrumentIdentity.Keys, "Keyboard" }
        };

        public static string Header(Enum instrument, Difficulty difficulty) => Header((InstrumentIdentity)instrument, difficulty);
        public static string Header(InstrumentIdentity instrument, Difficulty difficulty) => Header(InstrumentHeaderNames[instrument], difficulty);
        public static string Header(string instrumentName, Difficulty difficulty) => Header(difficulty.ToString() + instrumentName);
        public static string Header(string name) => $"[{name}]";

        public static string Line(string header, string? value) => value is null ? string.Empty : $"  {header} = {value}";

        /// <summary>
        /// Gets the written data for a note.
        /// </summary>
        /// <param name="index">Value of <see cref="Note.NoteIndex"/></param>
        /// <param name="sustain">Value of <see cref="Note.Length"/></param>
        public static string NoteData(byte index, uint sustain) => $"N {index} {sustain}";

        /// <summary>
        /// Gets the written value of a float.
        /// </summary>
        /// <param name="value">Value to get the written equivalent of</param>
        public static string Float(float value) => ((int)(value * 1000)).ToString().Replace(".", "").Replace(",", "");

        public static bool IsSectionEnd(string line) => line == "}";
    }
}
