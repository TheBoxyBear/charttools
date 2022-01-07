using ChartTools.SystemExtensions;
using ChartTools.SystemExtensions.Linq;
using ChartTools.Tools.Optimizing;

using System;
using System.Collections.Generic;

namespace ChartTools.IO.Chart
{
    public static class common
    {
        internal delegate bool IncludeNotePolicy(uint position, byte noteIndex, ICollection<byte> ignored);

        private const string DrumsHeaderName = "Drums";

        /// <summary>
        /// Part names of <see cref="InstrumentIdentity"/> without the difficulty
        /// </summary>
        private static readonly Dictionary<InstrumentIdentity, string> instrumentHeaderNames = new()
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

        /// <summary>
        /// Gets the full part name for a track.
        /// </summary>
        /// <exception cref="ArgumentException"/>
        /// <param name="instrument">Instrument to include in the part name</param>
        /// <param name="difficulty">Difficulty to include in the part name</param>
        private static string GetFullPartName(InstrumentIdentity instrument, Difficulty difficulty) => Enum.IsDefined(typeof(Difficulty), difficulty)
                ? $"{difficulty}{instrumentHeaderNames[instrument]}"
                : throw new ArgumentException("Difficulty is undefined.");

        private static string CreateHeader(Enum instrument, Difficulty difficulty) => CreateHeader((InstrumentIdentity)instrument, difficulty);
        private static string CreateHeader(InstrumentIdentity instrument, Difficulty difficulty) => CreateHeader(instrumentHeaderNames[instrument], difficulty);
        private static string CreateHeader(string instrumentName, Difficulty difficulty) => CreateHeader(difficulty.ToString() + instrumentName);
        private static string CreateHeader(string name) => $"[{name}]";

        internal static bool IncludeSyncTrackAllPolicy(uint position, ICollection<uint> ignored, string objectName) => true;
        internal static bool IncludeSyncTrackFirstPolicy(uint position, ICollection<uint> ignored, string objectName)
        {
            if (ignored.Contains(position))
                return false;

            ignored.Add(position);
            return true;
        }
        internal static bool IncludeSyncTrackExceptionPolicy(uint position, ICollection<uint> ignored, string objectName)
        {
            if (ignored.Contains(position))
                throw new Exception($"Duplicate {objectName} on position {position}. Consider using a different {nameof(DuplicateTrackObjectPolicy)} to avoid this error.");

            ignored.Add(position);
            return true;
        }

        internal static void ApplyOverlappingStarPowerPolicy(IEnumerable<SpecicalPhrase> starPower, OverlappingStarPowerPolicy policy)
        {
            switch (policy)
            {
                case OverlappingStarPowerPolicy.Cut:
                    starPower.CutLengths();
                    break;
                case OverlappingStarPowerPolicy.ThrowException:
                    foreach ((var previous, var current) in starPower.RelativeLoop())
                        if (Optimizer.LengthNeedsCut(previous!, current!))
                            throw new Exception($"Overlapping star power phrases at position {current!.Position}. Consider using {nameof(OverlappingStarPowerPolicy.Cut)} to avoid this error.");
                    break;
            }
        }
    }
}
