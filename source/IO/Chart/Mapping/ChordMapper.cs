using ChartTools.Extensions.Linq;
using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration;
using ChartTools.IO.Configuration.Sessions;

using System.Collections.Generic;

namespace ChartTools.IO.Chart.Mapping
{
    internal static class ChordMapper
    {
        public static IEnumerable<TrackObjectEntry> Map(IEnumerable<Chord> source, WritingSession session)
        {
            List<uint> orderedPositions = new();
            LaneChord? previousChord = null;

            foreach (var chord in source)
            {
                if (session.DuplicateTrackObjectProcedure(chord.Position, "chord", () =>
                {
                    var index = orderedPositions.BinarySearchIndex(chord.Position, out bool exactMatch);

                    if (!exactMatch)
                        orderedPositions.Insert(index, chord.Position);

                    return exactMatch;
                }))
                    foreach (var entry in chord.GetChartData(previousChord, (chord.ChartSupportedMoridier ? UnsupportedModifiersResults.Modifier : session.UnsupportedModifiersProcedure(chord)).HasFlag(UnsupportedModifiersResults.Modifier), session.Formatting!))
                        yield return entry;

                previousChord = chord;
            }
        }
    }
}
