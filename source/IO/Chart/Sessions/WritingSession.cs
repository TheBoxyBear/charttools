using System;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools.IO.Chart.Sessions
{
    internal class WritingSession
    {
        public delegate IEnumerable<string> ChordLinesGetter(Chord? previous, Chord current);
        public delegate bool DuplicateTrackObjectHandler(uint position, ICollection<uint> ignored, string objectType);

        public WritingConfiguration Configuration { get; }
        public ChordLinesGetter GetChordLines { get; }
        public DuplicateTrackObjectHandler DuplicateTrackObjectProcedure { get; }
        public uint HopoThreshold { get; set; }

        public WritingSession(WritingConfiguration? config)
        {
            Configuration = config ?? ChartParser.DefaultWriteConfig;
            GetChordLines = Configuration.UnsupportedModifierPolicy switch
            {
                UnsupportedModifierPolicy.IgnoreChord => (_, _) => Enumerable.Empty<string>(),
                UnsupportedModifierPolicy.ThrowException => (_, chord) => throw new Exception($"Chord at position {chord.Position} as an unsupported modifier for the chart format. Consider using a different {nameof(UnsupportedModifierPolicy)} to avoid this error."),
                UnsupportedModifierPolicy.IgnoreModifier => (_, chord) => chord.GetChartNoteData(),
                UnsupportedModifierPolicy.Convert => (previous, chord) => chord.GetChartData(previous, this)
            };
            DuplicateTrackObjectProcedure = Configuration.DuplicateTrackObjectPolicy switch
            {
                DuplicateTrackObjectPolicy.IncludeAll => DuplicateIncludeAll,
                DuplicateTrackObjectPolicy.IncludeFirst => DuplicateIncludeFirst,
                DuplicateTrackObjectPolicy.ThrowException => DuplicateException
            };
        }

        private static bool DuplicateIncludeAll(uint position, ICollection<uint> ignored, string objectType) => true;
        private static bool DuplicateIncludeFirst(uint position, ICollection<uint> ignored, string objectType)
        {
            if (ignored.Contains(position))
                return false;

            ignored.Add(position);
            return true;
        }
        private static bool DuplicateException(uint position, ICollection<uint> ignored, string objectType)
        {
            if (ignored.Contains(position))
                throw new Exception($"Duplicate {objectType} on position {position}. Consider using a different {nameof(DuplicateTrackObjectPolicy)} to avoid this error.");

            ignored.Add(position);
            return true;
        }
    }

}
