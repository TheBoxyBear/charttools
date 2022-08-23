using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Sessions;
using ChartTools.SystemExtensions.Linq;

using System.Collections.Generic;
using System.Linq;

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
                    foreach (var entry in (chord.ChartSupportedModifiers ? chord.GetChartModifierData(previousChord, session) : session.GetChordEntries(previousChord, chord)).Concat(chord.GetChartNoteData()))
                        yield return entry;

                previousChord = chord;
            }
        }
    }
}
