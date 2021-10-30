using ChartTools.SystemExtensions;
using ChartTools.SystemExtensions.Linq;
using ChartTools.Tools.Optimizing;

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
                throw new Exception($"Duplicate {objectName} on position {position}. Consider using a different {nameof(DuplicateTrackObjectPolicy)} to avoid this error.");

            ignored.Add(position);
            return true;
        }

        private static void ApplyOverlappingStarPowerPolicy(UniqueTrackObjectCollection<StarPowerPhrase> starPower, OverlappingStarPowerPolicy policy)
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
